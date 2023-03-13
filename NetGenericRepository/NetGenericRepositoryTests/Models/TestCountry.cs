namespace NetGenericRepositoryTests.Models;

public class TestCountry : BaseEntity<Guid>
{
    public TestCountry(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string Name { get; set; } = string.Empty;
}