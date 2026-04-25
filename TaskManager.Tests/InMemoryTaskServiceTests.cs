using TaskManager.Web.Models;
using TaskManager.Web.Services;
using Xunit;
using TaskStatus = TaskManager.Web.Models.TaskStatus;

namespace TaskManager.Tests
{
    public class InMemoryTaskServiceTests
    {
        private InMemoryTaskService CreateService() => new InMemoryTaskService();

        private TaskItem SampleTask(string title = "Fix login bug", string assignedTo = "Alice") => new TaskItem
        {
            Title = title,
            AssignedTo = assignedTo,
            DueDate = DateTime.Today.AddDays(5),
            Status = TaskStatus.New
        };

        [Fact]
        public void Add_ShouldAssignIncrementalId()
        {
            var service = CreateService();
            var task1 = SampleTask("Task One");
            var task2 = SampleTask("Task Two");

            service.Add(task1);
            service.Add(task2);

            Assert.Equal(1, task1.Id);
            Assert.Equal(2, task2.Id);
        }

        [Fact]
        public void GetAll_ShouldReturnAllAddedTasks()
        {
            var service = CreateService();
            service.Add(SampleTask("Task A"));
            service.Add(SampleTask("Task B"));

            var result = service.GetAll().ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_WithValidId_ShouldReturnCorrectTask()
        {
            var service = CreateService();
            service.Add(SampleTask("Deploy hotfix"));

            var result = service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Deploy hotfix", result.Title);
        }

        [Fact]
        public void GetById_WithInvalidId_ShouldReturnNull()
        {
            var service = CreateService();

            var result = service.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void Update_ExistingTask_ShouldModifyAllFields()
        {
            var service = CreateService();
            service.Add(SampleTask("Old Title", "Bob"));

            var updated = new TaskItem
            {
                Id = 1,
                Title = "New Title",
                AssignedTo = "Charlie",
                DueDate = DateTime.Today.AddDays(10),
                Status = TaskStatus.InProgress
            };

            var result = service.Update(updated);
            var task = service.GetById(1);

            Assert.True(result);
            Assert.Equal("New Title", task!.Title);
            Assert.Equal("Charlie", task.AssignedTo);
            Assert.Equal(TaskStatus.InProgress, task.Status);
        }

        [Fact]
        public void Update_NonExistentTask_ShouldReturnFalse()
        {
            var service = CreateService();

            var result = service.Update(new TaskItem { Id = 99, Title = "Ghost", AssignedTo = "X", DueDate = DateTime.Today.AddDays(1) });

            Assert.False(result);
        }

        [Fact]
        public void Delete_ExistingTask_ShouldRemoveIt()
        {
            var service = CreateService();
            service.Add(SampleTask());

            var deleted = service.Delete(1);
            var remaining = service.GetAll().ToList();

            Assert.True(deleted);
            Assert.Empty(remaining);
        }

        [Fact]
        public void Delete_NonExistentTask_ShouldReturnFalse()
        {
            var service = CreateService();

            var result = service.Delete(404);

            Assert.False(result);
        }
    }
}
