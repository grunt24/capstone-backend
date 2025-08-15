using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.IRepositories
{
    public interface IGradeCalculationService
    {
        Task<MidtermGrade> CalculateAndSaveMidtermGradeAsync(MidtermGradeDto studentGradeDto);
    }
}
