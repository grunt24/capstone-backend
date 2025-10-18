using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using System.Threading.Tasks;

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
        Task<bool> AddQuizToMidtermGradeAsync(int studentId, int midtermGradeId, string label, int? score, int? total);
        //new
        Task<bool> AddQuizToMidtermGradeAsync(int studentId, int gradeId, string label, int score, int total);
        Task<bool> AddClassStandingToMidtermGradeAsync(int studentId, int gradeId, string label, int score, int total);
        Task<MidtermGrade> CalculateAndSaveSingleMidtermGradeAsyncV2(MidtermGradeDto dto);

        Task<MidtermGradeUploadResult> CalculateMidtermGradesForSubjectAsync(int subjectId, int academicPeriodId);
        Task<FinalsGradeUploadResult> CalculateFinalsGradesForSubjectAsync(int subjectId, int academicPeriodId);

        Task<IEnumerable<MidtermGradeDto>> GetGradesBySubjectAndPeriodAsync(int subjectId, int academicPeriodId);
        Task<bool> UpdateMidtermGradeAsync(int studentId, MidtermGradeDto updatedGrade);

        //Finals
        Task<ResponseData<IEnumerable<FinalsGradeDto>>> GetFinalsGradesBySubjectAndPeriodAsync(int subjectId, int academicPeriodId);
        Task<bool> UpdateFinalsGradeAsync(int studentId, FinalsGradeDto updatedGrade);

        //
        Task<MidtermGradeUploadResult> CalculateMidtermGradesForAllSubjectsAsync();
        Task<FinalsGradeUploadResult> CalculateFinalsGradesForAllSubjectsAsync();
    }
}
