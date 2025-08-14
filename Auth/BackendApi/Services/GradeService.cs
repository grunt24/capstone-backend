// BackendApi.Services/GradeService.cs
using AutoMapper;
using BackendApi.Context;
using BackendApi.Core;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackendApi.Services
{
    public class GradeService : IGradeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        IAuthRepository authRepository;

        public GradeService(AppDbContext context, IMapper mapper, IAuthRepository authRepository)
        {
            _context = context;
            _mapper = mapper;
            this.authRepository = authRepository;
        }

        public async Task<GeneralServiceResponse> SaveGradesAsync(List<SaveGradesDto> saveGradesDtoList)
        {
            if (saveGradesDtoList == null || !saveGradesDtoList.Any())
            {
                return new GeneralServiceResponse { Success = false, Message = "No grades provided." };
            }

            foreach (var saveGradesDto in saveGradesDtoList)
            {
                var studentSubject = await _context.StudentSubjects
                    .Include(ss => ss.Grade)
                        .ThenInclude(g => g.GradeItems)
                    .FirstOrDefaultAsync(ss => ss.StudentID == saveGradesDto.StudentId && ss.SubjectID == saveGradesDto.SubjectId);

                if (studentSubject == null)
                {
                    return new GeneralServiceResponse { Success = false, Message = $"Student-Subject record not found for Student ID {saveGradesDto.StudentId} and Subject ID {saveGradesDto.SubjectId}." };
                }

                // If a grade record doesn't exist, create one
                if (studentSubject.Grade == null)
                {
                    studentSubject.Grade = new Grade();
                    _context.Grades.Add(studentSubject.Grade);
                }

                // Clear existing grade items and let EF Core handle the deletion
                studentSubject.Grade.GradeItems.Clear();

                // Add new grade items from the DTO
                var newGradeItems = saveGradesDto.Scores.Select(s => new GradeItem
                {
                    Type = s.Type,
                    Score = s.Score,
                    Total = s.Total
                }).ToList();

                foreach (var item in newGradeItems)
                {
                    studentSubject.Grade.GradeItems.Add(item);
                }

                studentSubject.Grade.MainGrade = saveGradesDto.MainGrade;
                studentSubject.Grade.CalculatedGrade = saveGradesDto.CalculatedGrade;
            }

            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Grades saved successfully."
            };
        }

        public async Task<IEnumerable<GradeDto>> GetGradesBySubjectIdAsync(int subjectId)
        {
            // Get the current user from the auth repository
            var currentUser = await authRepository.GetCurrentUserAsync();

            // Check if the user is authorized (Teacher, Admin, Superadmin)
            if (currentUser == null ||
                (currentUser.Role != UserRole.Teacher &&
                currentUser.Role != UserRole.Admin &&
                currentUser.Role != UserRole.Superadmin))
            {
                throw new UnauthorizedAccessException("You are not authorized to view this data.");
            }

            // Fetch student subjects for the given subjectId
            var studentSubjects = await _context.StudentSubjects
                .Where(ss => ss.SubjectID == subjectId)
                .Include(ss => ss.User) // Include related User info
                .Include(ss => ss.Subject) // Include related Subject info
                .Include(ss => ss.Grade) // Include related Grade info
                    .ThenInclude(g => g.GradeItems) // Include related GradeItems info
                .ToListAsync();

            // Prepare a list of GradeDto objects
            var gradesDto = new List<GradeDto>();

            foreach (var studentSubject in studentSubjects)
            {
                var gradeDto = new GradeDto
                {
                    StudentSubjectId = studentSubject.Id,
                    StudentId = studentSubject.StudentID,
                    StudentFullname = studentSubject.User?.Fullname,
                    SubjectId = studentSubject.SubjectID,
                    SubjectName = studentSubject.Subject?.SubjectName,
                    MainGrade = studentSubject.Grade?.MainGrade,
                    CalculatedGrade = studentSubject.Grade?.CalculatedGrade,
                };

                // Populate score items list if grade items exist
                if (studentSubject.Grade?.GradeItems != null)
                {
                    gradeDto.Scores = studentSubject.Grade.GradeItems
                        .Select(gi => new GradeItemDto { Type = gi.Type, Score = gi.Score, Total = gi.Total })
                        .ToList();
                }

                gradesDto.Add(gradeDto);
            }

            return gradesDto;
        }
    }
}
