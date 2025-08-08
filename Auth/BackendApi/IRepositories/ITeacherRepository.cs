using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.IRepositories
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<TeacherWithSubjectsDto>> GetAllTeachers();
        Task<TeacherWithSubjectsDto> GetTeacherById(int id);
        Task<GeneralServiceResponse> UpdateTeacher(int id, TeacherDto teacherDto);
        Task<GeneralServiceResponse> DeleteTeacher(int id);
        Task<Teacher?> GetTeacherByUserIdAsync(int userId);
        Task<GeneralServiceResponse> CreateTeacherWithAccountAsync(CreateTeacherWithAccountDto dto);
        Task<TeacherWithSubjectsDto> GetTeacherByUserId(int userId);

    }
}
