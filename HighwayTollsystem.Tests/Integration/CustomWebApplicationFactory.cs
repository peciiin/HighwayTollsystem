using HighwayTollsystem.Data;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace HighwayTollsystem.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("toll_test_db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private DbConnection _dbConnection = null!;
        private Respawner _respawner = null!;

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
                    options.UseNpgsql(_dbContainer.GetConnectionString());
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HighwayTollContext>();
                await context.Database.EnsureCreatedAsync();
            }

            _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            await _dbConnection.OpenAsync();

            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            });
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_dbConnection);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            if (_dbConnection != null)
            {
                await _dbConnection.CloseAsync();
                await _dbConnection.DisposeAsync();
            }
            await _dbContainer.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}