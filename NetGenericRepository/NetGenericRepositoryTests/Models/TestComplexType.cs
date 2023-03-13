namespace NetGenericRepositoryTests.Models;

public class TestComplexType : BaseEntity<Guid>
{
    public TestComplexType(TestAddress? address, List<TestEntity> testEnteties)
    {
        Address = address;
        TestEnteties = testEnteties ?? throw new ArgumentNullException(nameof(testEnteties));
    }

    public TestAddress? Address { get; set; }
    public List<TestEntity> TestEnteties { get; set; } = new List<TestEntity>();

    private TestComplexType()
    { }
}