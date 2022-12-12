using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Services;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Dto;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;
        public TasksController(ITaskService service)
        {

            _service = service;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _service.GetAll());
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("{id}",Name = nameof(GetOne))]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(TaskDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOne(int id)
        {
            try
            {
                return Ok(await _service.GetOne(id));
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created,Type=typeof(TaskDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]TaskDto dto)
        {
            try
            {
                await _service.Insert(dto);
                return CreatedAtAction(nameof(GetOne),new {id = dto.Id},dto);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] TaskDto dto,int id)
        {
            try
            {
                await _service.Update(dto,id);
                return NoContent();
            }
            catch(EntityNotFoundException)
            {
                return NotFound();
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }
    }
}
