using BackendApi.Core.Models.Dto;

namespace BackendApi.IRepositories
{
    public interface IStudentRepository
    {
        Task<StudentDto> GetStudentById(int id);
    }
}
