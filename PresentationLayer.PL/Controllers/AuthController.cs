using Azure.Core;
using Azure.Identity;
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
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;

namespace PresentationLayer.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly IIdentityService _service;
        private readonly IConfiguration _configuration;
        public AuthController(
            //IIdentityService service,
            IConfiguration configuration)
        {
            //_service = service;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var x = _configuration["AzureAdB2C"];
            var y = _configuration.GetSection("AzureAdB2C");
            var z = y.Value;
            //var s = JsonConvert.SerializeObject(y.Value);
            //var ss = JsonConvert.DeserializeObject(y.Value);
            return Ok(new { x, y, z });


        }
        //[HttpPost("login")]
        //[ProducesResponseType(StatusCodes.Status200OK,Type = typeof(AuthSuccessResponse))]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        //{
        //    var response = await _service.LoginAsync(dto);

        //    return Ok(new AuthSuccessResponse
        //    {
        //        Token = response.Token,
        //    });

        //}
        //[HttpPost("register")]
        //[ProducesResponseType(StatusCodes.Status200OK,Type =typeof(AuthSuccessResponse))]
        //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        //{
        //     var response = await _service.RegisterAsync(dto);

        //    return Ok(new AuthSuccessResponse
        //    {
        //        Token = response.Token,
        //    });
        //}


    }
}
