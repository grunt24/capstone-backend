using AutoMapper;
using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Repositories;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApi.Services
{
    public class StudentSubjectService : IStudentSubjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        IAuthRepository _authRepository;

        public StudentSubjectService(AppDbContext context, IMapper mapper, IAuthRepository authRepository)
        {
            _context = context;
            _mapper = mapper;
            _authRepository = authRepository;
        }

        

        public async Task<IEnumerable<StudentSubjectGroupedDto>> GetAllStudentSubjects()
        {
            var studentSubjects = await _context.StudentSubjects
                .Include(ss => ss.User)
                .Include(ss => ss.Subject)
                    .ThenInclude(s => s.Teacher)
                .ToListAsync();

            // Group by user
            var grouped = studentSubjects
                .Where(ss => ss.User != null && ss.Subject != null)
                .GroupBy(ss => ss.User!)
                .Select(g => new StudentSubjectGroupedDto
                {
                    UserId = g.Key.Id,
                    Fullname = g.Key.Fullname,
                    Subjects = g.Select(ss => new SubjectItemDto
                    {
                        SubjectId = ss.Subject!.Id,
                        SubjectName = ss.Subject.SubjectName,
                        SubjectCode = ss.Subject.SubjectCode,
                        TeacherName = ss.Subject.Teacher?.Fullname ?? "No Teacher",
                        Department = ss.Subject.Department

                    }).ToList()
                });

            return grouped;
        }




        public async Task<StudentSubject> GetStudentSubjectById(int id)
        {
            return await _context.StudentSubjects
                .Include(ss => ss.User)
                .Include(ss => ss.Subject)
                    .ThenInclude(t=>t.Teacher.Fullname)
                .FirstOrDefaultAsync(ss => ss.Id == id);
        }

        public async Task<GeneralServiceResponse> AddStudentSubject(StudentSubjectsDto dto)
        {
            if (dto.SubjectIds == null || !dto.SubjectIds.Any())
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "No subjects provided."
                };
            }

            var currentUser = await _authRepository.GetCurrentUserAsync();

            // Load subject names from DB
            var subjectNames = await _context.Subjects
                .Where(s => dto.SubjectIds.Contains(s.Id))
                .Select(s => s.SubjectName)
                .ToListAsync();

            // Load current academic period
            var academicPeriod = await _context.AcademicPeriods
                .FirstOrDefaultAsync(ap => ap.IsCurrent);

            if (academicPeriod == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "No current academic period found."
                };
            }

            var existingSubjects = await _context.StudentSubjects
    .Where(ss => ss.StudentID == dto.StudentId && dto.SubjectIds.Contains(ss.SubjectID))
    .Select(ss => ss.SubjectID)
    .ToListAsync();

            if (existingSubjects.Any())
            {
                var existingSubjectNames = await _context.Subjects
                    .Where(s => existingSubjects.Contains(s.Id))
                    .Select(s => s.SubjectName)
                    .ToListAsync();

                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = $"Student already has the following subject(s): {string.Join(", ", existingSubjectNames)}"
                };
            }

            var subjectList = string.Join(", ", subjectNames);

            var eventDescription = $"{currentUser.Username.Pascalize()} assigned subject(s) [{subjectList}] to student ID {dto.StudentId}.";

            var userEvent = new UserEvent
            {
                UserId = currentUser.Id,
                EventDescription = eventDescription,
                Timestamp = _authRepository.TimeStampFormat()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var studentSubjects = dto.SubjectIds.Select(subjectId => new StudentSubject
                {
                    StudentID = dto.StudentId,
                    SubjectID = subjectId
                }).ToList();

                await _context.StudentSubjects.AddRangeAsync(studentSubjects);

                // Create default midterm and finals grades for each subject
                foreach (var subjectId in dto.SubjectIds)
                {
                    var academicYear = $"{academicPeriod.StartYear}-{academicPeriod.EndYear}";
                    var semester = academicPeriod.Semester;

                    var midterm = new MidtermGrade
                    {
                        StudentId = dto.StudentId,
                        SubjectId = subjectId,
                        AcademicPeriodId = academicPeriod.Id,
                        Semester = semester,
                        AcademicYear = academicYear,
                        RecitationScore = 0,
                        AttendanceScore = 0,
                        SEPScore = 0,
                        ProjectScore = 0,
                        PrelimScore = 0,
                        PrelimTotal = 0,
                        MidtermScore = 0,
                        MidtermTotal = 0,
                        TotalMidtermGrade = 0,
                        TotalMidtermGradeRounded = 0,
                        GradePointEquivalent = 5.00
                    };

                    var finals = new FinalsGrade
                    {
                        StudentId = dto.StudentId,
                        SubjectId = subjectId,
                        AcademicPeriodId = academicPeriod.Id,
                        Semester = semester,
                        AcademicYear = academicYear,
                        RecitationScore = 0,
                        AttendanceScore = 0,
                        SEPScore = 0,
                        ProjectScore = 0,
                        FinalsScore = 0,
                        FinalsTotal = 0,
                        TotalQuizScore = 0,
                        ClassStandingTotalScore = 0,
                        ClassStandingAverage = 0,
                        ClassStandingPG = 0,
                        ClassStandingWeightedTotal = 0,
                        QuizPG = 0,
                        QuizWeightedTotal = 0,
                        SEPPG = 0,
                        SEPWeightedTotal = 0,
                        ProjectPG = 0,
                        ProjectWeightedTotal = 0,
                        TotalScoreFinals = 0,
                        OverallFinals = 0,
                        CombinedFinalsAverage = 0,
                        FinalsPG = 0,
                        FinalsWeightedTotal = 0,
                        TotalFinalsGrade = 0,
                        TotalFinalsGradeRounded = 0,
                        GradePointEquivalent = 5.00
                    };

                    await _context.MidtermGrades.AddAsync(midterm);
                    await _context.FinalsGrades.AddAsync(finals);
                }

                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Subjects and default grades added to student successfully."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<GeneralServiceResponse> DeleteStudentSubject(int id)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();

            var entity = await _context.StudentSubjects.FindAsync(id);
            if (entity == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "StudentSubject not found"
                };
            }

            var userEvent = new UserEvent
            {
                UserId = currentUser.Id,
                EventDescription = $"{currentUser.Username.Pascalize()} removed subject ID {entity.SubjectID} from student ID {entity.StudentID}",
                Timestamp = _authRepository.TimeStampFormat()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.StudentSubjects.Remove(entity);
                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "StudentSubject deleted successfully"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GeneralServiceResponse> UpdateStudentSubjects(StudentSubjectsDto dto)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();

            var existingSubjects = _context.StudentSubjects
                .Where(ss => ss.StudentID == dto.StudentId);

            var newStudentSubjects = dto.SubjectIds?.Select(subjectId => new StudentSubject
            {
                StudentID = dto.StudentId,
                SubjectID = subjectId
            }).ToList() ?? new List<StudentSubject>();

            var eventDescription = $"{currentUser.Username.Pascalize()} updated subjects for student ID {dto.StudentId}. Assigned {newStudentSubjects.Count} subject(s).";

            var userEvent = new UserEvent
            {
                UserId = currentUser.Id,
                EventDescription = eventDescription,
                Timestamp = _authRepository.TimeStampFormat()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.StudentSubjects.RemoveRange(existingSubjects);
                await _context.StudentSubjects.AddRangeAsync(newStudentSubjects);
                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Student subjects updated successfully"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
