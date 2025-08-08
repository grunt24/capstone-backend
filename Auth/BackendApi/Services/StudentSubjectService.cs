using AutoMapper;
using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
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

        public StudentSubjectService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                        TeacherName = ss.Subject.Teacher?.Fullname ?? "No Teacher"
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

            var studentSubjects = dto.SubjectIds.Select(subjectId => new StudentSubject
            {
                StudentID = dto.StudentId,
                SubjectID = subjectId
            }).ToList();

            await _context.StudentSubjects.AddRangeAsync(studentSubjects);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Subjects added to student successfully."
            };
        }

        public async Task<GeneralServiceResponse> DeleteStudentSubject(int id)
        {
            var entity = await _context.StudentSubjects.FindAsync(id);
            if (entity == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "StudentSubject not found"
                };
            }

            _context.StudentSubjects.Remove(entity);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "StudentSubject deleted successfully"
            };
        }

        public async Task<GeneralServiceResponse> UpdateStudentSubjects(StudentSubjectsDto dto)
        {
            // Remove all existing student-subject relationships
            var existingSubjects = _context.StudentSubjects
                .Where(ss => ss.StudentID == dto.StudentId);

            _context.StudentSubjects.RemoveRange(existingSubjects);

            // Add new subjects
            if (dto.SubjectIds != null && dto.SubjectIds.Any())
            {
                var newStudentSubjects = dto.SubjectIds.Select(subjectId => new StudentSubject
                {
                    StudentID = dto.StudentId,
                }).ToList();

                await _context.StudentSubjects.AddRangeAsync(newStudentSubjects);
            }

            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Student subjects updated successfully"
            };
        }
    }
}
