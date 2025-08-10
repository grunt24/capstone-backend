using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.IRepositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<SubjectWithTeacherDto>> GetAllSubjects();
        Task<Subject> GetSubjectById(int id);
        Task<GeneralServiceResponse> CreateSubject(SubjectDto subjectDto);
        Task<GeneralServiceResponse> UpdateSubject(int id, SubjectDto subjectDto);
        Task<GeneralServiceResponse> DeleteSubject(int id);
        Task<IEnumerable<SubjectWithStudentsDto>> GetSubjectsByTeacherId(int teacherId);
    }
}
