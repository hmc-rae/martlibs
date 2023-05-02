# MonSerializer
A custom-made serialization / deserialization tool that converts objects to a byte array.

By hmc-rae.

## Features
 - Support for classes, structs, primitives, strings, and arrays
 - Inherent handling for reference semantics
 - Output to bytes instead of strings

## Version
 - 0.2
	- Arrays are now serializable, maintaining reference semantics
 - 0.1 
	- First release

## Usage

``` csharp
byte[] MonSerializer.Serialize<T>(T obj)
```
Converts a given object of type T to a byte array in MON format.
All publicly visible fields\* will be saved, unless they have the attribute `[MonSerializer.MonIgnore]`. 

If a field contains a reference to another object, it will instead contain a pointer to another position in the byte array
where that object will now be saved. Objects referenced by fields that are private or hidden by `[MonSerializer.MonIgnore]` 
will not be included in the byte array.

Structs will be stored in series - directly after the field which contains them.

```csharp
T MonSerializer.Deserialize<T>(byte[] data)
```
Converts the provided byte array into an object of type T, preserving referencing as original.

Unlike serialization, fields that have attribute `[MonSerializer.MonIgnore]` will not be ignored when reading. 
This may be changed in a future update.

Throws `FieldAccessException` if an invalid field is encountered.

```csharp
object? MonSerializer.Deserialize(byte[] data, Type type)
``` 
Converts the provided byte array into an object of the provided type, preserving referencing as original.

Unlike serialization, fields that have attribute `[MonSerializer.MonIgnore]` will not be ignored when reading. 
This may be changed in a future update.

Throws `FieldAccessException` if an invalid field is encountered.