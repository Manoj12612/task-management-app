using Microsoft.AspNetCore.Mvc;
using TaskManager.Web.Models;
using TaskManager.Web.Services;

namespace TaskManager.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public IActionResult Index()
        {
            var tasks = _taskService.GetAll();
            return View(tasks);
        }

        public IActionResult Create()
        {
            return View(new TaskItem { DueDate = DateTime.Today.AddDays(1) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskItem task)
        {
            if (!ModelState.IsValid)
                return View(task);

            _taskService.Add(task);
            TempData["Success"] = "Task created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var task = _taskService.GetById(id);
            if (task is null)
                return NotFound();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TaskItem task)
        {
            if (!ModelState.IsValid)
                return View(task);

            var updated = _taskService.Update(task);
            if (!updated)
                return NotFound();

            TempData["Success"] = "Task updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var task = _taskService.GetById(id);
            if (task is null)
                return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _taskService.Delete(id);
            TempData["Success"] = "Task deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
