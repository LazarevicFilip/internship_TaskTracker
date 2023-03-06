using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using BusinessLogic.BAL.Cache;
using BusinessLogic.BAL.Logging;
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
var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    VisualStudioTenantId = builder.Configuration["VisualStudioTenantId"]
});
var client = new SecretClient(new Uri(builder.Configuration["VaultUri"]), credentials);
builder.Services.AddSingleton(client);
//add auth
builder.Services.AddJwtAuthetification(builder.Configuration);
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
//inject AzureStorageOptions object
builder.Services.AddSingleton(x =>
{
    return new AzureStorageOptions
    {
        AzureBlobStorageConnectionString = builder.Configuration["AzureBlobStorageConnectionString"],
        AzureBlobStorageContainer = builder.Configuration["AzureBlobStorageContainer"]
    };
});
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }




