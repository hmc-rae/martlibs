# Pool
A data structure capable of readily inserting & removing objects up to a maximum depth.

By hmc-rae

## Version
 - 1.0
	- First release

## Usage

### Methods
```csharp
Pool<T>(uint capacity)
```
Creates a new pool to store objects of type T with a given max capacity.

```csharp
T? Get(int i)
```
Returns the object stored at index 'i' in the pool, if it exists - else, return the default form of T.

```csharp
void Add(T item)
```
Appends the given item to the pool.

Throws `ObjectPoolOverflowException` if the pool is at maximum capacity.

```csharp
void Insert(int i, T item)
```
Inserts the given item to the pool at the given index.

Throws `IndexOutOfRangeException` if the given index is invalid.

```csharp
void Remove(int i)
```
Removes the item at the given index in the pool.
The rest of the pool will be adjusted in an unordered manner to reduce the occupied size by 1.


Throws `IndexOutOfRangeException` if the given index is invalid.

```csharp
void Flush()
```
Resets the internal occupied size of the pool to 0, effectively clearing it.

Note that this function does not remove the references stored in the pool, preventing memory from being cleared.
To remove these references, use `PurgeUnused()` in conjunction with `Flush()`.

```csharp
void PurgeUnused()
```
Clears out all unused elements in the pool, ensuring that everything not currently 'in the pool' is removed.

### Fields

```csharp 
uint OccupiedSize
```
The number of items currently 'in the pool'. All indicies up to this value are valid and addressable.

```csharp
uint MaxSize
```
The maximum capacity of the pool. This limit cannot be exceeded.