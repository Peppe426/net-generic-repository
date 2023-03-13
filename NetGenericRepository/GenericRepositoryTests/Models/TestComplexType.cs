namespace GenericRepositoryTests.Models;

public class TestComplexType : BaseEntity<Guid>
{
    public TestComplexType(TestAddress? address)
    {
        Address = address;
    }

    public string Name { get; set; } = string.Empty;

    public TestAddress? Address { get; set; }

    public List<TestPerson> Persons { get; set; } = new List<TestPerson>();

    public TestComplexType AddPerson(string firstname, string lastName, int age)
    {
        Persons.Add(new(firstname, lastName, age));
        return this;
    }

    public TestComplexType ShallowCopy()
    {
        return (TestComplexType)this.MemberwiseClone();
    }

    public TestComplexType SetName(string name)
    {
        Name = name;
        return this;
    }

    public TestComplexType()
    { }
}