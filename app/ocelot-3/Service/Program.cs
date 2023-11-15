using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // Generate ocelot configuration file with multiple json file
                    // Setting ocelot service with hostingContext and config object.
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        // Setting host server setting and default ocelot configuration
                        config
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true)
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                            .AddJsonFile("ocelot.json")
                            .AddEnvironmentVariables();
                        // Merge ocelot configuration come from path "/app/ocelot", and replace local ocelot.json.
                        config.AddOcelot("/app/ocelot", hostingContext.HostingEnvironment);
                    });
                });
    }
}
