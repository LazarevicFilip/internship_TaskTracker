using BusinessLogic.BAL.User;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Core;
using Domain.Dto;
using Domain.Dto.Auth;
using Domain.Dto.Auth.Responses;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _service;
        public AuthController(IIdentityService service)
        {
            _service = service;
        }
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var response = await _service.LoginAsync(dto);

            return Ok(new AuthSuccessResponse
            {
                Token = response.Token,
            });

        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK,Type =typeof(AuthSuccessResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
             var response = await _service.RegisterAsync(dto);

            return Ok(new AuthSuccessResponse
            {
                Token = response.Token,
            });
        }


    }
}
