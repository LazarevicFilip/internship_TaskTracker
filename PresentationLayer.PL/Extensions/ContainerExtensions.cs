using BusinessLogic.BAL.Services;
using BusinessLogic.BAL.Validators;
using BusinessLogic.BAL.Validators.ProjectsValidator;
using BusinessLogic.BAL.Validators.TaskValidators;
using DataAccess.DAL;
using Domain.Interfaces.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Core;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.BAL.Options;
using Microsoft.Identity.Web;
using DataAccess.DAL.Logging;

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
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidIssuer = settings["Issuer"],
                ValidAudience = "any",
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["Secret"])),
            };


            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidIssuer = settings["Issuer"],
                        ValidAudience = "any",
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["Secret"])),
                        ClockSkew = TimeSpan.Zero,
                        ValidateLifetime = true,
                    };
                });

            service.AddSingleton(tokenValidationParameters);
        }
        /// <summary>
        /// Add helper class that issue tokens to clients.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="settings"></param>
        public static void AddProjectOptions(this IServiceCollection service, IConfiguration settings)
        {
            var jwtSettings = new JwtSettings();
            settings.Bind(nameof(jwtSettings), jwtSettings);
            service.AddSingleton(jwtSettings);
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
            services.AddScoped<RegisterUserValidator>();
            services.AddScoped<FileRequestDtoValidator>();
        }
        /// <summary>
        /// Add services
        /// </summary>
        /// <param name="services"></param>
        public static void AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            //add services(repositories)
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ILoggingService, ConsoleLogger>();
            services.AddScoped<IFileService, FileService>();

        }
        /// <summary>
        /// Add patterns
        /// </summary>
        /// <param name="services"></param>
        public static void AddProjectPatterns(this IServiceCollection services)
        {
            //Add patterns
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

    }

   
}
