using DataAccess.DAL;
using Domain.Dto;
using Domain.Dto.Auth.Responses;
using Domain.Dto.V1.Request;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;
        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK,Type =typeof(IEnumerable<ProjectResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] SearchDto dto)
        {
            return Ok(await _service.GetAllAsync(dto));
        }

        [HttpGet("{id:int}", Name = nameof(GetOneProject))]
        [ProducesResponseType(StatusCodes.Status200OK,Type=typeof(ProjectResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOneProject(int id)
        {
            return Ok(await _service.GetOneAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectResponseDto))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] ProjectRequestDto dto)
        {
           var project = await _service.InsertAsync(dto);
           return CreatedAtAction(nameof(GetOneProject), new { id = project.Id, }, project);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromForm] UpdateProjectRequestDto dto, int id)
        {
            await _service.UpdateAsync(dto, id);
            return NoContent();

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        [HttpDelete("force/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> forceDelete(int id)
        {
            await _service.ForceDeleteAsync(id);
            return NoContent();
        }
        [HttpPost("{id}/tasks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTasksToProject([FromBody] AddTasksDto tasks,int id)
        {
            await _service.AddTasksToProjectAsync(tasks,id);
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpDelete("{id}/tasks")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveTasksFromProject([FromBody] AddTasksDto tasks, int id)
        {
            await _service.RemoveTasksFromProjectAsync(tasks, id);
            return NoContent();
        }
        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserProjects()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            var projects = await _service.GetProjectsOfUserAsync(int.Parse(userId!.Value));

            return Ok(projects);
        }
        [HttpGet("{id}/tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProjectTasks(int id)
        {
            var tasksOfProject =await _service.GetProjectTasksAsync(id);

            return Ok(tasksOfProject);
        }
        [HttpGet("{id}/users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Users(int id)
        {
            var usersOfProject = _service.GetProjectUsers(id);
            return Ok(usersOfProject);
        }

    }
}
