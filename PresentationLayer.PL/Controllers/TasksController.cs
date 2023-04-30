using Domain.Dto;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Dto.V1.Request;
using DataAccess.DAL;

namespace PresentationLayer.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;
        private readonly IFileService _fileService;
        public TasksController(ITaskService service, IFileService fileService)
        {
            _service = service;
            _fileService = fileService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] PagingDto dto)
        {

            return Ok(await _service.GetAllAsync(dto));

        }
        [HttpGet("{id}",Name = nameof(GetOne))]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(TaskDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOne(int id)
        {

            return Ok(await _service.GetOneAsync(id));

        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created,Type=typeof(TaskDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] TaskDto dto)
        {
           
            await _service.InsertAsync(dto);
            return CreatedAtAction(nameof(GetOne),new {id = dto.Id},dto);
           
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] TaskDto dto,int id)
       {
            await _service.UpdateAsync(dto,id);
            return NoContent();
           
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost("file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadTaskFile([FromForm] FileRequestDto file)
        {
            var uri = await _fileService.UploadTaskFileAsync(file.File);

            await _service.InsertTaskFilesAsync(file,uri.Item1,uri.Item2);

            return Ok(new {uri = uri});
        }

        [HttpDelete("{id}/file/{fileName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTaskFile(int id, string fileName)
        {
            await _service.DeleteTaskFilesAsync(id,fileName);

            await _fileService.DeleteTaskFileAsync(fileName);

            return NoContent();
        }
        [HttpGet("{id}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetTaskFiles(int id)
        {
           var files = _service.GetTaskFiles(id);

            return Ok(files);
        }
    }
}
