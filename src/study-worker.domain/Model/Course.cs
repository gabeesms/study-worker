﻿namespace study_worker.domain.Model
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
