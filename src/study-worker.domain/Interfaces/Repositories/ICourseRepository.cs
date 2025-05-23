using study_worker.domain.Model;

namespace study_worker.domain.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(int id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(int id);
    }
}
