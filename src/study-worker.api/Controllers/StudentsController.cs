using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using study_worker.service.DTOs;
using study_worker.service.Interfaces;

namespace study_worker.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsByCourse(int courseId)
        {
            var students = await _studentService.GetStudentsByCourseIdAsync(courseId);
            return Ok(students);
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentDto studentDto)
        {
            var createdStudent = await _studentService.CreateStudentAsync(studentDto);
            return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.Id }, createdStudent);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, StudentDto studentDto)
        {
            if (id != studentDto.Id)
                return BadRequest();

            try
            {
                await _studentService.UpdateStudentAsync(studentDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollStudentInCourse(EnrollmentDto enrollmentDto)
        {
            var result = await _studentService.EnrollStudentInCourseAsync(enrollmentDto);

            if (!result)
                return BadRequest("Enrollment failed. Student or course not found.");

            return Ok();
        }

        [HttpPost("unenroll")]
        public async Task<IActionResult> RemoveStudentFromCourse(EnrollmentDto enrollmentDto)
        {
            var result = await _studentService.RemoveStudentFromCourseAsync(enrollmentDto);

            if (!result)
                return BadRequest("Unenrollment failed. Student or course not found or student is not enrolled in the course.");

            return Ok();
        }
    }
}
