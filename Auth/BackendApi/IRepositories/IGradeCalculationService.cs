using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.IRepositories
{
    public interface IGradeCalculationService
    {
        //Midterm
        Task<MidtermGradeUploadResult> CalculateAndSaveMidtermGradesAsync(List<MidtermGradeDto> studentGradesDto);

        //addded
        Task<MidtermGrade> CalculateAndSaveSingleMidtermGradeAsync(MidtermGradeDto studentGradeDto);
        Task<ResponseData<IEnumerable<MidtermGradeDto>>> GetMidtermGrades();
        Task<ResponseData<string>> DeleteMidtermGradesAsync(List<int> gradeIds);
        Task<GradeWeights?> GetWeightsAsync();
        //==================================================================================
        //Finals
        Task<FinalsGrade> CalculateAndSaveFinalGradesAsync(FinalsGradeDto studentGradesDto);
        Task<ResponseData<IEnumerable<FinalsGradeDto>>> GetFinalGrades();
        Task<ResponseData<string>> DeleteFinalsGradesAsync(List<int> gradeIds);

    }
}
