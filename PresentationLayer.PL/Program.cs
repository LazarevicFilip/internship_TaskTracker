using Azure.Storage.Blobs;
using BusinessLogic.BAL.Cache;
using BusinessLogic.BAL.Options;
using BusinessLogic.BAL.Services;
using BusinessLogic.BAL.Storage;
using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using PresentationLayer.PL.Extensions;
using PresentationLayer.PL.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//add auth
builder.Services.AddJwtAuthetification(builder.Configuration);
//add jwt handler
//builder.Services.AddProjectOptions(builder.Configuration);

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

//add support for azure storage
builder.Services.AddSingleton(service => new BlobServiceClient(builder.Configuration["AzureBlobStorageConnectionString"]));
builder.Services.AddSingleton<IBlobService, BlobService>();

//var azureStorageOptions = builder.Configuration.GetSection("AzureStorageOptions").Get<AzureStorageOptions>();
//builder.Services.AddSingleton(azureStorageOptions);

builder.Services.AddProjectServices();
builder.Services.AddProjectPatterns();

builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientAppPolicy", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
//Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskTracker", Version = "v1" });
}).AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
//Add custom middleware. Global exception hanlder (global try/cacth block)
app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("ClientAppPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }




