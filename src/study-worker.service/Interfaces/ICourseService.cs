using study_worker.service.DTOs;

namespace study_worker.service.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> CreateCourseAsync(CourseDto courseDto);
        Task UpdateCourseAsync(CourseDto courseDto);
        Task DeleteCourseAsync(int id);
    }
}
