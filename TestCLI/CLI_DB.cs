using ClassLibrary.DB_Interaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCLI
{
    internal class CLI_DB
    {
        public void Init()
        {
            var builder = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<KnowledgeDbContext>(options =>
                    options.UseSqlServer(DB_Connection.GetConnectionString(), b => b.MigrationsAssembly("TestCLI")));
            });

            var app = builder.Build();
        }
    }
}