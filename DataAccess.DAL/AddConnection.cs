using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL
{
    public class AddConnection 
    {
        private readonly IConfiguration _settings;
        public AddConnection(IConfiguration settings)
        {
            _settings = settings;
        }
        protected IConfiguration GetConfiguration { get; }
    }
}
