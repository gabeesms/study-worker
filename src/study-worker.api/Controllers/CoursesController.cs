using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using study_worker.service.DTOs;
using study_worker.service.Interfaces;

namespace study_worker.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(CourseDto courseDto)
        {
            var createdCourse = await _courseService.CreateCourseAsync(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id }, createdCourse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseDto courseDto)
        {
            if (id != courseDto.Id)
                return BadRequest();

            try
            {
                await _courseService.UpdateCourseAsync(courseDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            return NoContent();
        }
    }
}
