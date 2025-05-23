namespace study_worker.domain.Events
{
    public class CourseCreatedEvent
    {
        public string EventType { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
