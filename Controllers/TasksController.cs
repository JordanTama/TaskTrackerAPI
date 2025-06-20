using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.DTOs;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Services;

namespace TaskTrackerAPI.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(TaskItemService service) : ControllerBase
{
    private readonly TaskItemService _service = service;

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Post([FromBody] TaskItemCreateDto dto)
    {
        var task = await _service.CreateAsync(dto);
        return Created($"api/tasks/{task.Id}", task);
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskItem>>> GetAll(
        [FromQuery] bool? isCompleted,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder = "asc",
        [FromQuery] int? page = 1,
        [FromQuery] int? pageSize = 10)
    {
        // Validate the 'search' value
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLowerInvariant();
        }

        // Validate the 'sortBy' value
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            sortBy = sortBy.Trim().ToLowerInvariant();
            var validSortKeys = new[] { "title", "description", "iscompleted", "createdat" };

            if (!validSortKeys.Contains(sortBy))
            {
                return ValidationError(nameof(sortBy), sortBy);
            }
        }

        // Validate the 'sortOrder' value
        if (!string.IsNullOrWhiteSpace(sortOrder))
        {
            sortOrder = sortOrder.Trim().ToLowerInvariant();
            var validOrderKeys = new[] { "asc", "desc" };

            if (!validOrderKeys.Contains(sortOrder))
            {
                return ValidationError(nameof(sortOrder), sortOrder);
            }
        }

        // Validate the 'page' value
        if (page.HasValue && page.Value <= 0)
        {
            return ValidationError(nameof(page), page);
        }

        // Validate the 'pageSize' value
        if (pageSize.HasValue && (pageSize.Value <= 0 || pageSize > 100))
        {
            return ValidationError(nameof(pageSize), pageSize);
        }

        return await _service.ReadAllAsync(isCompleted, search, sortBy, sortOrder, page, pageSize);

        BadRequestObjectResult ValidationError<T>(string valueName, T value)
        {
            return BadRequest($"Invalid {valueName} value: {value?.ToString() ?? "null"}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> Get(int id)
    {
        var task = await _service.ReadAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] TaskItemUpdateDto dto)
    {
        bool result = await _service.UpdateAsync(id, dto);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        bool result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}