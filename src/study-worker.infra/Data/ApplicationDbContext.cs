﻿using Microsoft.EntityFrameworkCore;
using study_worker.domain.Model;

namespace study_worker.infra.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Construtor para injeção de dependência
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Construtor padrão (necessário para migrações)
        public ApplicationDbContext() { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Students)
                .WithMany(s => s.Courses)
                .UsingEntity(j => j.ToTable("CourseStudent"));
        }
    }
}