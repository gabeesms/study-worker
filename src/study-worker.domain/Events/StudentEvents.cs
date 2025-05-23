namespace study_worker.domain.Events
{
    public class StudentCreatedEvent
    {
        public string EventType { get; set; }
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public class StudentUpdatedEvent
    {
        public string EventType { get; set; }
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class StudentDeletedEvent
    {
        public string EventType { get; set; }
        public int StudentId { get; set; }
    }

    public class StudentEnrolledEvent
    {
        public string EventType { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
    }

    public class StudentUnenrolledEvent
    {
        public string EventType { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime UnenrolledAt { get; set; }
    }
}
