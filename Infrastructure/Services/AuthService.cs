//using Infrastructure.DTOs;
//using Infrastructure.Entities;
//using Infrastructure.Helpers;
//using Infrastructure.Hubs;
//using Infrastructure.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

//namespace Infrastructure.Services;

//public class AuthService(
//    UserManager<UserEntity> userManager,
//    SignInManager<UserEntity> signInManager,
//    INotificationService notificationService, IHubContext<NotificationHub> notificationHub) : IAuthService
//{
//    private readonly UserManager<UserEntity> _userManager = userManager;
//    private readonly SignInManager<UserEntity> _signInManager = signInManager;
//    private readonly INotificationService _notificationService = notificationService;
//    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

//    //public async Task<bool> LoginAsync(UserSignInDTO loginDto)
//    //{
//    //    var result = await _signInManager.PasswordSignInAsync(
//    //        loginDto.Email,
//    //        loginDto.Password,
//    //        loginDto.RememberMe,
//    //        lockoutOnFailure: false
//    //    );

//    //    return result.Succeeded;
//    //}
//    public async Task<bool> LoginAsync(UserSignInDTO loginDto)
//    {
//        var result = await _signInManager.PasswordSignInAsync(
//            loginDto.Email,
//            loginDto.Password,
//            loginDto.RememberMe,
//            lockoutOnFailure: false
//        );

//        if (result.Succeeded)
//        {
//            var user = await _userManager.FindByEmailAsync(loginDto.Email);
//            if (user != null)
//            {
//                var notificationEntity = new NotificationEntity
//                {
//                    Message = $"{user.FirstName} {user.LastName} signed in.",
//                    NotificationTypeId = 1,
//                    NotificationTargetGroupId = 1
//                };



//                await _notificationService.AddNotificationAsync(notificationEntity);
//            }
//        }

//        return result.Succeeded;
//    }

//    public async Task<bool> AdminLoginAsync(AdminLoginDTO loginDto)
//    {
//        var user = await _userManager.FindByEmailAsync(loginDto.Email);

//        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
//            return false;

//        if (!await _userManager.IsInRoleAsync(user, "Admin"))
//            return false;

//        await _signInManager.SignInAsync(user, loginDto.RememberMe);
//        return true;
//    }


//    public async Task<ServiceResponse<string>> SignUpAsync(UserSignUpDTO dto)
//    {
//        var response = new ServiceResponse<string>();

//        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
//        if (existingUser != null)
//        {
//            response.Success = false;
//            response.Message = "Email is already in use.";
//            return response;
//        }

//        if (!dto.AcceptTerms)
//        {
//            response.Success = false;
//            response.Message = "You must accept the Terms and Conditions.";
//            return response;
//        }

//        var nameParts = dto.FullName.Trim().Split(' ', 2);
//        var firstName = nameParts[0];
//        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

//        var user = new UserEntity
//        {
//            Email = dto.Email,
//            UserName = dto.Email,
//            FirstName = firstName,
//            LastName = lastName
//        };

//        var result = await _userManager.CreateAsync(user, dto.Password);

//        if (result.Succeeded)
//        {
//            response.Success = true;
//            response.Message = "User registered successfully!";
//        }
//        else
//        {
//            response.Success = false;
//            response.Message = string.Join(", ", result.Errors.Select(e => e.Description));
//        }

//        return response;
//    }
//}







using Infrastructure.DTOs;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Hubs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.Services;

public class AuthService(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager,
    INotificationService notificationService,
    IHubContext<NotificationHub> notificationHub
) : IAuthService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;



    public async Task<bool> LoginAsync(UserSignInDTO loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            loginDto.Email,
            loginDto.Password,
            loginDto.RememberMe,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user != null)
            {
                var notification = new NotificationEntity
                {
                    Message = $"{user.FirstName} {user.LastName} signed in.",
                    NotificationTypeId = 1,
                    NotificationTargetGroupId = 1
                };
                await _notificationService.AddNotificationAsync(notification);
            }
        }

        return result.Succeeded;
    }

    public async Task<bool> AdminLoginAsync(AdminLoginDTO loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return false;

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
            LastName = lastName,
            AvatarUrl = "/images/default-avatar.png"
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
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

    public async Task SignOutAsync(ClaimsPrincipal userPrincipal)
    {
        var user = await _userManager.GetUserAsync(userPrincipal);
        await _signInManager.SignOutAsync();

        if (user != null)
        {
            var notification = new NotificationEntity
            {
                Message = $"{user.FirstName} {user.LastName} signed out.",
                NotificationTypeId = 2,
                NotificationTargetGroupId = 1
            };
            await _notificationService.AddNotificationAsync(notification);
        }
    }



    public async Task<ServiceResponse<object>> ExternalLoginCallbackAsync(ExternalLoginInfo info)
    {
        var response = new ServiceResponse<object>();


        if (info == null)
        {
            response.Success = false;
            response.Message = "External login info not found.";
            return response;
        }

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true
        );

        if (result.Succeeded)
        {
            response.Success = true;
            response.Message = "Signed in successfully.";
            return response;
        }

        // User does not exist, create one
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

        var user = new UserEntity
        {
            Email = email,
            UserName = $"ext_{info.LoginProvider.ToLower()}_{email}",
            FirstName = firstName,
            LastName = lastName,
            AvatarUrl = "/images/default-avatar.png"
        };

        var identityResult = await _userManager.CreateAsync(user);
        if (!identityResult.Succeeded)
        {
            response.Success = false;
            response.Message = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return response;
        }

        await _userManager.AddToRoleAsync(user, "User");
        await _userManager.AddLoginAsync(user, info);
        await _signInManager.SignInAsync(user, isPersistent: false);

        response.Success = true;
        response.Message = "User created and signed in.";
        return response;
    }

}


