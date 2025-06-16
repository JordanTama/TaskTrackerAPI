using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Data
{
    public class TaskDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }

        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }
    }
}