namespace NetGenericRepositoryTests.Models;

public class TestAddress : BaseEntity<Guid>
{
    public TestAddress(string street, string city, string postalCode)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        ZipCode = postalCode;
    }

    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public TestCountry Country { get; set; }
    public List<TestEntity> Enteties { get; set; } = new List<TestEntity>();

    public TestAddress InintCountry(string name)
    {
        Country = new(name);
        return this;
    }

    public TestAddress AddEntity(string firstname, string lastName, int age, DateTime created = default, DateTime updated = default)
    {
        Enteties.Add(new(firstname, lastName, age, created, updated));
        return this;
    }

    private TestAddress()
    { }
}