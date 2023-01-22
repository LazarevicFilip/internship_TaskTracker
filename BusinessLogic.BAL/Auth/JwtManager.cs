using BusinessLogic.BAL.Dto;
using BusinessLogic.BAL.User;
using DataAccess.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Auth
{
    public class JwtManager
    {
        private readonly TaskContext _context;
        private readonly IConfiguration _settings;
        public JwtManager(
            TaskContext context,
            IConfiguration settings)
        {
            _context = context;
            _settings = settings;
        }
        /// <summary>
        /// Make and return token.
        /// </summary>
        /// <param name="dto">User credentials dto.</param>
        /// <returns></returns>
        public string MakeToken(UserLoginDto dto)
        {
            //Get User
            JwtUser actor = Login(dto);
            //Write claims
            var claims = new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _settings["Jwt:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64,  _settings["Jwt:Issuer"]),
                new Claim("UserId", actor.Id.ToString(), ClaimValueTypes.String,  _settings["Jwt:Issuer"]),
                new Claim("Email", actor.Email),
            };
            //Signing token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Create token
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _settings["Jwt:Issuer"],
                audience: "Any",
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(double.Parse(_settings["Jwt:Minutes"]!)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// Get User from Db. If there is user with provided credentials Jwt User is returned.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        private JwtUser Login(UserLoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == dto.UserName);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }
            //Encrypt provided password and mathc with existing one form db. 
            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!valid)
            {
                throw new UnauthorizedAccessException();
            }

            var actor = new JwtUser
            {
                Id = user.Id,
                Identity = user.Email,
                Email = user.Email,
            };
            return actor;
        }
    }
}

