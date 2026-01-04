using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementation.Manager
{
    public class AdminManager
    {
        public AdminManager(IConfiguration config)
        {
            Config = config;
        }

        public IConfiguration Config { get; }
    }
}
