using BusinessLogic.BAL.Logging;
using BusinessLogic.BAL.Services;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PresentationLayer.PL.Extensions;
using PresentationLayer.PL.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//add auth
builder.Services.AddJwtAuthetification(builder.Configuration);

//add jwt handler
builder.Services.AddJwtManager(builder.Configuration);

//add http context
builder.Services.AddHttpContextAccessor();

//Add dbContext
builder.Services.AddDbContext<TaskContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]!);
});
//add validators
builder.Services.AddValidators();
//add user
builder.Services.AddUser();
builder.Services.AddScopedServices();

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
//Add custom middleware. Global exception hanlder (global try/cacth block)
app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();