using Infrastructure.DTOs;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class AuthService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    public async Task<bool> LoginAsync(UserSignInDTO loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            loginDto.Email,
            loginDto.Password,
            loginDto.RememberMe,
            lockoutOnFailure: false
        );

        return result.Succeeded;
    }

    public async Task<bool> AdminLoginAsync(AdminLoginDTO loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return false;

        // Check if the user is in the "Admin" role
        if (!await _userManager.IsInRoleAsync(user, "Admin"))
            return false;

        await _signInManager.SignInAsync(user, loginDto.RememberMe);
        return true;
    }


    public async Task<ServiceResponse<string>> SignUpAsync(UserSignUpDTO dto)
    {
        var response = new ServiceResponse<string>();

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            response.Success = false;
            response.Message = "Email is already in use.";
            return response;
        }

        if (!dto.AcceptTerms)
        {
            response.Success = false;
            response.Message = "You must accept the Terms and Conditions.";
            return response;
        }

        var nameParts = dto.FullName.Trim().Split(' ', 2);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        var user = new UserEntity
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            response.Success = true;
            response.Message = "User registered successfully!";
        }
        else
        {
            response.Success = false;
            response.Message = string.Join(", ", result.Errors.Select(e => e.Description));
        }

        return response;
    }
}
