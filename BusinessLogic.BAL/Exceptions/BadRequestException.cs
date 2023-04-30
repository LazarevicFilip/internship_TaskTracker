using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string msg)
            : base(msg)
        {

        }
    }
}
