using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PatientsService.NotificationService
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
                    webBuilder.UseSerilog((context, loggerConfig) =>
                    {
                        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
                        telemetryConfiguration.InstrumentationKey = context.Configuration["ApplicationInsights:InstrumentationKey"];

                        loggerConfig.ReadFrom.Configuration(context.Configuration);
                        loggerConfig.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
                        // always log to console (docker env)
                        loggerConfig.WriteTo.Console();

                        loggerConfig.WriteTo.File("log_.txt", rollingInterval: RollingInterval.Day);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
