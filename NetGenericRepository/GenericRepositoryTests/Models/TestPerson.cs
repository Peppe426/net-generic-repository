namespace GenericRepositoryTests.Models;

public class TestPerson : BaseEntity<Guid>
{
    public string? Firstname { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public TestPerson(string firstname, string lastName, int age, DateTime created = default, DateTime updated = default) : base(created, updated)
    {
        Firstname = firstname ?? throw new ArgumentNullException(nameof(firstname));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Age = age;
    }

    public TestPerson ShallowCopy()
    {
        return (TestPerson)this.MemberwiseClone();
    }

    public TestPerson SetFirstName(string name)
    {
        Firstname = name;
        return this;
    }

    public TestPerson()
    {
    }
}