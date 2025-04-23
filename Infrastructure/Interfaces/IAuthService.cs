using Infrastructure.DTOs;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(UserSignInDTO loginDto);
    Task<bool> AdminLoginAsync(AdminLoginDTO adminLoginDto);
    Task<ServiceResponse<string>> SignUpAsync(UserSignUpDTO registerDto);
    Task SignOutAsync(ClaimsPrincipal user);
    Task<ServiceResponse<object>> ExternalLoginCallbackAsync(ExternalLoginInfo info);
}

//public interface IAuthService
//{
//    Task<bool> LoginAsync(UserSignInDTO loginDto);
//    Task<bool> AdminLoginAsync(AdminLoginDTO adminLoginDto);
//    Task<ServiceResponse<string>> SignUpAsync(UserSignUpDTO registerDto);
//}
