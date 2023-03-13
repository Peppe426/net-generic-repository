namespace NetGenericRepositoryTests.UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldCreateEntity()
        {
            //Given When
            TestEntity input = new("Jon", "Doe", 36);

            //Then
            input.Should().NotBeNull();
            input.Created.Date.Should().Be(DateTime.Today);
            input.Updated.Date.Should().Be(DateTime.Today);
            input.Deleted.Should().Be(null);
            input.Id.Should().Be(Guid.Empty);
            input.Firstname.Should().Be("Jon");
            input.LastName.Should().Be("Doe");
            input.Age.Should().Be(36);
        }
    }
}