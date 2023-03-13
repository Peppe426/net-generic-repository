using System.Collections;
using System.Reflection;

namespace NetGenericRepository;

public class Repository<Context, T, Identifier> : IRepository<T, Identifier> where Context : DbContext where T : BaseEntity<Identifier>
{
    private readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
    }

    public virtual async Task<T> AddEntity(T input)
    {
        EntityEntry<T> entityEntry = await _context.Set<T>().AddAsync(input);
        await _context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public virtual async Task<T?> RemoveEntity(Identifier id)
    {
        T? input = await FindEntityById(id);
        if (input == null)
        {
            return null;
        }

        var output = _context.Remove(input);
        output.Entity.Delete();
        await _context.SaveChangesAsync();
        return output.Entity;
    }

    public virtual async Task<T?> FindEntity(Identifier id, bool includeSoftDelete = false)
    {
        T? output = await FindEntityById(id, includeSoftDelete);
        if (output == null)
        {
            return null;
        }
        return output;
    }

    public virtual async Task<T?> PatchEntity(T input)
    {
        T? output = await FindEntityById(input.Id);
        if (output == null)
        {
            return null;
        }

        _context.Attach(output);

        Parallel.ForEach(input.GetType().GetProperties(), (PropertyInfo prop) =>
        {
            object? propValue = GetPropertyValue(input, prop);
            Attribute? keyAttribute = GetKeyAttribute(prop);

            if (propValue is null)
            {
                return;
            }

            if (keyAttribute != null)
            {
                return;
            }

            setNewProperyValue(prop, output, propValue);

            _context.Entry(output).Property(prop.Name).IsModified = true;
        });

        await _context.SaveChangesAsync();
        return output;
    }

    private void setNewProperyValue(PropertyInfo propertyInfo, T entity, object? newPropValue)
    {
        var outputPropValue = _context.Entry(entity).Property(propertyInfo.Name);
        outputPropValue.CurrentValue = newPropValue;
    }

    private Attribute? GetKeyAttribute(PropertyInfo prop)
    {
        return prop.GetCustomAttribute(typeof(KeyAttribute));
    }

    private object? GetPropertyValue(T input, PropertyInfo prop)
    {
        return prop.GetValue(input, prop.GetIndexParameters());
    }

    public virtual async Task<T?> SoftDeleteEntity(T input)
    {
        try
        {
            T? output = await FindEntityById(input.Id);
            if (output == null)
            {
                return null;
            }

            if (input.Rowversion != output.Rowversion)
            {
                throw new DbUpdateConcurrencyException(output.Rowversion.ToString());
            }
            input.Delete();
            _context.Entry(output).CurrentValues.SetValues(input);
            await _context.SaveChangesAsync();
            return output;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryUpdateException("Concurrency exception", ex.Message, ex.InnerException);
        }
        catch (Exception ex)
        {
            throw new Exception("Faild to delete entity", ex.InnerException);
        }
    }

    public virtual async Task<T?> UpdateEntity(T input, List<Type>? enteties = null, bool deepUpdate = false)
    {
        try
        {
            if (enteties?.Any() is true || deepUpdate is true)
            {
                var output = await DeepUpdate(input, enteties);
                await _context.SaveChangesAsync();
                return output;
            }
            else
            {
                var output = await ShallowUpdate(input);
                await _context.SaveChangesAsync();
                return output;
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryUpdateException("Concurrency exception", ex.Message, ex.InnerException);
        }
        catch (Exception ex)
        {
            throw new Exception("Faild to update entity", ex.InnerException);
        }
    }

    private async Task<dynamic> ShallowUpdate(dynamic input)
    {
        dynamic output = await _context.FindAsync(input.GetType(), input.Id);

        if (output == null)
        {
            return null;
        }

        if (input.Rowversion != output.Rowversion)
        {
            throw new DbUpdateConcurrencyException(output.Rowversion.ToString());
        }

        input.Update();
        _context.Entry(output).CurrentValues.SetValues(input);

        return output;
    }

    /// <summary>
    /// Update provided input, if enteties are provided, then only those will get updated. if deepupdate is true all enteties will get updated
    /// </summary>
    /// <param name="input">Generic entity</param>
    /// <param name="enteties">List of explicit enteties that shpuld be updated</param>
    /// <param name="deepUpdate">If true all enteties will be updated, enteties can be null</param>
    /// <returns></returns>
    private async Task<dynamic> DeepUpdate(dynamic input, List<Type>? enteties, bool deepUpdate = false)
    {
        var output = await ShallowUpdate(input);
        await UpdateProperty(input, enteties, deepUpdate);
        return output;
    }

    private async Task UpdateProperty(dynamic input, List<Type>? enteties, bool deepUpdate = false)
    {
        var properties = input.GetType().GetProperties();
        foreach (var property in properties)
        {
            var PropetyValue = input.GetType().GetProperty(property.Name).GetValue(input, null);
            if (ShouldUpdateEntity(input, property, enteties, deepUpdate) is true)
            {
                await ShallowUpdate(PropetyValue);
            }
            if (PropetyValue is IList)
            {
                foreach (var item in PropetyValue)
                {
                    await DeepUpdate(item, enteties, deepUpdate);
                }
            }
        }
    }

    private async Task<T?> FindEntityById(Identifier id, bool includeSoftDelete = false)
    {
        var output = await _context.Set<T>().FindAsync(id);

        if (output is null)
        {
            return null;
        }

        if (includeSoftDelete is false && output.Deleted != null)
        {
            return null;
        }

        return output;
    }

    private bool ShouldUpdateEntity(dynamic input, PropertyInfo prop, List<Type>? enteties, bool deepUpdate = false)
    {
        if (IsSimple(prop.PropertyType.GetTypeInfo()) is true)
        {
            return false;
        }

        dynamic? propertyValue = prop.GetValue(input, prop.GetIndexParameters());
        Attribute? isIdentifier = prop.GetCustomAttribute(typeof(KeyAttribute));
        if (propertyValue is null || isIdentifier != null)
        {
            return false;
        }

        Type propType = propertyValue.GetType();
        var entity = enteties?.FirstOrDefault(x => x.Name.ToLower() == propType.Name.ToLower());
        if (entity != null || deepUpdate is true)
        {
            return true;
        }
        return false;
    }

    private bool IsSimple(TypeInfo type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // nullable type, check if the nested type is simple.
            return IsSimple((type.GetGenericArguments()[0]).GetTypeInfo());
        }
        return type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(decimal));
    }

    private Identifier? GetIdentifierValue(Type propType, dynamic propValue)
    {
        var propInfo = propType.GetProperties().FirstOrDefault(p => p.CustomAttributes.Any(attr => attr.AttributeType == typeof(KeyAttribute)));
        Identifier? identifier = (Identifier)propInfo?.GetValue(propValue)!;
        return identifier;
    }
}

//Parallel.ForEach(input.GetType().GetProperties(), prop =>
//{
//    var propValue = prop.GetValue(input, prop.GetIndexParameters());
//    var isIdentifier = prop.GetCustomAttribute(typeof(KeyAttribute));

//    if (propValue is null)
//    {
//        return;
//    }

//    if (isIdentifier != null)
//    {
//        return;
//    }

//    var outputPropValue = _context.Entry(output).Property(prop.Name);
//    outputPropValue.CurrentValue = propValue;

//    _context.Entry(output).Property(prop.Name).IsModified = true;
//});

//input.GetType().GetProperties().ToList().ForEach((PropertyInfo prop) =>
//{
//    var propValue = prop.GetValue(input, prop.GetIndexParameters());

//    if (propValue is null)
//    {
//        return;
//    }
//    var isIdentifier = prop.GetCustomAttribute(typeof(KeyAttribute));
//    if (isIdentifier != null)
//    {
//        return;
//    }

//    var outputPropValue = _context.Entry(output).Property(prop.Name);
//    outputPropValue.CurrentValue = propValue;

//    _context.Entry(output).Property(prop.Name).IsModified = true;

//});