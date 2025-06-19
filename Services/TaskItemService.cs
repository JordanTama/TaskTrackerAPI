using TaskTrackerAPI.Data;
using TaskTrackerAPI.DTOs;
using TaskTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskTrackerAPI.Services;

public class TaskItemService(TaskDbContext context)
{
    private readonly TaskDbContext _context = context;

    public async Task<TaskItem> CreateAsync(TaskItemCreateDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        return task;
    }

    public async Task<List<TaskItem>> ReadAllAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<TaskItem?> ReadAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(int id, TaskItemUpdateDto dto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return false;
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }
}