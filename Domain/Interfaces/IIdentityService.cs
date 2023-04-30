using Domain.Core;
using Domain.Dto.Auth;
using Domain.Dto.V1.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IIdentityService
    {
        Task<AutheticationResult> RegisterAsync(UserRegisterDto dto);
        Task<AutheticationResult> LoginAsync(UserLoginDto dto);
        Task<AutheticationResult> RefreshTokenAsync(RefreshTokenRequest dto);
    }
}
