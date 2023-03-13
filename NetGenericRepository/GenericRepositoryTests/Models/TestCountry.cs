namespace GenericRepositoryTests.Models;

public class TestCountry : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;

    public TestCountry(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public TestCountry SetName(string name)
    {
        Name = name;
        return this;
    }

    protected TestCountry()
    {
    }
}