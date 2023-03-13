using Microsoft.Data.Sqlite;

namespace NetGenericRepositoryTests.IntegrationTests.Setup;

public abstract class IntegrationTestsBase : IDisposable
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
        //Dispose(true);
        GC.SuppressFinalize(this);
    }
}