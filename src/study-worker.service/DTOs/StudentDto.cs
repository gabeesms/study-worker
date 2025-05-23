namespace study_worker.service.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredAt { get; set; }
        public List<CourseDto> Courses { get; set; } = new List<CourseDto>();
    }

    public class CreateStudentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class EnrollmentDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
}
