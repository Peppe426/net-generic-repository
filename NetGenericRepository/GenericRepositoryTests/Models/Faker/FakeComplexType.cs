using Bogus;
using Bogus.DataSets;

namespace GenericRepositoryTests.Models.Faker;

public class FakeComplexType
{
    private const string _locale = "sv";
    protected TestComplexType TestComplexType;
    protected List<TestComplexType> TestComplexTypes;

    private Faker<TestComplexType> FakerComplexType = new Faker<TestComplexType>()
        .RuleFor(p => p.Name, s => s.Company.CompanyName());

    public FakeComplexType InitComplexType()
    {
        TestComplexType = FakerComplexType.Generate();
        return this;
    }

    public FakeComplexType IntitAddress()
    {
        var testAddress = new FakeAddress().InitAddress().AddPersons(5).SetCountry().Build();
        TestComplexType.Address = testAddress;
        return this;
    }

    public FakeComplexType AddPersons(int count)
    {
        for (int i = 0; i <= count; i++)
        {
            var fakePerson = new Name(_locale);
            int age = new Random().Next(5, 100);
            TestComplexType.AddPerson(fakePerson.FirstName(), fakePerson.LastName(), age);
        }
        return this;
    }

    public TestComplexType Build() => TestComplexType;

    public List<TestComplexType> BuildMissions() => TestComplexTypes;

    public FakeComplexType()
    {
    }
}