using Domain.Dto;

namespace Domain.Interfaces.Services
{
    public interface ITaskService
    {
        Task<IList<TaskDto>> GetAllAsync(PagingDto dto);
        Task<TaskDto> GetOneAsync(int id);
        Task UpdateAsync(TaskDto task, int id);
        Task InsertAsync(TaskDto task);
        Task DeleteAsync(int id);
    }
}
