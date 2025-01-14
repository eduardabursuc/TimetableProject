using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

            return GetToken(user.Email, 24*60).Result;

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
        
        public async Task<Result<string>> UpdateUserAsync(User user)
        {
            var existingUser = await usersDbContext.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null)
            {
                return Result<string>.Failure("User not found");
            }
            existingUser.Password = user.Password;
            await usersDbContext.SaveChangesAsync();
            return Result<string>.Success("User updated successfully");
        }

        public Task<Result<string>> GetToken(string email, int minutes)
        {
            var user = GetByEmailAsync(email).Result;
            
            if (!user.IsSuccess)
            {
                return Task.FromResult(Result<string>.Failure("User not found"));
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Data.Email),
                new Claim(ClaimTypes.Role, user.Data.AccountType)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(Result<string>.Success(tokenHandler.WriteToken(token)));
        }
        
    }
}
