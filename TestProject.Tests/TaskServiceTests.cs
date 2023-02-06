using BusinessLogic.BAL.Services;
using DataAccess.DAL.Core;
using Domain.Interfaces;
using Moq;
using System.Reflection;
using Domain.Dto;
using DataAccess.DAL;
using System.Threading.Tasks;
using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Logging;
using BusinessLogic.BAL.Validators.TaskValidators;
using Xunit;
using DataAccess.DAL.Extensions;
using System.Linq.Expressions;

namespace TaskTracker.UnitTests
{
    public class TaskServiceTests 
    {
        private readonly TaskService _sut;
        private readonly Mock<IUnitOfWork> _taskServiceMock = new Mock<IUnitOfWork>();
        private readonly Mock<ILoggingService> _loggerMock = new Mock<ILoggingService>();
        public TaskServiceTests()
        {

           
            _sut = new TaskService(_taskServiceMock.Object,  _loggerMock.Object);

        }
        [Fact]
        public async Task GetOne_ShouldReturnTask_WhenTaskExists()
        {
            //Arrange
            var taskId = 2;
            var taskName = "Test";
            var taskModel = new TaskModel
            {
                Id = taskId,
                Name = taskName,
            };
            _taskServiceMock.Setup(x => x.Repository<TaskModel>().SingleOrDefaultAsync(x => x.Id == taskId)).ReturnsAsync(taskModel);
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
            _taskServiceMock.Setup(x => x.Repository<TaskModel>().SingleOrDefaultAsync(It.IsAny<Expression<Func<TaskModel, bool>>>())).ReturnsAsync(()=> null!);
            //Act and Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetOne(It.IsAny<int>()));
        }
        [Fact]
        public async Task GetOne_ShouldReturnLogMessage_WhenTaskDoesNotExists()
        {
            //Arrange
            var taskId = 2;
            var taskName = "Test";
            var taskModel = new TaskModel
            {
                Id = taskId,
                Name = taskName,
            };
            _taskServiceMock.Setup(x => x.Repository<TaskModel>().SingleOrDefaultAsync(x => x.Id == taskId)).ReturnsAsync(taskModel);
            //Act
            var task = await _sut.GetOne(taskId);
            //Assert
            _loggerMock.Verify(x => x.LogInforamtion("Retrived a task with Id: {id} from GetOne method {repo}", task.Id, typeof(TaskService)), Times.Once);
        }
    }
}