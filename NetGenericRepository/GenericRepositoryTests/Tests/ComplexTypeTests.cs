using FluentAssertions;
using GenericRepositoryTests.Models;
using GenericRepositoryTests.Models.Faker;
using Microsoft.EntityFrameworkCore;
using TestContext = GenericRepositoryTests.Models.TestContext;

namespace GenericRepositoryTests.Tests;

public class ComplexTypeTests : IntegrationTestBase
{
    private Repository<TestContext, TestComplexType, Guid> _repository = default!;
    private TestComplexType _baseComplexType;

    [SetUp]
    public async Task Setup()
    {
        _repository = new(Context);
        _baseComplexType = await InitComplexType();
    }

    [Test]
    public async Task ShouldAddEntity()
    {
        //Given
        TestComplexType input = new FakeComplexType()
            .InitComplexType()
            .IntitAddress()
            .AddPersons(5)
            .Build();

        //When
        var output = await _repository.AddEntity(input);

        //Then
        output.Name.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ShouldFindEntity()
    {
        //Given init
        //When
        var outcome = await _repository.FindEntity(_baseComplexType.Id);

        //Then
        outcome.Should().NotBeNull();
        outcome!.Name.Should().Be(_baseComplexType.Name);
    }

    [Test]
    public async Task ShouldPatchEntity()
    {
        //Given
        string expectedName = "Joe";

        //When
        var output = await _repository.FindEntity(_baseComplexType.Id);

        output!.SetName(expectedName);
        await _repository.UpdateEntity(output);

        //Then
        output.Name.Should().NotBeNullOrWhiteSpace();
        output.Name.Should().Be(expectedName);
    }

    [Test]
    public async Task ShouldDeepUpdateEntity()
    {
        //Given
        string expectedStreetName = "My Street";
        string expectedCountryname = "My Country";
        TestComplexType input = new FakeComplexType()
            .InitComplexType()
            .IntitAddress()
            .AddPersons(5)
            .Build();
        var output = await _repository.AddEntity(input);
        var enteties = new List<Type>
        {
            typeof(TestPerson),
            typeof(TestAddress),
            typeof(TestCountry)
        };

        //When
        var updatedEntity = output.ShallowCopy();
        var updatedAddress = updatedEntity!.Address!.ShallowCopy();

        updatedEntity.Address = updatedAddress;
        updatedEntity.Address!.SetStreet(expectedStreetName);
        updatedEntity.Address!.SetCountryName(expectedCountryname);

        var outcome = await _repository.UpdateEntity(updatedEntity, enteties);

        //Then
        outcome!.Address!.Street.Should().Be(expectedStreetName);
        outcome!.Address!.Country.Name.Should().Be(expectedCountryname);
    }

    [Test]
    public async Task ShouldAddPersonOnDeepUpdateEntity()
    {
        //Given
        int expectedPersonCount = 6;
        TestComplexType input = new FakeComplexType()
            .InitComplexType()
            .IntitAddress()
            .AddPersons(5)
            .Build();
        var output = await _repository.AddEntity(input);
        var enteties = new List<Type>
        {
            typeof(TestPerson),
            typeof(TestAddress),
            typeof(TestCountry)
        };

        //When
        var updatedEntity = output.ShallowCopy();

        updatedEntity.AddPerson("Joe", "Rogan", 50);

        var outcome = await _repository.UpdateEntity(updatedEntity, enteties);

        //Then
        outcome!.Address!.Persons.Should().HaveCount(expectedPersonCount);
    }

    [Test]
    public async Task ShouldUpdateEntetyInListOnDeepUpdateEntity()
    {
        //Given
        string expectedFirstName = "Joe";
        TestComplexType input = new FakeComplexType()
            .InitComplexType()
            .IntitAddress()
            .AddPersons(5)
            .Build();
        var output = await _repository.AddEntity(input);

        //When
        var updatedEntity = output.ShallowCopy();

        updatedEntity.Persons.First().SetFirstName(expectedFirstName);
        updatedEntity.Address!.Persons.First().SetFirstName(expectedFirstName);

        var outcome = await _repository.UpdateEntity(updatedEntity, null, true);

        //Then
        outcome!.Address!.Persons.First().Firstname.Should().Be(expectedFirstName);
        outcome!.Persons.First().Firstname.Should().Be(expectedFirstName);
    }

    [Test]
    public async Task ShouldThrowDbUpdateConcurrencyExceptionOnUpdateEntity()
    {
        //Given
        string expectedName = "Joe";
        string expeCtedconcurrencyErrorName = "Sofia";
        TestComplexType input = new FakeComplexType()
            .InitComplexType()
            .IntitAddress()
            .AddPersons(5)
            .Build();
        var output = await _repository.AddEntity(input);

        //When
        var clientOne = await _repository.FindEntity(output.Id);
        var clientTwo = clientOne!.ShallowCopy();

        clientOne.SetName(expectedName);
        await _repository.UpdateEntity(clientOne);

        clientTwo.SetName(expeCtedconcurrencyErrorName);

        var act = async () => await _repository.UpdateEntity(clientTwo);

        //Then
        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Test]
    public async Task ShouldRemoveEntity()
    {
        TestComplexType input = new FakeComplexType()
           .InitComplexType()
           .IntitAddress()
           .AddPersons(5)
           .Build();

        var output = await _repository.AddEntity(input);
        var outcome = await _repository.RemoveEntity(output.Id);
        var expectedResult = await _repository.FindEntity(output.Id, true);

        outcome!.Should().NotBeNull();
        outcome!.Deleted.Should().HaveDay(DateTime.UtcNow.Day);
        expectedResult.Should().BeNull();
    }

    [Test]
    public async Task ShouldSoftDeleteEntity()
    {
        TestComplexType input = new FakeComplexType()
           .InitComplexType()
           .IntitAddress()
           .AddPersons(5)
           .Build();

        var output = await _repository.AddEntity(input);
        var outcome = await _repository.SoftDeleteEntity(output);
        var expectedResult = await _repository.FindEntity(output.Id, true);

        outcome!.Should().NotBeNull();
        outcome!.Deleted.Should().HaveDay(DateTime.UtcNow.Day);
        expectedResult.Should().NotBeNull();
        expectedResult!.Deleted.Should().HaveDay(DateTime.UtcNow.Day);
    }

    private async Task<TestComplexType> InitComplexType()
    {
        TestComplexType input = new FakeComplexType()
            .InitComplexType()
            .IntitAddress()
            .AddPersons(5)
            .Build();

        var output = await _repository.AddEntity(input);
        return output;
    }
}