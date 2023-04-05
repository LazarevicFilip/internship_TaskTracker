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
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(settings.GetSection("AzureAdB2C"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();
        }
        /// <summary>
        /// Add helper class that issue tokens to clients.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="settings"></param>
        public static void AddProjectOptions(this IServiceCollection service, IConfiguration settings)
        {
            var authConfig = new AuthConfig();
            settings.Bind(nameof(authConfig), authConfig);
            service.AddSingleton(authConfig);
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
        /// Add services
        /// </summary>
        /// <param name="services"></param>
        public static void AddProjectServices(this IServiceCollection services)
        {
            //add services(repositories)
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ILoggingService, ConsoleLogger>();
        }
        /// <summary>
        /// Add patterns
        /// </summary>
        /// <param name="services"></param>
        public static void AddProjectPatterns(this IServiceCollection services)
        {
            //Add patterns
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        
    }

   
}
