using Microsoft.EntityFrameworkCore;

namespace GenericRepositoryTests.Models;

public class TestContext : DbContext
{
    public TestContext(DbContextOptions options) : base(options)
    {
    }

    private DbSet<TestPerson> Persons { get; set; }
    private DbSet<TestAddress> Addresses { get; set; }
    private DbSet<TestCountry> Countries { get; set; }
    private DbSet<TestComplexType> ComplexTypes { get; set; }
}