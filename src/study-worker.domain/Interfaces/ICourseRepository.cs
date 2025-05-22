using study_worker.domain.Model;

namespace study_worker.domain.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(Guid id);
    }
}
