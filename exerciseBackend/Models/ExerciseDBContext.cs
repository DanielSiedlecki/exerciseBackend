using Microsoft.EntityFrameworkCore;

namespace exerciseBackend.Models
{
    public class ExerciseDBContext : DbContext
    {
        public ExerciseDBContext(DbContextOptions<ExerciseDBContext> options) : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }
    }
}