using Domain.Dto;
using Domain.Dto.V1.Request;
using Microsoft.AspNetCore.Http;

namespace Domain.Interfaces.Services
{
    public interface ITaskService
    {
        Task<IList<TaskDto>> GetAllAsync(PagingDto dto);
        Task<TaskDto> GetOneAsync(int id);
        Task UpdateAsync(TaskDto task, int id);
        Task InsertAsync(TaskDto task);
        Task InsertTaskFilesAsync(FileRequestDto dto, string fileUri, string newFileName);
        Task DeleteTaskFilesAsync(int taskId, string fileName);
        Task DeleteAsync(int id);
        public List<FileResponseDto> GetTaskFiles(int taskId);
    }
}
