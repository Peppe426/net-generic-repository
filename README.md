# net-generic-repository

When developers are assigned to implement an API, they often encounter repetitive tasks to implement CRUD operations. To simplify this process, this repository offers a generic interface that leverages Entity Framework Core to handle entity creation, reading, updating, and deletion. Furthermore Shallow and deep update is supported. Soft delete on base entity is supported.

## More about C# generics

C# generics is a feature of the C# programming language that allows you to create classes, interfaces, methods, and delegates that can work with any data type. This allows for greater code reusability and type safety by eliminating the need for redundant code that performs the same operations on different data types. With generics, you can define a class or method once, and use it with multiple data types without having to rewrite it for each type. Generics in C# are parameterized types, meaning that you can specify the type of data that the generic class or method should work with at runtime. This makes C# generics a powerful tool for creating flexible and efficient code that can handle a variety of data types.

Read more at: [Generic classes and methods in C#](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics)

## How to use net-generic-repository

<https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.add?view=efcore-7.0>
Create / Add Supported

Read / Find Supported

Update / Update Supported

Delete / Delete Supported

## Concurrency conflicts

This repository supports application-managed optimistic concurrency using Entity frameworks [ConcurrencyCheck](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#application-managed-concurrency-tokens)
using data annotations:

```
public class BaseEntity<Identifier>
{
 [ConcurrencyCheck]
 public Guid Version { get; set; }
}
```

### Concurrency conflicts in action

1. The version is set when an entity is initialized.
2. Whenever the entity is updated or deleted, the version is also updated.
3. Concurrency conflicts arise when the version of the input does not match the version in the database.
4. If this condition is not met, a DbUpdateConcurrencyException is thrown.

```
if (input.Rowversion != output.Rowversion)
{
 throw new DbUpdateConcurrencyException(output.Rowversion.ToString());
}
```

The error is caught and thrown as a domain error, which includes the Rowversion for easier handling of client-side concurrency:

```
public class RepositoryUpdateException : DbUpdateConcurrencyException
{
    public RepositoryUpdateException(string message, string rowVersion, Exception? innerException) : base(message, innerException)
    {
        Rowversion = rowVersion;
    }

    public string Rowversion { get; set; } = string.Empty;
}
```

### More about Concurrency conflicts

Concurrency conflicts occur when multiple users or processes try to access the same resource or data simultaneously. These conflicts can cause data inconsistency and errors in an application. To resolve concurrency conflicts, you can use various techniques such as:

* Locking: Locking is a technique used to ensure that only one user or process can access a resource or data at a time. This technique can prevent concurrency conflicts by blocking other users or processes from accessing the same resource until the first user or process has finished its task.

* Versioning: Versioning is a technique that involves maintaining multiple versions of the same data. When a user or process updates the data, a new version is created, and the old version is retained. This technique can prevent concurrency conflicts by allowing multiple users or processes to access and update different versions of the same data simultaneously.

* Optimistic concurrency control: Optimistic concurrency control is a technique that allows multiple users or processes to access and update the same data simultaneously. However, before committing the changes, the system checks whether any other user or process has updated the data in the meantime. If there is a conflict, the system notifies the user or process to resolve the conflict.

* Pessimistic concurrency control: Pessimistic concurrency control is a technique that locks the resource or data as soon as a user or process accesses it. This technique can prevent concurrency conflicts, but it can also reduce system performance by creating a bottleneck.

* Conflict resolution algorithms: Conflict resolution algorithms are used to automatically resolve conflicts between multiple users or processes. These algorithms can compare the conflicting changes and determine the most appropriate solution to resolve the conflict.

Read more at: [Resolving concurrency conflicts](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#resolving-concurrency-conflicts)

### Shallow and deep update

Shallow and deep update are two different approaches to updating data in a database or other data storage system.

A shallow update only modifies the top-level fields of an object or record, without changing any of its nested fields. This approach is faster and less resource-intensive than a deep update, but it can lead to data inconsistencies if the nested fields have changed since the last update.

On the other hand, a deep update modifies both the top-level fields and any nested fields of an object or record. This approach is more thorough and ensures data consistency, but it can be slower and require more resources than a shallow update.

In summary, a shallow update only updates the top-level fields of an object or record, while a deep update updates both the top-level fields and any nested fields. The choice between these two approaches depends on the specific requirements of the data storage system and the use case for updating the data.

### soft delete

Soft delete is a technique used in database management systems to mark a record as deleted without actually removing it from the database. Instead of deleting the record permanently, the system sets a flag on the record indicating that it has been deleted.

This approach offers several benefits. Soft delete enables the recovery of deleted records if necessary, allows for the auditing of data changes, and maintains referential integrity by keeping related records intact. It also helps to ensure that data is not lost accidentally or maliciously.

Soft delete is commonly used in situations where data needs to be retained for compliance, auditing, or historical purposes. It can also be useful in scenarios where data needs to be restored after accidental or erroneous deletion.

In summary, soft delete is a database management technique that marks a record as deleted without actually removing it from the database. This approach helps to maintain data integrity, recover deleted records, and audit data changes.
