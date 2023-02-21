using BusinessLogic.BAL.Auth;
using BusinessLogic.BAL.Logging;
using BusinessLogic.BAL.Services;
using BusinessLogic.BAL.User;
using BusinessLogic.BAL.Validators;
using BusinessLogic.BAL.Validators.ProjectsValidator;
using BusinessLogic.BAL.Validators.TaskValidators;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Interfaces.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace PresentationLayer.PL.Extensions
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// Configuration for JWT bearer auth schema
        /// </summary>
        /// <param name="service"></param>
        /// <param name="settings"></param>
        public static void AddJwtAuthetification(this IServiceCollection service,IConfiguration settings)
        {
            var issuer = $"{settings["AAD:Instance"]}{settings["AAD:TenantId"]}{"/v2.0"}";
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {

                    //options.Audience = settings["AAD:ClientId"];
                    //options.Authority = $"{settings["AAD:Instance"]}{settings["AAD:TenantId"]}";
                    //options.RequireHttpsMetadata = false;
                    //options.SaveToken = true;
                    //options.TokenValidationParameters = new TokenValidationParameters
                    //{

                    //    ValidIssuer = issuer,
                    //    ValidateIssuer = true,
                    //    ValidateLifetime = true,
                    //    ClockSkew = TimeSpan.Zero

                    //};
                });
            //service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(settings.GetSection("AzureAdB2C"));
            //service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //  .AddMicrosoftIdentityWebApi(options =>
            //  {
            //      settings.Bind("AzureAdB2C", options);
            //      options.TokenValidationParameters.NameClaimType = "name";
            //      options.Events = new JwtBearerEvents
            //      {
            //          OnTokenValidated = context =>
            //          {
            //              retur
            //              // Perform custom token validation here
            //          }
            //      };
            //  });

        }
        /// <summary>
        /// Add helper class that issue tokens to clients.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="settings"></param>
        public static void AddJwtManager(this IServiceCollection service, IConfiguration settings)
        {
            service.AddTransient(x =>
            {
                var context = x.GetService<TaskContext>();
                return new JwtManager(context!, settings);
            });
        }
        /// <summary>
        /// Add user to application. If token is not present or invalid, AnonymousUser is returned.
        /// </summary>
        /// <param name="services"></param>
        public static void AddUser(this IServiceCollection services)
        {
            services.AddTransient<IApplicationUser>(x =>
            {
                var request = x.GetService<IHttpContextAccessor>();

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
        /// <summary>
        /// Add validators (Fluent Validation Library)
        /// </summary>
        /// <param name="services"></param>
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddScoped<UpdateTaskValidator>();
            services.AddScoped<CreateTaskValidator>();
            services.AddScoped<UpdateProjectValidator>();
            services.AddScoped<CreateProjectValidator>();
            services.AddScoped<AddTasksDtoValidator>();
        }
        /// <summary>
        /// Add services and patterns
        /// </summary>
        /// <param name="services"></param>
        public static void AddScopedServices(this IServiceCollection services)
        {
            //add services(repositories)
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IProjectService, ProjectService>();
            //Add patterns
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILoggingService, ConsoleLogger>();
        }
    }
    
}
