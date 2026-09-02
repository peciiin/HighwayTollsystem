using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HighwayTollsystem.Data;
using Microsoft.EntityFrameworkCore;
using HighwayTollsystem.Services;
using Microsoft.Extensions.DependencyInjection;
using HighwayTollsystem.Models;

namespace HighwayTollsystem.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<HighwayTollContext>));
                if (dbDescriptor != null)
                {
                    services.Remove(dbDescriptor);
                }

                var simulatorDescriptor = services.SingleOrDefault(
                    d => d.ImplementationType == typeof(TollSimulatorService));
                if (simulatorDescriptor != null)
                {
                    services.Remove(simulatorDescriptor);
                }

                services.AddDbContext<HighwayTollContext>(options =>
                {
                    options.UseInMemoryDatabase("HighwayTollIntegrationTestDb");
                });
            });
        }
    }
}
