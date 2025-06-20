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

    public async Task<List<TaskItem>> ReadAllAsync(
        bool? isCompleted,
        string? search,
        string? sortBy,
        string? sortOrder,
        int? page,
        int? pageSize)
    {
        var query = _context.Tasks.AsQueryable();

        if (isCompleted.HasValue)
        {
            query = query.Where(task => task.IsCompleted == isCompleted.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(task => EF.Functions.Like(task.Title, $"%{search}%"));
        }

        bool orderAscending = sortOrder == "asc";

        query = sortBy switch
        {
            "title" => orderAscending
                                ? query.OrderBy(task => task.Title)
                                : query.OrderByDescending(task => task.Title),
            "description" => orderAscending
                                ? query.OrderBy(task => task.Description)
                                : query.OrderByDescending(task => task.Description),
            "iscompleted" => orderAscending
                                ? query.OrderBy(task => task.IsCompleted)
                                : query.OrderByDescending(task => task.IsCompleted),
            "createdat" => orderAscending
                                ? query.OrderBy(task => task.CreatedAt)
                                : query.OrderByDescending(task => task.CreatedAt),
            _ => query.OrderBy(task => task.Id),
        };
        
        if (page.HasValue && pageSize.HasValue)
        {
            int take = pageSize.Value;
            int pageStart = (page.Value - 1) * take;
            query = query.Skip(pageStart).Take(take);
        }

        return await query.ToListAsync();
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