namespace NetGenericRepository;

public class BaseEntity<Identifier>
{
    public BaseEntity(DateTime created = default, DateTime updated = default)
    {
        Created = ConfirmDate(created);
        Updated = ConfirmDate(updated);
        Rowversion = Guid.NewGuid();
    }

    [Key]
    public Identifier Id { get; set; } = default(Identifier)!;

    [ConcurrencyCheck]
    public Guid Rowversion { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime? Deleted { get; set; }

    private DateTime ConfirmDate(DateTime dateTime)
    {
        if (dateTime == default(DateTime))
        {
            return DateTime.UtcNow;
        }
        return dateTime!;
    }

    private DateTime? ConfirmDate(DateTime? dateTime)
    {
        if (dateTime is null)
        {
            return null;
        }
        return dateTime!;
    }

    public BaseEntity<Identifier> Update()
    {
        this.Updated = DateTime.UtcNow;
        this.Rowversion = Guid.NewGuid();

        return this;
    }

    public BaseEntity<Identifier> Delete()
    {
        this.Deleted = DateTime.UtcNow;
        this.Rowversion = Guid.NewGuid();

        return this;
    }

    public BaseEntity<Identifier> SetId(Identifier id)
    {
        this.Id = id;
        return this;
    }
}