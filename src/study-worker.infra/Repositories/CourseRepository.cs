using study_worker.domain.Interfaces;
using study_worker.domain.Model;
using study_worker.infra.Data;

namespace study_worker.infra.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }
    }
}
