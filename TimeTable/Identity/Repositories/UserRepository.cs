using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _usersDbContext;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserRepository(ApplicationDbContext usersDbContext, IConfiguration configuration)
        {
            _usersDbContext = usersDbContext;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>(); // Instantiate the PasswordHasher
        }

        public async Task<Result<string>> Login(User user)
        {
            // Retrieve the user from the database
            var existingUser = await _usersDbContext.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null)
            {
                return Result<string>.Failure("Invalid credentials");
            }
            // Verify the hashed password using BCrypt
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password);
            if (!isPasswordValid)
            {
                return Result<string>.Failure("Invalid credentials");
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, user.Email)]),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Success(tokenHandler.WriteToken(token));

        }

        public async Task<Result<string>> Register(User user, CancellationToken cancellationToken)
        {
            _usersDbContext.Users.Add(user);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return Result<string>.Success(user.Email);
        }
        
        public async Task<Result<User>>GetByEmailAsync(string email)
        {
            var user = await _usersDbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Result<User>.Failure("User not found");
            }
            return Result<User>.Success(new User
            {
                Email = user.Email,
                AccountType = user.AccountType
            });
        }
    }
}
