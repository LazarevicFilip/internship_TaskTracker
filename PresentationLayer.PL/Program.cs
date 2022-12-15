using BusinessLogic.BAL.Auth;
using BusinessLogic.BAL.Services;
using BusinessLogic.BAL.Validators;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PresentationLayer.PL.Extensions;
using PresentationLayer.PL.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var settings = new AppSettings();
builder.Configuration.Bind("Jwt",settings);
builder.Services.AddSingleton(settings);
//add auth
builder.Services.AddJwtAuthetification(builder.Configuration);
//add jwt handler
builder.Services.AddJwtManager(builder.Configuration);
//add http context
builder.Services.AddHttpContextAccessor();
//Add dbContext
//builder.Services.AddDbContext<TaskContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
//});
builder.Services.AddScoped<TaskContext>();

builder.Services.AddScoped<UpdateTaskValidator>();
builder.Services.AddScoped<CreateTaskValidator>();
builder.Services.AddScoped<ProjectValidator>();
//add user
builder.Services.AddUser();
//add services/repositories
 builder.Services.AddScoped<ITaskService, TaskService>();
 builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandler>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();




//builder.Services.AddTransient(x =>
//{
//    var optiinsBuilder = new DbContextOptionsBuilder<TaskContext>();
//    var connection = builder.Configuration["ConnectionString:DefaultConnection"];
//    optiinsBuilder.UseSqlServer(connection);
//    var options = optiinsBuilder.Options;
//    return new TaskContext(options);
//});