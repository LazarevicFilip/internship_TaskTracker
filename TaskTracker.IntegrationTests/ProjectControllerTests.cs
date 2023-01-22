using Domain.Dto;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.IntegrationTests
{
    public class ProjectControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithNoProjects_ReturnsEmptyArray()
        {
            // Arrange
            var endpoint = "/api/projects";
            // Act
            var response = await _httpClient.GetAsync(endpoint);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var x = await (response.Content.ReadAsAsync<List<ProjectDto>>());
            x.Should().AllBeOfType<ProjectDto>();
            x.Should().OnlyHaveUniqueItems();
            x.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public async Task GetOne_ReturnsProject_WhenProjectExistsInDatabase()
        {
            //Arrange
            var project = await CreateProjectAsync(new ProjectDto
            {
                Name = "Project from tests",
                StartDate = new DateTime(2022, 11, 21),
                CompletionDate = new DateTime(2023, 12, 22),
                ProjectStatus = (DataAccess.DAL.Core.ProjectStatus)1,
                ProjectPriotiry = 1

            });
            var endpoint = $"/api/projects/{project.Id}";
            //Act
            var response = await _httpClient.GetAsync(endpoint);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returned = await response.Content.ReadAsAsync<ProjectDto>();
            returned.Id.Should().Be(project.Id);
            //Cleanup
            await DeleteProjectAsync(project.Id);
        }
       
    }
}
