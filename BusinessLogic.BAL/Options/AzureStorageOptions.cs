using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Options
{
    public class AzureStorageOptions
    {
        public string AzureBlobStorageContainer { get; set; }
        public string AzureBlobStorageConnectionString { get; set; }
    }
}
