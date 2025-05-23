using study_worker.domain.Interfaces.Repositories;
using study_worker.domain.Model;
using study_worker.infra.Messaging;
using study_worker.service.DTOs;
using study_worker.service.Interfaces;

namespace study_worker.service.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMessagePublisher _messagePublisher;

        public CourseService(ICourseRepository courseRepository, IMessagePublisher messagePublisher)
        {
            _courseRepository = courseRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            return course == null ? null : MapToDto(course);
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(MapToDto);
        }

        public async Task<CourseDto> CreateCourseAsync(CourseDto courseDto)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _courseRepository.AddAsync(course);

            // Publish event to RabbitMQ usando nossa interface correta
            var courseCreatedEvent = new CourseCreatedEvent
            {
                EventType = "CourseCreated",
                CourseId = course.Id,
                Title = course.Title,
                Description = course.Description,
                CreatedAt = course.CreatedAt
            };

            // Usa o método PublishAsync da nossa interface
            await _messagePublisher.PublishAsync(courseCreatedEvent, "course_events");

            return MapToDto(course);
        }

        public async Task UpdateCourseAsync(CourseDto courseDto)
        {
            var existingCourse = await _courseRepository.GetByIdAsync(courseDto.Id);
            if (existingCourse == null)
                throw new KeyNotFoundException($"Course with ID {courseDto.Id} not found");

            existingCourse.Title = courseDto.Title;
            existingCourse.Description = courseDto.Description;

            await _courseRepository.UpdateAsync(existingCourse);
        }

        public async Task DeleteCourseAsync(int id)
        {
            await _courseRepository.DeleteAsync(id);
        }

        private CourseDto MapToDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CreatedAt = course.CreatedAt
            };
        }
    }
}
