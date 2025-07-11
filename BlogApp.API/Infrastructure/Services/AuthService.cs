using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Core.Interfaces;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoRegister;

namespace BlogApp.API.Infrastructure.Services;

[Register(ServiceLifetime.Scoped)]
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<BaseResponse<LoginResponse>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BaseResponse<LoginResponse>.Unauthorized("Invalid email or password");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return BaseResponse<LoginResponse>.Unauthorized("Invalid email or password");
        }

        var token = await GenerateJwtTokenAsync(user);
        var response = new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            UserName = user.UserName!
        };

        return BaseResponse<LoginResponse>.Success(response, "Login successful");
    }

    public async Task<BaseResponse<RegisterResponse>> RegisterAsync(string firstName, string lastName, string email, string userName, string password, string confirmPassword)
    {
        if (password != confirmPassword)
        {
            return BaseResponse<RegisterResponse>.ValidationError(new List<string> { "Passwords do not match" }, "Passwords do not match");
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return BaseResponse<RegisterResponse>.ValidationError(new List<string> { "User with this email already exists" }, "User with this email already exists");
        }

        var existingUserName = await _userManager.FindByNameAsync(userName);
        if (existingUserName != null)
        {
            return BaseResponse<RegisterResponse>.ValidationError(new List<string> { "Username is already taken" }, "Username is already taken");
        }

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BaseResponse<RegisterResponse>.ValidationError(errors, "Registration failed");
        }

        var response = new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            UserName = user.UserName!
        };

        return BaseResponse<RegisterResponse>.Success(response, "Registration successful");
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName)
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 