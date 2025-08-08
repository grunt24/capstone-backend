using AutoMapper;
using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
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

        public TeacherService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            // Check if username already exists
            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            // 1. Create User
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

            // 2. Create Teacher (linked to user)
            var teacher = new Teacher
            {
                Fullname = dto.Fullname,
                UserID = user.Id
            };

            // 3. Assign Subjects
            if (dto.SubjectIds.Any())
            {
                teacher.Subjects = await _context.Subjects
                    .Where(s => dto.SubjectIds.Contains(s.Id))
                    .ToListAsync();
            }

            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Teacher account and subject assignment created successfully"
            };
        }

        public async Task<GeneralServiceResponse> UpdateTeacher(int id, TeacherDto teacherDto)
        {
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

            existingTeacher.Fullname = teacherDto.Fullname;
            existingTeacher.UserID = teacherDto.UserId;

            // Update Subjects
            existingTeacher.Subjects.Clear();
            if (teacherDto.SubjectIds.Any())
            {
                var subjects = await _context.Subjects
                    .Where(s => teacherDto.SubjectIds.Contains(s.Id))
                    .ToListAsync();

                foreach (var sub in subjects)
                    existingTeacher.Subjects.Add(sub);
            }

            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Teacher updated successfully."
            };
        }

        public async Task<GeneralServiceResponse> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            var subjectsWithTeacher = await _context.Subjects
    .Where(s => s.TeacherId == teacher.Id)
    .ToListAsync();

            foreach (var subject in subjectsWithTeacher)
            {
                subject.TeacherId = null;
            }

            // Optional: Clear navigation property to avoid tracking issues
            teacher.Subjects.Clear();


            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Teacher deleted successfully."
            };
        }
    }
}
