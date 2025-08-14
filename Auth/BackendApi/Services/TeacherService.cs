using AutoMapper;
using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApi.Services
{
    public class TeacherService : ITeacherRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _authRepository;

        public TeacherService(AppDbContext context, IMapper mapper, IAuthRepository authRepository)
        {
            _context = context;
            _mapper = mapper;
            _authRepository = authRepository;
        }

        public async Task<Teacher?> GetTeacherByUserIdAsync(int userId)
        {
            return await _context.Teachers.FirstOrDefaultAsync(t => t.UserID == userId);
        }

        public async Task<IEnumerable<TeacherWithSubjectsDto>> GetAllTeachers()
        {
            var teachers = await _context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.User)

                .ToListAsync();

            return _mapper.Map<IEnumerable<TeacherWithSubjectsDto>>(teachers);
        }

        public async Task<TeacherWithSubjectsDto> GetTeacherByUserId(int userId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserID == userId);

            return _mapper.Map<TeacherWithSubjectsDto>(teacher);
        }


        public async Task<TeacherWithSubjectsDto> GetTeacherById(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            return _mapper.Map<TeacherWithSubjectsDto>(teacher);
        }

        public async Task<GeneralServiceResponse> CreateTeacherWithAccountAsync(CreateTeacherWithAccountDto dto)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();

            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new StudentModel
                {
                    Username = dto.Username,
                    Role = UserRole.Teacher,
                    Fullname = dto.Fullname,
                };

                var hasher = new PasswordHasher<StudentModel>();
                user.Password = hasher.HashPassword(user, dto.Password);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var teacher = new Teacher
                {
                    Fullname = dto.Fullname,
                    UserID = user.Id
                };

                if (dto.SubjectIds.Any())
                {
                    teacher.Subjects = await _context.Subjects
                        .Where(s => dto.SubjectIds.Contains(s.Id))
                        .ToListAsync();
                }

                await _context.Teachers.AddAsync(teacher);

                var userEvent = new UserEvent
                {
                    UserId = currentUser.Id,
                    Timestamp = _authRepository.TimeStampFormat(),
                    EventDescription = $"{currentUser.Username.Pascalize()} created a teacher account for {teacher.Fullname}"
                };

                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Teacher account and subject assignment created successfully"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<GeneralServiceResponse> UpdateTeacher(int id, TeacherDto teacherDto)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();

            var existingTeacher = await _context.Teachers
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTeacher == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "Teacher not found"
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                existingTeacher.Fullname = teacherDto.Fullname;
                existingTeacher.UserID = teacherDto.UserId;

                existingTeacher.Subjects.Clear();

                if (teacherDto.SubjectIds.Any())
                {
                    var subjects = await _context.Subjects
                        .Where(s => teacherDto.SubjectIds.Contains(s.Id))
                        .ToListAsync();

                    foreach (var sub in subjects)
                        existingTeacher.Subjects.Add(sub);
                }

                var userEvent = new UserEvent
                {
                    UserId = currentUser.Id,
                    Timestamp = _authRepository.TimeStampFormat(),
                    EventDescription = $"{currentUser.Username.Pascalize()} updated teacher: {existingTeacher.Fullname}"
                };

                await _context.UserEvents.AddAsync(userEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Teacher updated successfully."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<GeneralServiceResponse> DeleteTeacher(int id)
        {
            var currentUser = await _authRepository.GetCurrentUserAsync();

            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.User) // Include User to access username for logging
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "Teacher not found"
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Nullify TeacherId on Subjects
                var subjectsWithTeacher = await _context.Subjects
                    .Where(s => s.TeacherId == teacher.Id)
                    .ToListAsync();

                foreach (var subject in subjectsWithTeacher)
                {
                    subject.TeacherId = null;
                }

                // 2. Optional: Clear navigation properties
                teacher.Subjects.Clear();

                // 3. Remove the Teacher
                _context.Teachers.Remove(teacher);

                // 4. Remove the associated User (if exists)
                var user = await _context.Users.FindAsync(teacher.UserID);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }

                // 5. Log the action
                var userEvent = new UserEvent
                {
                    UserId = currentUser.Id,
                    Timestamp = _authRepository.TimeStampFormat(),
                    EventDescription = $"{currentUser.Username.Pascalize()} deleted teacher: {teacher.Fullname}."
                };

                await _context.UserEvents.AddAsync(userEvent);

                // 6. Save changes and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Teacher and associated user deleted successfully."
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
