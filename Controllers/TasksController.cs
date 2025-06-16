using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.DTOs;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly TaskDbContext _context;

        public TasksController(TaskDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Post([FromBody] TaskItemCreateDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return Created($"api/tasks/{task.Id}", task);
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> GetAll()
        {
            return await _context.Tasks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> Get(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            return task == null ? NotFound() : Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TaskItem item)
        {
            var existing = await _context.Tasks.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = item.Title;
            existing.Description = item.Description;
            existing.IsCompleted = item.IsCompleted;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _context.Tasks.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(existing);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}