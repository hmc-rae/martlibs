# MonSerializer
A custom-made serialization / deserialization tool that converts objects to a byte array.

By hmc-rae.

## Features
 - Support for classes, structs, primitives, strings, and arrays
 - Inherent handling for reference semantics
 - Output to bytes instead of strings

## Version
 - 0.3
	- All fields are now visible to MonSerializer. Fields that are not public are ignored by default, but can be included with `[MonSerializer.MonInclude]`.
	Fields that are public are included by default, but can be ignored with `[MonSerializer.MonIgnore]`.
 - 0.2
	- Arrays are now serializable, maintaining reference semantics
		- 0.2.1
			- Could not previously serialize directly into an array due to an oversight. Patched.
		- 0.2.2
			- Added overload for Deserialize to read from a file to deserialize.
 - 0.1 
	- First release

## Usage
### Serialization
``` csharp
byte[] MonSerializer.Serialize<T>(T obj)
```
Converts a given object of type T to a byte array in MON format.
All publicly visible fields\* will be saved, unless they have the attribute `[MonSerializer.MonIgnore]`. 
Fields that are non-public by default can be included with the attribute `[MonSerializer.MonInclude]`.


If a field contains a reference to another object, it will instead contain a pointer to another position in the byte array
where that object will now be saved. Objects referenced by fields that are non-public or otherwise hidden by `[MonSerializer.MonIgnore]` 
will not be included in the byte array.

Structs will be stored in series - directly after the field which contains them.
### Deserialization
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

### Deserialization from file

```csharp
T MonSerializer.Deserialize<T>(string filename)
```
Reads the given file into a byte array, and deserializes it into an object of type T.

Unlike serialization, fields that have attribute `[MonSerializer.MonIgnore]` will not be ignored when reading. 
This may be changed in a future update.

Throws `FieldAccessException` if an invalid field is encountered.

Throws `FileNotFoundException` if an invalid filename is passed.

```csharp
object? MonSerializer.Deserialize(string filename, Type type)
``` 
Reads the given file into a byte array, and deserializes it into an object of the given type.

Unlike serialization, fields that have attribute `[MonSerializer.MonIgnore]` will not be ignored when reading. 
This may be changed in a future update.

Throws `FieldAccessException` if an invalid field is encountered.

Throws `FileNotFoundException` if an invalid filename is passed.

## Notes

\* Some fields may not be serializable at this point - namely, `System.Type` proves difficult due to the runtime nature of the type. 
This may be solved in the future, but at this point, it is recommended to attach `[MonSerializer.MonIgnore]` labels to any field that is not a:
 - struct
 - class 
 - array
 - primitive