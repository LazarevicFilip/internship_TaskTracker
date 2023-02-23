using DataAccess.DAL;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Domain.Dto;
using System;

namespace TaskTracker.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient _httpClient;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        //var descriptor = services.SingleOrDefault(d =>
                        //     d.ServiceType == typeof(DbContextOptions<TaskContext>));

                        //if (descriptor != null)
                        //{
                        //    services.Remove(descriptor);
                        //}
                        //services.AddDbContext<TaskContext>(options => options.UseInMemoryDatabase("testDb"));
                    });
                });
            _httpClient = appFactory.CreateClient();
        }
        protected async Task AutheticateAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }
        protected async Task<ProjectResponseDto> CreateProjectAsync(ProjectResponseDto request)
        {
            var endpoint = "/api/projects";
            var response = await _httpClient.PostAsJsonAsync(endpoint, request);
            return await response.Content.ReadAsAsync<ProjectResponseDto>();
        }
        protected async Task<string> DeleteProjectAsync(int id)
        {
            var endpoint = $"/api/projects/force/{id}";
            var response = await _httpClient.DeleteAsync(endpoint);
            return await response.Content.ReadAsStringAsync();
        }
        private async Task<string> GetJwtAsync()
        {
            return await Task.Run(() => "123") ;
        }
    }
}