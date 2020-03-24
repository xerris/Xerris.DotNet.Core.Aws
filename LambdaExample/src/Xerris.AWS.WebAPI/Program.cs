using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Xerris.AWS.WebAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                Environment.Exit(1);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>()
                .UseSerilog();
        }
    }
}