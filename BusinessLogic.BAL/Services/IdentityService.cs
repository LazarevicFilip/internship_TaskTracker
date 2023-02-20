using BusinessLogic.BAL.Options;
using BusinessLogic.BAL.User;
using BusinessLogic.BAL.Validators;
using Domain.Core;
using Domain.Dto.Auth;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.Identity.Client;

namespace BusinessLogic.BAL.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IRepository<Domain.Core.User> _repository;
        private readonly RegisterUserValidator _registerValidator;
       
        private readonly AuthConfig _config;
        public IdentityService(IRepository<Domain.Core.User> repository, RegisterUserValidator registerValidator, AuthConfig config)
        {
            _repository = repository;
            _registerValidator = registerValidator;
            _config = config;
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
                var user = await _repository.SingleOrDefaultAsync(x => x.Email == dto.Email);

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
                return await GenerateAutheticateResultForUserAsync(actor);
            }catch(Exception )
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
                await _repository.InsertAsync(user);

                return await GenerateAutheticateResultForUserAsync(new JwtUser
                {
                    Id = user.Id,
                    Identity = user.Email,
                    Email = user.Email,
                });
            }
            catch (Exception) 
            {
                throw;
            }
        }
        /// <summary>
        /// Make and return token.
        /// </summary>
        /// <param name="user">User obj.</param>
        /// <returns></returns>
        public async Task<AutheticationResult> GenerateAutheticateResultForUserAsync(JwtUser actor)
        {
            //Write claims
            //var claims = new List<Claim>
            //{
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //    new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
            //    new Claim("UserId", actor.Id.ToString(), ClaimValueTypes.String, _settings.Issuer),
            //    new Claim(JwtRegisteredClaimNames.Email, actor.Email)
            //};
            ////Signing token
            //var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Secret!));

            //var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            ////Create token
            //var now = DateTime.UtcNow;
            //var token = new JwtSecurityToken(
            //    issuer: _settings.Issuer,
            //    audience: "any",
            //    claims: claims,
            //    notBefore: now,
            //    expires: now.Add(_settings.TokenLifeTime),
            //    signingCredentials: credentials);
        
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder
                .Create(_config.ClientId)
                .WithClientSecret(_config.ClientSecret)
                .WithAuthority(new Uri(_config.Authority))
                .Build();
            string[] ResourceIds = new string[] { _config.ResourceId };
            
            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException();
            }
            return new AutheticationResult
            {
                IsSuccess = true,
                Token = result.AccessToken,
            };
        }
    }
}
