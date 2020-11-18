using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    //https://snede.net/you-dont-need-a-idesigntimedbcontextfactory/
    class StackGameContextFactory : IDesignTimeDbContextFactory<StackGameContext>
    {
        public StackGameContext CreateDbContext(string[] args)
        {
            var appSettingPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../ScheduledTask"));
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(appSettingPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            // Here we create the DbContextOptionsBuilder manually.        
            var builder = new DbContextOptionsBuilder<StackGameContext>();

            // Build connection string. This requires that you have a connectionstring in the appsettings.json
            var connectionString = configuration.GetConnectionString("stack-gamer");
            builder.UseSqlServer(connectionString);
            // Create our DbContext.
            return new StackGameContext(builder.Options);
        }
    }
}
