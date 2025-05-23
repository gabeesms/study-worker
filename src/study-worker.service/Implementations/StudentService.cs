using study_worker.domain.Events;
using study_worker.domain.Interfaces.Repositories;
using study_worker.domain.Model;
using study_worker.infra.Messaging;
using study_worker.service.DTOs;
using study_worker.service.Interfaces;

namespace study_worker.service.Implementations
{

    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMessagePublisher _messagePublisher;

        public StudentService(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IMessagePublisher messagePublisher)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<StudentDto> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            return student == null ? null : MapToDto(student);
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToDto).ToList();
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
        {
            var student = new Student
            {
                Name = createStudentDto.Name,
                Email = createStudentDto.Email,
                RegisteredAt = DateTime.UtcNow
            };

            await _studentRepository.AddAsync(student);

            var studentCreatedEvent = new StudentCreatedEvent
            {
                EventType = "StudentCreated",
                StudentId = student.Id,
                Name = student.Name,
                Email = student.Email,
                RegisteredAt = student.RegisteredAt
            };

            await _messagePublisher.PublishAsync(studentCreatedEvent, "student_events");

            return MapToDto(student);
        }

        public async Task UpdateStudentAsync(StudentDto studentDto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(studentDto.Id);
            if (existingStudent == null)
                throw new KeyNotFoundException($"Student with ID {studentDto.Id} not found");

            existingStudent.Name = studentDto.Name;
            existingStudent.Email = studentDto.Email;

            await _studentRepository.UpdateAsync(existingStudent);

            var studentUpdatedEvent = new StudentUpdatedEvent
            {
                EventType = "StudentUpdated",
                StudentId = existingStudent.Id,
                Name = existingStudent.Name,
                Email = existingStudent.Email
            };

            await _messagePublisher.PublishAsync(studentUpdatedEvent, "student_events");
        }

        public async Task DeleteStudentAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);

            // Publish event
            var studentDeletedEvent = new StudentDeletedEvent
            {
                EventType = "StudentDeleted",
                StudentId = id
            };

            await _messagePublisher.PublishAsync(studentDeletedEvent, "student_events");
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsByCourseIdAsync(int courseId)
        {
            var students = await _studentRepository.GetStudentsByCourseIdAsync(courseId);
            return students.Select(MapToDto).ToList();
        }

        public async Task<bool> EnrollStudentInCourseAsync(EnrollmentDto enrollmentDto)
        {
            var result = await _studentRepository.EnrollStudentInCourseAsync(
                enrollmentDto.StudentId,
                enrollmentDto.CourseId);

            if (result)
            {
                // Publish enrollment event
                var enrollmentEvent = new StudentEnrolledEvent
                {
                    EventType = "StudentEnrolled",
                    StudentId = enrollmentDto.StudentId,
                    CourseId = enrollmentDto.CourseId,
                    EnrolledAt = DateTime.UtcNow
                };

                await _messagePublisher.PublishAsync(enrollmentEvent, "enrollment_events");
            }

            return result;
        }

        public async Task<bool> RemoveStudentFromCourseAsync(EnrollmentDto enrollmentDto)
        {
            var result = await _studentRepository.RemoveStudentFromCourseAsync(
                enrollmentDto.StudentId,
                enrollmentDto.CourseId);

            if (result)
            {
                var unenrollmentEvent = new StudentUnenrolledEvent
                {
                    EventType = "StudentUnenrolled",
                    StudentId = enrollmentDto.StudentId,
                    CourseId = enrollmentDto.CourseId,
                    UnenrolledAt = DateTime.UtcNow
                };

                await _messagePublisher.PublishAsync(unenrollmentEvent, "enrollment_events");
            }

            return result;
        }

        private StudentDto MapToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                RegisteredAt = student.RegisteredAt,
                Courses = student.Courses?.Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt
                }).ToList() ?? new List<CourseDto>()
            };
        }
    }

}
