using study_worker.domain.Model;

namespace study_worker.domain.Interfaces.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(int id);
        Task<IEnumerable<Student>> GetAllAsync();
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId);
        Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId);
        Task<bool> RemoveStudentFromCourseAsync(int studentId, int courseId);
    }
}
