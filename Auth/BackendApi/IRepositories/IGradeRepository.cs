using BackendApi.Core.General;
using BackendApi.Core.Models.Dto;

namespace BackendApi.IRepositories
{
    public interface IGradeRepository
    {
        Task<GeneralServiceResponse> SaveGradesAsync(List<SaveGradesDto> saveGradesDto);
        Task<IEnumerable<GradeDto>> GetGradesBySubjectIdAsync(int subjectId);
    }
}
