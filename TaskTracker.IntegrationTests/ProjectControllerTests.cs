using Azure.Core;
using DataAccess.DAL.Core;
using Domain.Dto;
using Domain.Dto.V1.Request;
using Domain.Dto.V1.Responses;
using FluentAssertions;
using System.Net;


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
            var x = await (response.Content.ReadAsAsync<PagedResponse<ProjectResponseDto>>());
            //x.Should().BeEmpty();
            x.Data.Should().AllBeOfType<ProjectResponseDto>();
            x.Data.Should().OnlyHaveUniqueItems();
            x.Data.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public async Task GetOne_ReturnsProject_WhenProjectExistsInDatabase()
        {
            //Arrange
            var project = await CreateProjectAsync(new ProjectRequestDto
            {
                Name = "Project from tests",
                StartDate = new DateTime(2022, 11, 21),
                CompletionDate = new DateTime(2023, 12, 22),
                ProjectStatus = (DataAccess.DAL.Core.ProjectStatus.Active),
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
