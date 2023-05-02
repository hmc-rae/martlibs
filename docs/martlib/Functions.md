# Functions
A series of general-purpose functions I find myself using a lot. Mostly boilerplate code.

By hmc-rae.

## Features
 - File searching utilities
 - Byte array manipulation
 
## Versions
 - 1.0
	- First release
		- 1.0.1
			- Added Byte & SByte read capability

## Usage

### File seek
```csharp
void Functions.Seek(string directory, Func<string, int> action)
```
Recursively checks all files in a given directory, considering all subfolders. For each file (not folder), an action is performed on the file.

The file name is passed to the function `action` as the only parameter, and anticipates a return value of int. Currently, return value is not used.

### Array manipulation
```csharp
T[] Functions.BitReaders.Double<T>(T[] list)
```
Given an array of type T, doubles the size of the array and reinserts all values.

### Data writing

```csharp
byte[] Functions.BitReaders.Write(byte[] output, object? value, ref ulong position)
```
Writes the byte data of a given value to the byte array `output`. If the byte data will not fit into `output`, `output` will be doubled and the data added.

While there is the default function that accepts `object?` as the value, overloads exist for the following primitive types:
 - long
 - ulong
 - int
 - uint
 - short
 - ushort
 - byte
 - sbyte
 - float
 - double
 - bool		(0 or 255)
 - string	(null-terminated)

The resultant byte array is returned (in event of the array doubling in size). The parameter `position` is updated to point to the 'end' of the byte array after writing the value.

### Data reading
```csharp
void Functions.BitReaders.Read(byte[] data, ref ulong position, out ? output)
```
Reads data from a byte array, starting at `position`, into the field `output` by reference. 
The referenced parameter `position` will be updated to point to the 'end' of the byte array after reading.

The following types are supported:
 - long
 - ulong
 - int
 - uint
 - short
 - ushort
 - byte
 - sbyte
 - float
 - double
 - bool		(0 or 255)
 - string	(null-terminated)

```csharp
? Functions.BitReaders.Read(byte[] data, ref ulong position, ? output)
```
Reads data from a byte array, starting at `position`, and returns the value. 
The referenced parameter `position` will be updated to point to the 'end' of the byte array after reading.

The following types are supported:
 - long
 - ulong
 - int
 - uint
 - short
 - ushort 
 - byte
 - sbyte
 - float
 - double
 - bool		(0 or 255)
 - string	(null-terminated)