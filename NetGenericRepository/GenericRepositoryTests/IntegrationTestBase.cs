using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TestContext = GenericRepositoryTests.Models.TestContext;

namespace GenericRepositoryTests;

public abstract class IntegrationTestBase : IDisposable
{
    protected TestContext Context = default!;

    [SetUp]
    public void Init()
    {
        InitDatabase();
    }

    //https://codeopinion.com/testing-with-ef-core/
    private void InitDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestContext>()
        .UseSqlite(connection)
            .Options;

        try
        {
            Context = new TestContext(options);
            Context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}