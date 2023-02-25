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



## Resolving concurrency conflicts

<https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#resolving-concurrency-conflicts>
