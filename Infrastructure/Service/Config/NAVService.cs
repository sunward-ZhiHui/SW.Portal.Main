using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.Config
{
    public class NAVService
    {
        private readonly IConfiguration _config;
        public NAVService(IConfiguration config, string company)
        {
            _config = config;
            Context = new NAV.NAV(new Uri($"{_config[company + ":OdataUrl"]}/Company('{_config[company + ":Company"]}')"))
            {
                Credentials = new NetworkCredential(_config[company + ":UserName"], _config[company + ":Password"])
            };
        } 
        public NAV.NAV Context { get; private set; }
    }
}
