using Bogus;
using Bogus.DataSets;

namespace NetGenericRepositoryTests.IntegrationTests.Faker;

public class FakeAddress
{
    private const string _locale = "sv";
    protected TestAddress TestAddress;
    protected List<TestAddress> TestAddresses;

    private Faker<TestAddress> FakerAddress =
    new Faker<TestAddress>(_locale)
    //.StrictMode(true)
    .RuleFor(p => p.City, s => s.Address.City())
    .RuleFor(p => p.Street, s => s.Address.StreetAddress())
    .RuleFor(p => p.ZipCode, s => s.Address.ZipCode());

    public FakeAddress InitAddress()
    {
        TestAddress = FakerAddress.Generate();
        return this;
    }

    public FakeAddress InitAddresses(int count)
    {
        TestAddresses = FakerAddress.Generate(count);
        return this;
    }

    public FakeAddress SetCountry()
    {
        var fakeAddress = new Bogus.DataSets.Address(_locale);
        TestAddress.InintCountry(fakeAddress.Country());
        return this;
    }

    public FakeAddress AddEneties(int count = 1)
    {
        for (int i = count; i < count; i++)
        {
            var fakePerson = new Name(_locale);
            int age = new Random().Next(5, 100);
            TestAddress.AddEntity(fakePerson.FindName(), fakePerson.LastName(), age);
        }

        return this;
    }

    public TestAddress Build() => TestAddress;

    public List<TestAddress> BuildMissions() => TestAddresses;
}