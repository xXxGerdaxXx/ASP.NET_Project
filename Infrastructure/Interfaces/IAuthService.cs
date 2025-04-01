using Infrastructure.DTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(UserSignInDTO loginDto);
    Task<bool> AdminLoginAsync(AdminLoginDTO adminLoginDto);
    Task<ServiceResponse<string>> SignUpAsync(UserSignUpDTO registerDto);
}
