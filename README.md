[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_System.Runtime.Serialization&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=nanoframework_System.Runtime.Serialization) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_System.Runtime.Serialization&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=nanoframework_System.Runtime.Serialization) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.System.Runtime.Serialization.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Runtime.Serialization/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

# Welcome to the .NET **nanoFramework** System.Runtime.Serialization repository

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.System.Runtime.Serialization | [![Build Status](https://dev.azure.com/nanoframework/System.Runtime.Serialization/_apis/build/status/nanoframework.System.Runtime.Serialization?branchName=main)](https://dev.azure.com/nanoframework/System.Runtime.Serialization/_build/latest?definitionId=101&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.System.Runtime.Serialization.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Runtime.Serialization/) |

## Usage

The BinaryFormatter class allow to serialize and deserialize to/from binary format. This allows a compact storage and, because it's implemented in native code, faster execution for serialization and deserialization operations.

The only requirement is to decorate a class or it's fields with the `[Serializable]` attribute.
Other attributes are available to provide hints to the serialization engine so the serialization data it's reduced as much as possible. More on this on the next section.

> **Warning** the implementation of binary serialization for .NET **nanoFramework** is **NOT** compatible with the one of .NET Framework or .NET Core, menaning that it's not possible to use this to exchange data between the two frameworks.

### Serializing a class

Follows a `Person` class that will be used in the following examples.

```csharp
[Serializable]
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public DateTime Birthday { get; set; }
    public int ID { get; set; }
    public string[] ArrayProperty { get; set; }
    public Person Friend { get; set; }
}
```

To serialize it, simply call the method `Serialize()` and pass the class instance. Like this:

```csharp
var nestedTestClass = new Person()
{
    FirstName = "John",
    LastName = "Doe",
    Birthday = new DateTime(1988, 4, 23),
    ID = 2700,
    Address = null,
    ArrayProperty = new string[] { "hello", "world" },
    Friend = new Person()
    {
        FirstName = "Bob",
        LastName = "Smith",
        Birthday = new DateTime(1983, 7, 3),
        ID = 2000,
        Address = "123 Some St",
        ArrayProperty = new string[] { "hi", "planet" },
    }
};

byte[] serializedPerson = BinaryFormatter.Serialize(nestedTestClass);
```

The `serializedPerson` byte array contains the binary representation for the `nestedTestClass`.

### Deserializing a class

In order to deserialize a class from it's binary representation to an instance of the class, call the `Deserialize()` method and pass the binary representation.

```csharp
[Serializable]

Person anotherPerson = (Person)BinaryFormatter.Deserialize(serializedPerson);
```

### Serialization hints

There are optional attributes that can be used to provide hints to the serialization engine so the serialization data it's reduced as much as possible.
Taking the `Person` class, follows the optimizations that are possible with the above example:

```csharp
[Serializable]
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public DateTime Birthday { get; set; }
    [SerializationHints(RangeBias = 2000)]
    public int ID { get; set; }
    [SerializationHints(ArraySize = 2)]
    public string[] ArrayProperty { get; set; }
    public Person Friend { get; set; }
}
```

The `SerializationHints` has several options to improve the data packing.
Looking at the `ID` property above, which is of `int` type, without any optimization it takes 32bits to store. Now, this is used to store an ID which, let's assume for the sake of the example that only store IDs bigger than 2000. 
Using the RangeBias with a value of `2000` that value will be subtracted to the value being stored. 
In the code above, the ID with value 2700, would be serialized as (2700 - 2700 = 700) which can be stored as 16bits value instead of the 32bits that it would initially take.

Another serialization hint is the array size. For the `ArrayProperty` let's assume that it will be always contain 2 elements. 
Decorating it with `ArraySize` and the size of the array, will store that information as part of the serialization data thus saving space that otherwise would be wasted with a generic count for the size of the array. 

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
