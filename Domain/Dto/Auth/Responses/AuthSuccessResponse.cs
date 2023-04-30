using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.Auth.Responses
{
    public class AuthSuccessResponse
    {
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
