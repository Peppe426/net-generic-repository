namespace NetGenericRepositoryTests.Models;

public class TestEntity : BaseEntity<Guid>
{
    public TestEntity(string firstname, string lastName, int age, DateTime created = default, DateTime updated = default) : base(created, updated)
    {
        Firstname = firstname ?? throw new ArgumentNullException(nameof(firstname));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Age = age;
    }

    public string? Firstname { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public TestEntity ShallowCopy()
    {
        return (TestEntity)this.MemberwiseClone();
    }
}