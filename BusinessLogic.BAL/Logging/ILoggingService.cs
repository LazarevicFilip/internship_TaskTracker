using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Logging
{
    public interface ILoggingService
    {
        void LogInforamtion(string message, params object[] parameters);
        void LogError(Exception ex, string message, params object[] parameters);
    }
}
