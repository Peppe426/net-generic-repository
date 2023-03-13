namespace NetGenericRepositoryTests.IntegrationTests.Setup;

public class TestContext : DbContext
{
    public TestContext(DbContextOptions options) : base(options)
    {
    }

    //public TestContext(DbContextOptions options) : base(options)
    //{
    //}

    //public TestContext(DbContextOptions<TestContext> options) : base(options)
    //{
    //}

    private DbSet<TestEntity> Entities { get; set; }
    //private DbSet<TestComplexType> ComplexTypes { get; set; }
    //private DbSet<TestAddress> Addresses { get; set; }
    //private DbSet<TestCountry> Contries { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    //    {
    //        var concurrencyProperty = entityType.GetProperties().FirstOrDefault(x => x.Name == "Rowversion");
    //        if (concurrencyProperty is not null)
    //        {
    //            concurrencyProperty.IsConcurrencyToken = true;
    //        }
    //    }
    //}

}

//https://github.com/dotnet/efcore/issues/19765

//https://www.infoworld.com/article/3672154/how-to-use-ef-core-as-an-in-memory-database-in-asp-net-core-6.html