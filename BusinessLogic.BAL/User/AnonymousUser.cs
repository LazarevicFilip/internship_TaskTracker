using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.User
{
    public class AnonymousUser : IApplicationUser
    {
        public string Identity => "anonymous";

        public int Id => 0;

        public string Email => "anonymoys@asp.com";
    }
}
