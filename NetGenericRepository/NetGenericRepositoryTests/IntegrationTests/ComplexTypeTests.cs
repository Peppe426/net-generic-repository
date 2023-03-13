using NetGenericRepositoryTests.IntegrationTests.Setup;
using System.Numerics;
using TestContext = NetGenericRepositoryTests.IntegrationTests.Setup.TestContext;

namespace NetGenericRepositoryTests.IntegrationTests;

public class ComplexTypeTests : IntegrationTestsBase
{
    private Repository<TestContext, TestComplexType, Guid> _repository = default!;
    private TestComplexType _baseEntity;

    [SetUp]
    public async Task Setup()
    {
        _repository = new(Context);
        _baseEntity = await InitBaseEntity();
    }

    [Test]
    public async Task ShouldAddEntity()
    {
        //Given init

        //When
        var output = _repository.AddEntity(_baseEntity);

        //Then

        output.Should().NotBeNull();
    }

    [Test]
    public async Task ShouldFindEntity()
    {
        //Given
        var expectedId = _baseEntity.Id;

        //When
        var outcome = await _repository.FindEntity(expectedId);

        //Then
        outcome.Should().NotBeNull();
    }

    [Test]
    public async Task ShouldPatchEntity()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ShouldUpdateEntity()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ShouldRemoveEntity()
    {
        Assert.Fail(); //Cascade support
    }

    [Test]
    public async Task ShouldSoftDeleteEntity()
    {
        Assert.Fail(); //cascade
    }

    private async Task<TestComplexType> InitBaseEntity(BigInteger expectedId = default(BigInteger))
    {
        TestAddress testAddress = new("Drottning gatan", "Stockholm", "16847");
        testAddress.InintCountry("Sweden");
        testAddress.AddEntity("Jon", "Doe", 36);
        testAddress.AddEntity("Peter", "Higs", 36);

        TestComplexType input = new TestComplexType(testAddress,
            new List<TestEntity>()
                {
                new("Jon", "Doe", 36),
                new("Peter", "Higs", 36)
                }
            );

        var output = await _repository.AddEntity(input);
        return output;
    }
}