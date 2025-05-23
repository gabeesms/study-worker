using study_worker.service.DTOs;

namespace study_worker.service.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> GetStudentByIdAsync(int id);
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto);
        Task UpdateStudentAsync(StudentDto studentDto);
        Task DeleteStudentAsync(int id);
        Task<IEnumerable<StudentDto>> GetStudentsByCourseIdAsync(int courseId);
        Task<bool> EnrollStudentInCourseAsync(EnrollmentDto enrollmentDto);
        Task<bool> RemoveStudentFromCourseAsync(EnrollmentDto enrollmentDto);
    }
}
