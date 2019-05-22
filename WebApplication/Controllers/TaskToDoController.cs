using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Application.Interfaces.Services.Domain;
using Microsoft.AspNetCore.Routing;
using System;

namespace WebApplication.Controllers
{
    public class TaskToDoController : Controller
    {
        private readonly ITaskToDoService _taskToDoService;

        public TaskToDoController(ITaskToDoService taskToDoService)
        {
            _taskToDoService = taskToDoService;
        }

        // GET: TaskToDo/Create/{userId}
        public IActionResult Create(Guid? userId)
        {
            ViewData["UserId"] = userId;
            return View();
        }

        // POST: TaskToDo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Start,DeadLine,UserId")] TaskToDo taskToDo)
        {
            if (ModelState.IsValid)
            {
                await _taskToDoService.AddAsync(taskToDo);
                return RedirectToAction("Index",
                      new RouteValueDictionary(
                          new { controller = "User", action = "Index", Id = taskToDo.UserId }));
            }
            ViewData["UserId"] = taskToDo.UserId;
            return View(taskToDo);
        }

        // GET: TaskToDo/Edit/{taskId}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskToDo = await _taskToDoService.GetByIdIncludingUserAsync(id.Value);
            if (taskToDo == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = taskToDo.UserId;
            return View(taskToDo);
        }

        // POST: TaskToDo/Edit/{taskId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Start,DeadLine,UserId")] TaskToDo taskToDo)
        {
            if (id != taskToDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _taskToDoService.UpdateAsync(taskToDo);
                return RedirectToAction("Index",
                      new RouteValueDictionary(
                          new { controller = "User", action = "Index", Id = taskToDo.UserId }));
            }
            ViewData["UserId"] = taskToDo.UserId;
            return View(taskToDo);
        }

        // GET: TaskToDo/Delete/{taskId}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskToDo = await _taskToDoService.GetByIdAsync(id);

            if (taskToDo == null)
            {
                return NotFound();
            }

            return View(taskToDo);
        }

        // POST: TaskToDo/Delete/{taskId}/{userId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid userId)
        {
            var taskToDo = await _taskToDoService.RemoveAsync(id);
            return RedirectToAction("Index",
                      new RouteValueDictionary(
                          new { controller = "User", action = "Index", Id = userId }));
        }

        // POST: TaskToDo/Complete/{taskId}/{userId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id, Guid userId)
        {
            await _taskToDoService.UpdateStatusAsync(id, true);
            return RedirectToAction("Index",
                      new RouteValueDictionary(
                          new { controller = "User", action = "Index", Id = userId }));
        }
    }
}
