using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.DTOs;

public class TaskItemCreateDto
{
    [Required]
    [StringLength(100)]
    public required string Title { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; } = false;
}