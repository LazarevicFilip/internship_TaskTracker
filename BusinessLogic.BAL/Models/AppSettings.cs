using BusinessLogic.BAL.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Models
{
    public class AppSettings
    {
        public string ConnectionStrings { get; set; } = string.Empty;
        public JwtSettings Jwt { get; set; } = new JwtSettings();
    }
}
