using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace NetGenericRepositoryTests.IntegrationTests.Setup;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            //config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json"));
        });

        builder.ConfigureServices((context, services) =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestContext>();

            dbContext.Database.EnsureCreated();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.OpenAsync().GetAwaiter().GetResult();

            //var appSettingsSection = context.Configuration.GetSection(AppSettings.SectionName);
            //services.Configure<AppSettings>(appSettingsSection);
            //AppSettings = appSettingsSection.Get<AppSettings>();
        });
    }

    //protected override void ConfigureWebHost(IWebHostBuilder builder)
    //{
    //    builder.UseEnvironment("tests");

    //    builder.ConfigureServices(services =>
    //    {
    //        CleanupServiceCollection(services);

    //        InitDatabase(services);
    //    });

    //    //builder.UseEnvironment("Development");
    //}

    private void InitDatabase(IServiceCollection services)
    {
        services.AddSingleton<DbConnection>(container =>
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            return _connection;
        });

        services.AddDbContext<TestContext>((container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);
        });
    }

    private void CleanupServiceCollection(IServiceCollection services)
    {
        var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TestContext>));
        services.Remove(dbContextDescriptor);

        var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));

        services.Remove(dbConnectionDescriptor);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Close();
    }
}

public partial class Program
{
}