using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL.Logging
{
    public class ConsoleLogger : ILoggingService
    {

        private readonly ILogger _logger;

        public ConsoleLogger(ILogger<ConsoleLogger> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string message, params object[] parameters)
        {
          _logger.LogError(ex, message, parameters);
        }

        public void LogInformation(string message, params object[] parameters)
        {
            _logger.LogInformation(message, parameters);
        }
    }
}
