using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlogApp.API;

public class TestHost
{
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestStartup>();
            });
}

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // This will be configured by the test
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // This will be configured by the test
    }
} 