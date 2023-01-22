using Domain.Dto;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;
        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK,Type =typeof(IEnumerable<ProjectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] SearchDto dto)
        {
            return Ok(await _service.GetAll(dto));
        }

        [HttpGet("{id}", Name = nameof(GetOneProject))]
        [ProducesResponseType(StatusCodes.Status200OK,Type=typeof(ProjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOneProject(int id)
        {
            return Ok(await _service.GetOne(id));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectDto))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] ProjectDto dto)
        {
           await _service.Insert(dto);
           return CreatedAtAction(nameof(GetOneProject), new { id = dto.Id, }, dto);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] ProjectDto dto, int id)
        {
            await _service.Update(dto, id);
            return NoContent();

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return NoContent();
        }
        [HttpDelete("force/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> forceDelete(int id)
        {
            await _service.forceDelete(id);
            return NoContent();
        }
        [HttpPost("{id}/tasks")]
        public async Task<IActionResult> AddTasksToProject([FromBody] AddTasksDto tasks,int id)
        {
            await _service.AddTasksToProject(tasks,id);
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpDelete("{id}/tasks")]
        public async Task<IActionResult> RemoveTasksFromProject([FromBody] AddTasksDto tasks, int id)
        {
            await _service.RemoveTasksFromProject(tasks, id);
            return NoContent();
        }

    }
}
