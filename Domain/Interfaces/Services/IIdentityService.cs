using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Core;
using Domain.Dto.Auth;

namespace Domain.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<AutheticationResult> RegisterAsync(UserRegisterDto dto);
        Task<AutheticationResult> LoginAsync(UserLoginDto dto);
    }
}
