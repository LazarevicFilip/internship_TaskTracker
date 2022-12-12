using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Auth
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public int Minutes { get; set; }
        public string Key { get; set; } = string.Empty;

    }
}
