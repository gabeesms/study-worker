namespace study_worker.domain.Model
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationInHours { get; set; }

        // Construtor padrão
        public Course() { }
        // Construtor com parâmetros
        public Course(Guid id, string name, string description, int durationInHours)
        {
            Id = id;
            Name = name;
            Description = description;
            DurationInHours = durationInHours;
        }
    }
}
