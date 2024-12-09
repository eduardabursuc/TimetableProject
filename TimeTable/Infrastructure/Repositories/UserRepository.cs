using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Common;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext usersDbContext, IConfiguration configuration)
        : IUserRepository
    {
        private readonly PasswordHasher<User> _passwordHasher = new(); // Instantiate the PasswordHasher

        public async Task<Result<string>> Login(User user)
        {
            // Retrieve the user from the database
            var existingUser = await usersDbContext.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
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
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, existingUser.AccountType)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Success(tokenHandler.WriteToken(token));

        }

        public async Task<Result<string>> Register(User user, CancellationToken cancellationToken)
        {
            usersDbContext.Users.Add(user);
            await usersDbContext.SaveChangesAsync(cancellationToken);
            return Result<string>.Success(user.Email);
        }
        
        public async Task<Result<User>>GetByEmailAsync(string email)
        {
            var user = await usersDbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
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
