using DataAccess.DAL.Core;
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
        public async Task GetAll_WithProjects_ReturnsUniqueArray()
        {
            // Arrange
            var endpoint = "/api/projects";
            // Act
            var response = await _httpClient.GetAsync(endpoint);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var x = await (response.Content.ReadAsAsync<List<ProjectResponseDto>>());
            //x.Should().BeEmpty();
            x.Should().AllBeOfType<ProjectResponseDto>();
            x.Should().OnlyHaveUniqueItems();
            x.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public async Task GetOne_ReturnsProject_WhenProjectExistsInDatabase()
        {
            //Arrange
            var project = await CreateProjectAsync(new ProjectResponseDto
            {
                Name = "Project from tests",
                StartDate = new DateTime(2022, 11, 21),
                CompletionDate = new DateTime(2023, 12, 22),
                ProjectStatus = (DataAccess.DAL.Core.ProjectStatus)1,
                ProjectPriority = Priority.Middle

            });
            var endpoint = $"/api/projects/{project.Id}";
            //Act
            var response = await _httpClient.GetAsync(endpoint);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returned = await response.Content.ReadAsAsync<ProjectResponseDto>();
            returned.Id.Should().Be(project.Id);
            //Cleanup
            await DeleteProjectAsync(project.Id);
        }
       
    }
}
