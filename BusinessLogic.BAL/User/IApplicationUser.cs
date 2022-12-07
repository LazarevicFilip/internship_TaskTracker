using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.User
{
    public interface IApplicationUser
    {
        public string Identity { get; }
        public int Id { get; }
        public string Email { get; }
    }
}
