using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Exceptions
{
    public class ConflictedActionException : Exception
    {
        public ConflictedActionException(string msg)
            :base(msg)
        {

        }
    }
}
