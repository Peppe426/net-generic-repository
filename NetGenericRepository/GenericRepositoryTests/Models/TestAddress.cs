namespace GenericRepositoryTests.Models;

public class TestAddress : BaseEntity<Guid>
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public TestCountry Country { get; set; }
    public List<TestPerson> Persons { get; set; } = new List<TestPerson>();

    public TestAddress(string street, string city, string postalCode)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        ZipCode = postalCode;
    }

    public TestAddress InitCountry(string name)
    {
        Country = new(name);
        return this;
    }

    public TestAddress AddEntity(string firstname, string lastName, int age, DateTime created = default, DateTime updated = default)
    {
        Persons.Add(new(firstname, lastName, age, created, updated));
        return this;
    }

    public TestAddress SetStreet(string street)
    {
        Street = street;
        return this;
    }

    public TestAddress SetCountryName(string name)
    {
        Country.SetName(name);
        return this;
    }

    public TestAddress ShallowCopy()
    {
        return (TestAddress)this.MemberwiseClone();
    }

    public TestAddress()
    { }
}