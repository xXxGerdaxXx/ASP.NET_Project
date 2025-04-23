using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs;

public class UserProfileUpdateDTO
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public IFormFile? Avatar { get; set; }
}
