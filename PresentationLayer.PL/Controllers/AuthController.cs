using BusinessLogic.BAL.Auth;
using BusinessLogic.BAL.Dto;
using BusinessLogic.BAL.Models;
using BusinessLogic.BAL.User;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly JwtManager _manager;
        private readonly TaskContext _context;
        private readonly IApplicationUser _user;

        public AuthController(JwtManager manager, TaskContext context, IApplicationUser user)
        {
            _manager = manager;
            _context = context;
            _user = user;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            var user = _manager.MakeToken(dto);
            if (user != null)
            {
                return Ok(new {Token= user });
            }
            return Unauthorized();
           
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return StatusCode(201);
        }
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var u = _user.Email;
            return Ok(u);
        }
       
    }
}
