using System.IdentityModel.Tokens.Jwt;
using BCrypt.Net;
using Capstone.Core.Entities;
using Capstone.Core.Entities.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Core.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenService _tokenService;

    public AuthService(AppDbContext context, JwtTokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<IEnumerable<User>> GetAllUser()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<string> RegisterAsync(string username, string password)
    {
        if (_context.Users.Any(us => us.Username == username))
        {
            throw new Exception("Username already exists");
        }

        // Hash the password securely
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Create a new user
        var user = new User
        {
            Username = username,
            Password = hashedPassword,
            Role = UserRole.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Save user to the database asynchronously

        // Generate JWT token for the new user
        return await _tokenService.GenerateJwtTokenAsync(user);
    }


    public async Task<LoginServiceResponse> LoginAsync(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new Exception("Invalid credentials!");
        }

        var newToken = await _tokenService.GenerateJwtTokenAsync(user);
        return new LoginServiceResponse
        {
            NewToken = newToken,
            Username = username,
            Role = user.Role.ToString()
        };
    }

}