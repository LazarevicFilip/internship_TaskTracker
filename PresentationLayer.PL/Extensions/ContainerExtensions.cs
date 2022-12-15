using BusinessLogic.BAL.Auth;
using BusinessLogic.BAL.User;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PresentationLayer.PL.Extensions
{
    public static class ContainerExtensions
    {
        public static void AddJwtAuthetification(this IServiceCollection service,IConfiguration settings)
        {
            //Configuration for JWT bearer auth schema
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = settings["Jwt:Issuer"],
                        ValidateIssuer = true,
                        ValidAudience = "Any",
                        ValidateAudience = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["Jwt:Key"])),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
        public static void AddJwtManager(this IServiceCollection service, IConfiguration settings)
        {
            service.AddTransient(x =>
            {
                var context = x.GetService<TaskContext>();
                var jwtSettings = x.GetService<AppSettings>();
                return new JwtManager(context!, settings);
            });
        }
        public static void AddUser(this IServiceCollection services)
        {
            services.AddTransient<IApplicationUser>(x =>
            {
                var request = x.GetService<IHttpContextAccessor>();
                //var header = accessor.HttpContext.Request.Headers["Authorization"];

                var claims = request.HttpContext.User;

                if (claims == null || claims.FindFirst("UserId") == null)
                {
                    return new AnonymousUser();
                }
                var user = new JwtUser
                {
                    Email = claims.FindFirst("Email").Value,
                    Id = Int32.Parse(claims.FindFirst("UserId").Value),
                    Identity = claims.FindFirst("Email").Value,
                };

                return user;
            });
        }
    }
}
