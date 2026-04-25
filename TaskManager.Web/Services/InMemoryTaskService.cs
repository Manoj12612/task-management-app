using TaskManager.Web.Models;

namespace TaskManager.Web.Services
{
    public class InMemoryTaskService : ITaskService
    {
        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public IEnumerable<TaskItem> GetAll() => _tasks.ToList();

        public TaskItem? GetById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

        public void Add(TaskItem task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
        }

        public bool Update(TaskItem updated)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == updated.Id);
            if (existing is null) return false;

            existing.Title = updated.Title;
            existing.AssignedTo = updated.AssignedTo;
            existing.DueDate = updated.DueDate;
            existing.Status = updated.Status;
            return true;
        }

        public bool Delete(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task is null) return false;

            _tasks.Remove(task);
            return true;
        }
    }
}
