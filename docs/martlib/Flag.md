# Flag
An object representing 64 unique boolean flags, separately accessible.

By hmc-rae

## Version
 - 1.0
	- First release
		- 1.0.1
			- Patched `Has()` - would not properly compare two objects.

## Usage

### Constructors
```csharp
FlagStruct(uint initial)
```
Creates a new FlagStruct with the flag string equal to the integer value provided.

```csharp
FlagStruct(ulong initial = 0)
```
Creates a new FlagStruct with the flag string equal to the long value provided. Defaults to 0.

```csharp
FlagStruct(object obj)
```
Creates a new FlagStruct with values corresponding to the object passed, provided that the object is a enum.