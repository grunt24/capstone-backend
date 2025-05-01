using BCASGradePortal.Core.Interfaces;
using BCASGradePortal.Core.Models;
using BCASGradePortal.Core.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BCASGradePortal.Core.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(AppDbContext context, JwtTokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<GeneralServiceResponse> RegisterAsync(string username, string password)
        {
            if (_context.Users.Any(us => us.Username == username))
            {
                throw new Exception("Username already exists");
            }

            var user = new User
            {
                Username = username,
                // Role = UserRole.User // Uncomment and set the role as needed
            };

            // Hash the password with the user context
            user.Password = _passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponse
            {
                Message = "User Created Successfully!"
            };
        }


        public async Task<LoginServiceResponse> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new Exception("Invalid credentials!");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid credentials!");
            }

            var newToken = await _tokenService.GenerateJwtTokenAsync(user);
            return new LoginServiceResponse
            {
                NewToken = newToken,
                Username = username,
                //Role = user.Role.ToString() // Uncomment and set the role as needed
            };
        }

        public async Task<IEnumerable<User>> UsersList()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
