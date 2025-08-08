using BackendApi.Core.Models;

namespace BackendApi.IRepositories
{
    public interface IJwtTokenRepository
    {
        Task<string> GenerateJwtTokenAsync(StudentModel user);
    }
}
