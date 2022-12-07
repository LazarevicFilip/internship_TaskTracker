using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.User
{
    public class JwtUser : IApplicationUser
    {
        public string Identity { get; set; } = string.Empty;

        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;  
}
}
