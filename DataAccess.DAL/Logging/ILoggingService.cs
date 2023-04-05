using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL.Logging
{
    public interface ILoggingService
    {
        void LogInformation(string message, params object[] parameters);
        void LogError(Exception ex, string message, params object[] parameters);
    }
}
