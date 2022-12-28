using BusinessLogic.BAL.Services;
using BusinessLogic.BAL.Validators.ProjectsValidator;
using BusinessLogic.BAL.Validators;
using DataAccess.DAL.Core;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Moq;
using System.Reflection;
using Domain.Dto;
using DataAccess.DAL;
using System.Threading.Tasks;
using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Logging;

namespace TestProject.Tests
{
    public class TaskServiceTests
    {
        private readonly TaskService _sut;
        private readonly Mock<IUnitOfWork> _taskServiceMock = new Mock<IUnitOfWork>();
        private readonly Mock<ILoggingService> _loggerMock = new Mock<ILoggingService>();

        public TaskServiceTests()
        {
            _sut = new TaskService(_taskServiceMock.Object, _loggerMock.Object);

        }
        [Fact]
        public async Task GetOne_ShouldReturnTask_WhenTaskExists()
        {
            //Arrange
            var taskId = 2;
            var taskName = "Test";
            var project = new TaskModel
            {
                Id = taskId,
                Name = taskName,
            };
            _taskServiceMock.Setup(x => x.Repository<TaskModel>().FindAsync(taskId)).ReturnsAsync(project);
            //Act
            var task = await _sut.GetOne(taskId);
            //Assert
            Assert.Equal(taskId, task.Id);
            Assert.Equal(taskName, task.Name);
        }
        [Fact]
        public async Task GetOne_ShouldReturnNothing_WhenTaskDoesNotExists()
        {
            //Arrange
            _taskServiceMock.Setup(x => x.Repository<TaskModel>().FindAsync(It.IsAny<int>())).ReturnsAsync(()=> null!);
            //Act and Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetOne(It.IsAny<int>()));
        }
        [Fact]
        public async Task GetOne_ShouldReturnLogMessage_WhenTaskDoesNotExists()
        {
            //Arrange
            var taskId = 2;
            var taskName = "Test";
            var project = new TaskModel
            {
                Id = taskId,
                Name = taskName,
            };
            _taskServiceMock.Setup(x => x.Repository<TaskModel>().FindAsync(taskId)).ReturnsAsync(project);
            //Act
            var task = await _sut.GetOne(taskId);
            //Assert
            _loggerMock.Verify(x => x.LogInforamtion("Retrived a task with Id: {id} from GetOne method {repo}", task.Id, typeof(TaskService)), Times.Once);
        }
    }
}