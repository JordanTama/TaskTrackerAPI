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
    public async Task<ActionResult<List<TaskItem>>> GetAll([FromQuery] bool? isCompleted, [FromQuery] string? search)
    {
        return await _service.ReadAllAsync(isCompleted, search);
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