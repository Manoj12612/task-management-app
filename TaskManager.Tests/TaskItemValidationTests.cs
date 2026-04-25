using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TaskManager.Web.Models;
using Xunit;
using TaskStatus = TaskManager.Web.Models.TaskStatus;

namespace TaskManager.Tests
{
    public class TaskItemValidationTests
    {
        private List<ValidationResult> Validate(TaskItem task)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(task);
            Validator.TryValidateObject(task, context, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void ValidTask_ShouldPassAllValidations()
        {
            var task = new TaskItem
            {
                Title = "Write unit tests",
                AssignedTo = "Priya",
                DueDate = DateTime.Today.AddDays(3),
                Status = TaskStatus.New
            };

            var errors = Validate(task);

            Assert.Empty(errors);
        }

        [Fact]
        public void EmptyTitle_ShouldFailValidation()
        {
            var task = new TaskItem
            {
                Title = "",
                AssignedTo = "Raj",
                DueDate = DateTime.Today.AddDays(2),
                Status = TaskStatus.New
            };

            var errors = Validate(task);

            Assert.Contains(errors, e => e.MemberNames.Contains("Title"));
        }

        [Fact]
        public void EmptyAssignedTo_ShouldFailValidation()
        {
            var task = new TaskItem
            {
                Title = "Some task",
                AssignedTo = "",
                DueDate = DateTime.Today.AddDays(2),
                Status = TaskStatus.New
            };

            var errors = Validate(task);

            Assert.Contains(errors, e => e.MemberNames.Contains("AssignedTo"));
        }

        [Fact]
        public void PastDueDate_ShouldFailFutureDateValidation()
        {
            var task = new TaskItem
            {
                Title = "Old task",
                AssignedTo = "Dev",
                DueDate = DateTime.Today.AddDays(-1),
                Status = TaskStatus.New
            };

            var errors = Validate(task);

            Assert.Contains(errors, e => e.MemberNames.Contains("DueDate"));
        }

        [Fact]
        public void TodayAsDueDate_ShouldFailFutureDateValidation()
        {
            var task = new TaskItem
            {
                Title = "Same day task",
                AssignedTo = "Dev",
                DueDate = DateTime.Today,
                Status = TaskStatus.New
            };

            var errors = Validate(task);

            Assert.Contains(errors, e => e.MemberNames.Contains("DueDate"));
        }

        [Fact]
        public void TitleExceeding100Chars_ShouldFailValidation()
        {
            var task = new TaskItem
            {
                Title = new string('A', 101),
                AssignedTo = "Dev",
                DueDate = DateTime.Today.AddDays(1),
                Status = TaskStatus.New
            };

            var errors = Validate(task);

            Assert.Contains(errors, e => e.MemberNames.Contains("Title"));
        }
    }
}
