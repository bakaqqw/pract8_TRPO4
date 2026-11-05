using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pract8_TRPO4.Models;
using System.ComponentModel.DataAnnotations;

namespace pract8_TRPO4.Controllers
{
    [ApiController]
    [Route("api/v1/tasks")]
    [Authorize(Roles = "User,Admin")]
    public class TaskController : ControllerBase
    {
        private readonly UdilovTrpo48zadContext _context;

        public TaskController(UdilovTrpo48zadContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_context.Tasks.ToList());
        }

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var login = User.Identity!.Name;
            var tasks = _context.Tasks.Where(t => t.OwnerLogin == login).ToList();
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            if (task.OwnerLogin != User.Identity!.Name && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            return Ok(task);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "Введите название задачи!" });
            }

            var task = new Models.Task
            {
                Title = request.Title,
                Description = request.Description,
                OwnerLogin = User.Identity!.Name!,
                IsCompleted = false
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();
            return Created($"/api/v1/tasks/{task.Id}", task);
        }

        [HttpPut("{id:int}")]
        public IActionResult Put([FromRoute] int id, [FromBody] UpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "Введите название задачи!" });
            }

            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            if (task.OwnerLogin != User.Identity!.Name && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.IsCompleted = request.IsCompleted!.Value;
            _context.SaveChanges();
            return Ok(task);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            if (task.OwnerLogin != User.Identity!.Name && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return NoContent();
        }
        public class CreateRequest
        {
            [Required]
            public string Title { get; set; } = null!;
            [Required(AllowEmptyStrings = true)]
            public string Description { get; set; } = null!;
        }

        public class UpdateRequest : CreateRequest
        {
            [Required]
            public bool? IsCompleted { get; set; }
        }
    }
}
