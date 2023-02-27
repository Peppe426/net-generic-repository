# net-generic-repository

As a developer, when tasked with implementing an API, you often find yourself repeatedly performing the same actions to implement CRUD operations. This repository is designed to simplify the process by providing a generic interface that can be used to handle creating, reading, updating, and deleting entities using Entity Framework Core.



## More about C# generics
C# generics is a feature of the C# programming language that allows you to create classes, interfaces, methods, and delegates that can work with any data type. This allows for greater code reusability and type safety by eliminating the need for redundant code that performs the same operations on different data types. With generics, you can define a class or method once, and use it with multiple data types without having to rewrite it for each type. Generics in C# are parameterized types, meaning that you can specify the type of data that the generic class or method should work with at runtime. This makes C# generics a powerful tool for creating flexible and efficient code that can handle a variety of data types.

Read more at: [Generic classes and methods in C#](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics)

## How to use net-generic-repository
https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.add?view=efcore-7.0
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
### Concurrency conflicts in action:

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