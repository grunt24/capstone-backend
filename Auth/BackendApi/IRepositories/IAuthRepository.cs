using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.IRepositories
{
    public interface IAuthRepository
    {
        Task<GeneralServiceResponse> RegisterAsync(UserCredentialsDto loginDto);
        Task<LoginServiceResponse> LoginAsync(LoginDto loginDto);
        Task<GeneralServiceResponse> UpdateUserRoleAsync(int id, UserRoleUpdateDto dto);
        Task<IEnumerable<StudentDto>> GetAllUsersAsync();
        Task<IEnumerable<StudentDto>> GetAllStudents();
        Task<GeneralServiceResponse> UpdateUserDetailsAsync(int id, UserUpdateDto dto);
        Task<GeneralServiceResponse> DeleteStudentAsync(int id);
        //added
        Task<UserDto> GetCurrentUserAsync();
        Task<ResponseData<UserEvent>> GetUserLatestAction();
        string TimeStampFormat();
    }
}
