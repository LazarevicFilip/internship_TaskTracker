using DataAccess.DAL;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Domain.Dto;
using System;
using Domain.Dto.V1.Request;
using System.Reflection.Metadata;

namespace TaskTracker.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient _httpClient;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>();
               
            _httpClient = appFactory.CreateClient();
        }
        protected async Task AuthenticateAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }
        protected async Task<ProjectResponseDto> CreateProjectAsync(ProjectRequestDto request)
        {
            //This is required because [FromFile] attribute on controller action
            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Name), "Name" },
                { new StringContent(request.StartDate.ToString("yyyy-MM-dd")), "StartDate" },
                { new StringContent(request.CompletionDate?.ToString("yyyy-MM-dd")), "CompletionDate" },
                { new StringContent(request.ProjectStatus.ToString()), "ProjectStatus" },
                { new StringContent(request.ProjectPriority.ToString()), "ProjectPriority" }
            };

            var endpoint = "/api/projects";
            var response = await _httpClient.PostAsync(endpoint, content);
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