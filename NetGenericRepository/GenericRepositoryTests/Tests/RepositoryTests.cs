using FluentAssertions;
using GenericRepositoryTests.Models;
using Microsoft.EntityFrameworkCore;
using TestContext = GenericRepositoryTests.Models.TestContext;

namespace GenericRepositoryTests.Tests;

public class RepositoryTests : IntegrationTestBase
{
    private Repository<TestContext, TestPerson, Guid> _repository = default!;
    private TestPerson _basePerson;

    [SetUp]
    public async Task Setup()
    {
        _repository = new(Context);
        _basePerson = await InitTestPerson();
    }

    [Test]
    public async Task ShouldAddEntity()
    {
        //Given Setup
        TestPerson input = new("Jon", "Doe", 36);

        //When
        var outcome = await _repository.AddEntity(input);

        //Then
        outcome.Firstname.Should().NotBeNullOrWhiteSpace();
        outcome.LastName.Should().NotBeNullOrWhiteSpace();
        outcome.Age.Should().BeGreaterThan(0);
        outcome.Rowversion.Should().NotBeEmpty();
        outcome.Created.Should().HaveDay(DateTime.UtcNow.Day);
        outcome.Updated.Should().HaveDay(DateTime.UtcNow.Day);
        outcome.Deleted.Should().BeNull();
    }

    [Test]
    public async Task ShouldFindEntity()
    {
        //Given
        var expectedId = _basePerson.Id;

        //When
        var outcome = await _repository.FindEntity(expectedId);

        //Then
        outcome.Should().NotBeNull();
        outcome!.Firstname.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ShouldPatchEntity()
    {
        int expectedAge = 38;
        var input = await _repository.FindEntity(_basePerson.Id);
        input = input!.ShallowCopy();
        input!.Age = expectedAge;
        input.Firstname = null;
        input.LastName = string.Empty;

        var outcome = await _repository.PatchEntity(input);

        outcome.Should().NotBeNull();
        outcome!.Firstname.Should().NotBeNullOrWhiteSpace();
        outcome!.LastName.Should().BeEmpty();
        outcome!.Age.Should().Be(expectedAge);
        outcome!.Rowversion.Should().NotBeEmpty();
        outcome!.Created.Should().HaveDay(DateTime.UtcNow.Day);
        outcome!.Updated.Should().HaveDay(DateTime.UtcNow.Day);
        outcome!.Deleted.Should().BeNull();
    }

    [Test]
    public async Task ShouldUpdateEntity()
    {
        //Given
        int age = 37;
        int expectedAge = 38;
        TestPerson input = new("Jon", "Doe", age);

        var outcomeInput = await _repository.AddEntity(input);
        outcomeInput.Age = expectedAge;

        //When
        var outcome = await _repository.UpdateEntity(outcomeInput);

        //Then
        outcome.Should().NotBeNull();
        outcome!.Firstname.Should().NotBeNullOrWhiteSpace();
        outcome!.LastName.Should().NotBeNullOrWhiteSpace();
        outcome!.Age.Should().Be(expectedAge);
        outcome!.Rowversion.Should().NotBeEmpty();
        outcome!.Created.Should().HaveDay(DateTime.UtcNow.Day);
        outcome!.Updated.Should().HaveDay(DateTime.UtcNow.Day);
        outcome!.Deleted.Should().BeNull();
    }

    [Test]
    public async Task ShouldThrowDbUpdateConcurrencyExceptionOnUpdateEntity()
    {
        //Given
        int age = 37;
        int expectedAge = 38;
        string expectedFirstname = "Peter";
        TestPerson input = new("Jon", "Doe", age);
        var output = await _repository.AddEntity(input);

        //When
        TestPerson clientOne = await _repository.FindEntity(output.Id);
        var clientTwo = clientOne!.ShallowCopy();

        clientOne!.Age = expectedAge;
        await _repository.UpdateEntity(clientOne);

        clientTwo!.Firstname = expectedFirstname;

        var act = async () => await _repository.UpdateEntity(clientTwo);

        //Then
        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Test]
    public async Task ShouldRemoveEntity()
    {
        //Given
        TestPerson input = new("Jon", "Doe", 36);
        var output = await _repository.AddEntity(input);

        //When
        var outcome = await _repository.RemoveEntity(output.Id);
        outcome!.Should().NotBeNull();
        outcome!.Deleted.Should().HaveDay(DateTime.UtcNow.Day);
    }

    [Test]
    public async Task ShouldSoftDeleteEntity()
    {
        //Given
        TestPerson input = new("Jon", "Doe", 36);
        var output = await _repository.AddEntity(input);

        //When
        var outcome = await _repository.SoftDeleteEntity(output);

        //Then
        outcome!.Should().NotBeNull();
        outcome!.Deleted.Should().HaveDay(DateTime.UtcNow.Day);
    }

    private async Task<TestPerson> InitTestPerson(Guid expectedId = default)
    {
        TestPerson input = new("Jon", "Doe", 36);
        if (expectedId != Guid.Empty)
        {
            input.SetId(expectedId!);
        }
        var output = await _repository.AddEntity(input);
        return output;
    }
}