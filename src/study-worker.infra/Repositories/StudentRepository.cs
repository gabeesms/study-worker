using Microsoft.EntityFrameworkCore;
using study_worker.domain.Interfaces.Repositories;
using study_worker.domain.Model;
using study_worker.infra.Data;

namespace study_worker.infra.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId)
        {
            return await _context.Students
                .Where(s => s.Courses.Any(c => c.Id == courseId))
                .ToListAsync();
        }

        public async Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId)
        {
            var student = await _context.Students
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            var course = await _context.Courses.FindAsync(courseId);

            if (student == null || course == null)
                return false;

            if (student.Courses == null)
                student.Courses = new List<Course>();

            if (!student.Courses.Any(c => c.Id == courseId))
            {
                student.Courses.Add(course);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveStudentFromCourseAsync(int studentId, int courseId)
        {
            var student = await _context.Students
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null || student.Courses == null)
                return false;

            var course = student.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course != null)
            {
                student.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
