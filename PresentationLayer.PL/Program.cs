using BusinessLogic.BAL.Cache;
using BusinessLogic.BAL.Logging;
using BusinessLogic.BAL.Services;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using PresentationLayer.PL.Extensions;
using PresentationLayer.PL.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//add auth
builder.Services.AddJwtAuthetification(builder.Configuration);
//add jwt handler
builder.Services.AddJwtOptions(builder.Configuration);

//add http context
builder.Services.AddHttpContextAccessor();
//Add dbContext
builder.Services.AddDbContext<TaskContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]!);
});
//add in-memory cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped(typeof(ICacheProvider<>), typeof(CacheProvider<>));
//add validators
builder.Services.AddValidators();

//add user
builder.Services.AddUser();

builder.Services.AddScopedServices();

builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        }); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskTracker", Version = "v1" });
}).AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

//app.UseCors(builder => builder
//    .WithOrigins("https://your-tenant-name.b2clogin.com")
//    .AllowAnyHeader()
//    .AllowAnyMethod()
//);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
//Add custom middleware. Global exception hanlder (global try/cacth block)
app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
    //.RequireAuthorization(new AuthorizeAttribute());

app.Run();

public partial class Program { }




