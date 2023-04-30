using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Options;
using BusinessLogic.BAL.Validators;
using DataAccess.DAL;
using Domain.Core;
using Domain.Dto.Auth;
using Domain.Dto.V1.Request;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RegisterUserValidator _registerValidator;
        private readonly JwtSettings _settings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TaskContext _db;
        public IdentityService(RegisterUserValidator registerValidator, JwtSettings settings, IUnitOfWork unitOfWork, TokenValidationParameters tokenValidationParameters, TaskContext db)
        {
            _registerValidator = registerValidator;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _tokenValidationParameters = tokenValidationParameters;
            _db = db;
        }
        /// <summary>
        /// Get User from Db. If there is user with provided credentials Jwt User is returned.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<AutheticationResult> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().SingleOrDefaultAsync(x => x.Email == dto.Email);

                if (user == null)
                {
                    throw new BadRequestException("User email/password is wrong.");

                }
                //Encrypt provided password and mathc with existing one form db. 
                var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

                if (!valid)
                {
                    throw new BadRequestException("User email/password is wrong.");
                }
                return await GenerateAutheticateResultForUserAsync(user);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Register user.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<AutheticationResult> RegisterAsync(UserRegisterDto dto)
        {
            try
            {
                _registerValidator.ValidateAndThrow(dto);
                var user = new Domain.Core.User
                {
                    UserName = dto.UserName,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                };
                await _unitOfWork.Repository<User>().InsertAsync(user);

                await _unitOfWork.Save();

                return await GenerateAutheticateResultForUserAsync(user);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AutheticationResult> RefreshTokenAsync(RefreshTokenRequest dto)
        {
            var validatedToken = GetPrincipalFromToken(dto.AccessToken);

            if(validatedToken == null)
            {
                return new AutheticationResult { IsSuccess = false , Errors = new string[] {"Ivalid token." } };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AutheticationResult { Errors = new[] { "This token hasn't expired yet." } };
            }
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == dto.RefreshToken);
            
            if (storedRefreshToken == null)
            {
                return new AutheticationResult { Errors = new[] { "This refresh token doesn't exists." } };
            }
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AutheticationResult { Errors = new[] { "This refresh token has expired." } };
            }
            if (storedRefreshToken.Invalidated)
            {
                return new AutheticationResult { Errors = new[] { "This refresh token has been invalidated." } };
            }
            if (storedRefreshToken.Used)
            {
                return new AutheticationResult { Errors = new[] { "This refresh token has been used." } };
            }
            if (storedRefreshToken.JwtId != jti)
            {
                return new AutheticationResult { Errors = new[] { "This refresh token does not match this JWT." } };
            }

            storedRefreshToken.Used = true;

            _db.RefreshTokens.Update(storedRefreshToken);

            await _db.SaveChangesAsync();

            var x = validatedToken.Claims.Single(x => x.Type == "UserId").Value;

            var user = await _db.Users.FindAsync(int.Parse((validatedToken.Claims.Single(x => x.Type == "UserId").Value)));

            return await GenerateAutheticateResultForUserAsync(user);
        }
        /// <summary>
        /// Verify that provided token is initially signed by our api.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns> 
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var valdiatedToken);

                if (!IsTokenWithValidSecurityAlgoritham(valdiatedToken))
                {
                    return null;
                }
                return principal;
            }catch (ArgumentException ex)
            {
                throw;
            }
        }
        private bool IsTokenWithValidSecurityAlgoritham(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase);
           
        }
        /// <summary>
        /// Make and return token.
        /// </summary>
        /// <param name="user">User obj.</param>
        /// <returns></returns>
        private async Task<AutheticationResult> GenerateAutheticateResultForUserAsync(User actor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //Write claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
                new Claim("UserId", actor.Id.ToString(), ClaimValueTypes.String, _settings.Issuer),
                new Claim(JwtRegisteredClaimNames.Email, actor.Email),
            };
            //Signing token
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Secret!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Create token
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: "any",
                claims: claims,
                notBefore: now,
                expires: now.Add(_settings.TokenLifeTime),
                //expires: now.AddDays(1),
                signingCredentials: credentials);

            var refreshTokenBytes = RandomNumberGenerator.GetBytes(64);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(refreshTokenBytes),
                JwtId = token.Id,
                UserId = actor.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();

            return new AutheticationResult
            {
                IsSuccess = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}
