using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
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
                
                var builder = WebApplication.CreateBuilder(args);
                var startup = new Startup(builder.Configuration);
                var app = builder.Build();
                startup.Configure(app, builder.Environment);

                app.Run();
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
    }
}