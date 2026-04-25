using TaskManager.Web.Models;

namespace TaskManager.Web.Services
{
    public interface ITaskService
    {
        IEnumerable<TaskItem> GetAll();
        TaskItem? GetById(int id);
        void Add(TaskItem task);
        bool Update(TaskItem task);
        bool Delete(int id);
    }
}
