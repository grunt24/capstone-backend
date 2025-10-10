using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Humanizer;
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
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthRepository(
            AppDbContext context,
            IJwtTokenRepository tokenService,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<StudentModel>();
            _contextAccessor = contextAccessor;
        }

        //Added
        public async Task<ResponseData<IEnumerable<UserEvent>>> GetUserLatestAction()
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser.Role is not (UserRole.Superadmin or UserRole.Admin))
            {
                throw new UnauthorizedAccessException("You are not authorized to view the latest events.");
            }


            var isSuperadmin = _context.Users.Any(u => u.Role == UserRole.Superadmin);

            var latestEvents = await _context.UserEvents
                .OrderByDescending(u => u.Timestamp) //latest event is in the top
                .Take(10)
                .ToListAsync();

            if (latestEvents != null)
            {
                return new ResponseData<IEnumerable<UserEvent>>
                {
                    Success = true,
                    Message = "Latest event fetched successfully.",
                    Data = latestEvents
                };
            }
            else
            {
                return new ResponseData<IEnumerable<UserEvent>>
                {
                    Success = true,
                    Message = "No Current events",
                    Data = null, 
                };
            }

        }
        public async Task<UserDto> GetCurrentUserAsync()
        {
            // Get the current user's claims from the HTTP context
            var userClaims = _contextAccessor.HttpContext?.User;

            // If there's no user or they are not authenticated, throw an exception
            if (userClaims == null || !userClaims.Identity.IsAuthenticated)
            {
                throw new Exception("User not authenticated");
            }

            // Extract user info from the claims
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var username = userClaims.FindFirst(ClaimTypes.Name)?.Value;

            // Find the user in the database using their ID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Map the user to a StudentDto to return
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Fullname = user.Fullname,
            };
        }
        public async Task<IEnumerable<StudentDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new StudentDto
                {
                    Id = u.Id,
                    StudentNumber = u.StudentNumber,
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
                    StudentNumber = u.StudentNumber,
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
            var currentUser = await GetCurrentUserAsync();

            // Check if username already exists
            if (_context.Users.Any(us => us.Username == userCredential.Username))
            {
                throw new Exception("Username already exists");
            }

            // Create new user
            var user = new StudentModel
            {
                StudentNumber = userCredential.StudentNumber,
                Username = userCredential.Username,
                Role = UserRole.Student,
                Department = userCredential.Department,
                YearLevel = userCredential.YearLevel,
                Fullname = userCredential.Fullname
            };

            // Hash password and set it on the user model
            user.Password = _passwordHasher.HashPassword(user, userCredential.Password);

            // Create a user event for tracking
            var transactionEvent = new UserEvent
            {
                UserId = currentUser.Id,
                EventDescription = $"{currentUser.Username.Pascalize()} created new user: {user.Username.ToUpper()}.",
                Timestamp = TimeStampFormat()
            };

            // Start a transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Add both entities
                await _context.AddAsync(user);
                await _context.AddAsync(transactionEvent);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback if anything goes wrong
                await transaction.RollbackAsync();
                // Handle or log the exception
                throw new Exception("An error occurred while saving the user and transaction event.", ex);
            }

            // Return success message
            return new GeneralServiceResponse
            {
                Success = true,
                Message = "User Created Successfully!"
            };
        }

        public async Task<LoginServiceResponse> LoginAsync(LoginDto userCredential)
        {
            //userCredential.Username = "superadmin";
            //userCredential.Password = "superadmin";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userCredential.Username);

            if (user == null)
                throw new Exception("Invalid credentials!");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, userCredential.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
                throw new Exception("Invalid credentials!");

            var newToken = await _tokenService.GenerateJwtTokenAsync(user);

            string? userFullname = null;
            int returnId;

            //added

            //user.LatestTransaction = $"{userCredential.Username.ToUpper()} Login at {DateTime.Now:yyyy-MM-dd hh:mm:ss}";

            var userEvent = new UserEvent
            {
                Timestamp = TimeStampFormat(),
                EventDescription = $"{user.Username.Pascalize()} Logged in",
                UserId = user.Id,
                User = user,
            };
            _context.UserEvents.Update(userEvent);
            await _context.SaveChangesAsync();

            if (user.Role == UserRole.Teacher)
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserID == user.Id);
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

        public string TimeStampFormat()
        {
            return $"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt}";
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
            var currentUser = await GetCurrentUserAsync();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var previousRole = user.Role;
            user.Role = dto.Role;

            var transactionEvent = new UserEvent
            {
                UserId = currentUser.Id,
                EventDescription = $"{currentUser.Username.Pascalize()} updated user '{user.Username}' role from {previousRole} to {dto.Role}.",
                Timestamp = TimeStampFormat()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Update(user);
                await _context.UserEvents.AddAsync(transactionEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponse
                {
                    Success = true,
                    Message = $"User role updated to {dto.Role}"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while updating user role and logging the event.", ex);
            }
        }

        public async Task<GeneralServiceResponse> DeleteStudentAsync(int id)
        {
            var currentUser = await GetCurrentUserAsync();

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

            var transactionEvent = new UserEvent
            {
                UserId = currentUser.Id,
                EventDescription = $"{currentUser.Username} deleted: {student.Username}",
                Timestamp = TimeStampFormat()
            };

            return new GeneralServiceResponse
            {
                Success = true,
                Message = "Student deleted successfully"
            };
        }


    }
}
