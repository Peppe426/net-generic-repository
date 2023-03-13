namespace NetGenericRepository.Interfaces;

public interface IRepository<T, Identifier> where T : BaseEntity<Identifier> //, new()
{
    Task<T> AddEntity(T input);

    Task<T?> FindEntity(Identifier id, bool includeSoftDelete = false);

    Task<T?> UpdateEntity(T input, List<Type>? enteties = null, bool deepUpdate = false);

    Task<T?> PatchEntity(T input);

    Task<T?> RemoveEntity(Identifier id);

    Task<T?> SoftDeleteEntity(T input);
}