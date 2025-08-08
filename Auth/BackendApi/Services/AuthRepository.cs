using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackendApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<StudentModel> _passwordHasher;
        private readonly IJwtTokenRepository _tokenService;
        private readonly ITeacherRepository teacherRepository;

        public AuthRepository(
            AppDbContext context,
            IJwtTokenRepository tokenService,
            IConfiguration configuration
,
            ITeacherRepository teacherRepository)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<StudentModel>();
            this.teacherRepository = teacherRepository;
        }
        public async Task<IEnumerable<StudentDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new StudentDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role.ToString(),
                    Fullname = u.Fullname,
                    Department = u.Department,
                    YearLevel = u.YearLevel,
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<StudentDto>> GetAllStudents()
        {
            return await _context.Users
                .Where(u=>u.Role == UserRole.Student)
                .Select(u => new StudentDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role.ToString(),
                    Fullname = u.Fullname,
                    Department = u.Department,
                    YearLevel = u.YearLevel
                    

                })
                .ToListAsync();
        }


        public async Task<GeneralServiceResponse> RegisterAsync(UserCredentialsDto userCredential)
        {
            if (_context.Users.Any(us => us.Username == userCredential.Username))
            {
                throw new Exception("Username already exists");
            }

            var user = new StudentModel
            {
                Username = userCredential.Username,
                Role = UserRole.Student,
                Department = userCredential.Department,
                YearLevel = userCredential.YearLevel,
                Fullname = userCredential.Fullname
            };

            user.Password = _passwordHasher.HashPassword(user, userCredential.Password);

            await _context.AddAsync(user);
            _context.SaveChanges();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "User Created Successfully!"
            };
        }

        public async Task<LoginServiceResponse> LoginAsync(LoginDto userCredential)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userCredential.Username);

            if (user == null)
                throw new Exception("Invalid credentials!");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, userCredential.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
                throw new Exception("Invalid credentials!");

            var newToken = await _tokenService.GenerateJwtTokenAsync(user);

            string? userFullname = null;
            int returnId;

            if (user.Role == UserRole.Teacher)
            {
                var teacher = await teacherRepository.GetTeacherByUserIdAsync(user.Id);
                if (teacher == null)
                    throw new Exception("Associated teacher not found.");

                returnId = teacher.Id;
                userFullname = teacher.Fullname;
            }
            else
            {
                var student = await GetStudentByUserIdAsync(user.Id);
                userFullname = student?.Fullname ?? user.Username; // fallback if student fullname is missing
                returnId = user.Id;
            }

            return new LoginServiceResponse
            {
                NewToken = newToken,
                Id = returnId, // dynamic: teacher.Id or user.Id
                Username = user.Username,
                Fullname = userFullname,
                Role = user.Role.ToString(),
            };
        }

        private async Task<StudentModel?> GetStudentByUserIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(t => t.Id == userId);
        }

        public async Task<GeneralServiceResponse> UpdateUserDetailsAsync(int id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Update only if values are provided (null checks)
            if (!string.IsNullOrEmpty(dto.Fullname))
                user.Fullname = dto.Fullname;

            if (!string.IsNullOrEmpty(dto.Department))
                user.Department = dto.Department;

            if (!string.IsNullOrEmpty(dto.YearLevel))
                user.YearLevel = dto.YearLevel;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "User details updated successfully"
            };
        }


        public async Task<GeneralServiceResponse> UpdateUserRoleAsync(int id, UserRoleUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            user.Role = dto.Role;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = $"User role updated to {dto.Role}"
            };
        }
        public async Task<GeneralServiceResponse> DeleteStudentAsync(int id)
        {
            var student = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (student == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Optional: remove related records like StudentSubjects if cascade delete is not configured
            var relatedSubjects = await _context.StudentSubjects
                .Where(ss => ss.StudentID == id)
                .ToListAsync();

            _context.StudentSubjects.RemoveRange(relatedSubjects);

            _context.Users.Remove(student);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Student deleted successfully"
            };
        }


    }
}
