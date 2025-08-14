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
    public class SubjectService : ISubjectRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        IAuthRepository _authRepository;

        public SubjectService(AppDbContext context, IMapper mapper, IAuthRepository authRepository)
        {
            _context = context;
            _mapper = mapper;
            _authRepository = authRepository;
        }

        public async Task<IEnumerable<SubjectWithTeacherDto>> GetAllSubjects()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Teacher)
                .ToListAsync();

            return subjects.Select(s => new SubjectWithTeacherDto
            {
                Id = s.Id,
                SubjectName = s.SubjectName,
                SubjectCode = s.SubjectCode,
                Description = s.Description,
                Credits = s.Credits,
                TeacherName = s.Teacher?.Fullname ?? "No Teacher Assigned"
            }).ToList();
        }

        public async Task<Subject> GetSubjectById(int id)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<GeneralServiceResponse> CreateSubject(SubjectDto subjectDto)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();
            var subject = _mapper.Map<Subject>(subjectDto);

            var userEvent = new UserEvent
            {
                UserId = currentUser.Id,
                Timestamp = _authRepository.TimeStampFormat(),
                EventDescription = $"{currentUser.Username.Pascalize()} created subject: {subject.SubjectName}"
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Subjects.AddAsync(subject);
                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Subject created successfully"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GeneralServiceResponse> UpdateSubject(int id, SubjectDto subjectDto)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "Subject not found"
                };
            }

            var originalName = subject.SubjectName;

            _mapper.Map(subjectDto, subject);

            var userEvent = new UserEvent
            {
                UserId = currentUser.Id,
                Timestamp = _authRepository.TimeStampFormat(),
                EventDescription = $"{currentUser.Username.Pascalize()} updated subject: {originalName} → {subject.SubjectName}"
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Subjects.Update(subject);
                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Subject updated successfully"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<SubjectWithStudentsDto>> GetSubjectsByTeacherId(int teacherId)
        {
            var subjects = await _context.Subjects
                .Where(s => s.TeacherId == teacherId)
                .Include(s => s.StudentSubjects) // Ensure StudentSubjects is included
                .ThenInclude(ss => ss.Grade) // Then include the Grade for each StudentSubject
                .Include(s => s.StudentSubjects) // Re-include to ensure navigation is complete for Student
                .ThenInclude(ss => ss.User) // Then include the Student for each StudentSubject
                .Select(s => new SubjectWithStudentsDto
                {
                    Id = s.Id,
                    SubjectName = s.SubjectName,
                    SubjectCode = s.SubjectCode,
                    Description = s.Description,
                    Credits = s.Credits,
                    TeacherName = s.Teacher != null ? s.Teacher.Fullname : "No Teacher Assigned",
                    Students = s.StudentSubjects.Select(ss => new StudentWithGradesDto
                    {
                        StudentId = ss.StudentID,
                        Fullname = ss.User.Fullname,
                        MainGrade = ss.Grade != null ? ss.Grade.MainGrade : null,
                        CalculatedGrade = ss.Grade != null ? ss.Grade.CalculatedGrade : null
                    }).ToList()
                })
                .ToListAsync();

            return subjects;
        }
        public async Task<GeneralServiceResponse> DeleteSubject(int id)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "Subject not found"
                };
            }

            var userEvent = new UserEvent
            {
                UserId = currentUser.Id,
                Timestamp = _authRepository.TimeStampFormat(),
                EventDescription = $"{currentUser.Username.Pascalize()} deleted subject: {subject.SubjectName}"
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Subjects.Remove(subject);
                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Subject deleted successfully"
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
