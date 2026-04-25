using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Assigned person is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string AssignedTo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Due date is required.")]
        [FutureDate(ErrorMessage = "Due date must be a future date.")]
        public DateTime DueDate { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.New;
    }

    public enum TaskStatus
    {
        New,
        InProgress,
        Completed
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
                return date.Date > DateTime.Today;
            return false;
        }
    }
}
