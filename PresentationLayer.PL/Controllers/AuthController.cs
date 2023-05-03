using DataAccess.DAL;
using Domain.Dto.Auth;
using Domain.Dto.Auth.Responses;
using Domain.Dto.V1.Request;
using Domain.Interfaces;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var response = await _service.LoginAsync(dto);

            return Ok(new AuthSuccessResponse { AccessToken = response.Token, RefreshToken = response.RefreshToken, Message = "You've  successfully  sign in" });

        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var response = await _service.RegisterAsync(dto);

            return Ok(new AuthSuccessResponse { AccessToken = response.Token, RefreshToken = response.RefreshToken, Message = "You've  successfully  sign up" });
        }
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest dto)
        {
            var response = await _service.RefreshTokenAsync(dto);

            return response.IsSuccess ? Ok(new AuthSuccessResponse { AccessToken = response.Token, RefreshToken = response.RefreshToken }) : BadRequest(new AuthBadResponse { Error = response.Errors.ToArray() });
        }
    }
}