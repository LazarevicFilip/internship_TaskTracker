﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Exceptions
{
    public class ForbiddenActionException : Exception
    {
        public ForbiddenActionException(string useCase, string user)
            : base($"User {user} has tried to execute {useCase} without having premission.")
        {
            
        }
    }
}
