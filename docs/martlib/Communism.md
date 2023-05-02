# Communism
A distributor-worker system for collaboratively operating on a data structure.

By hmc-rae

## Versions
 - 1.0
	- First release
	
## Usage

>## abstract Distributor\<T>

A Distributor is an abstract class that can operate on a data structure containing objects of type T. When implementing, a number of methods must be completed.

Worker threads can be added by creating instances of workers.

#### Default methods

```csharp
bool IsComplete()
```
Returns true if all worker threads have completed their work on the data.

```csharp
void Synchronize()
```
Waits for all worker threads to complete their tasks, calls `complete()`, and returns control to the main line.

This method will not work if the threads are not currently working.

```csharp
void Start()
```
Prompts all worker threads to start their work on the data. 

This method will not work if the threads are currently working.

```csharp
T? GetObject()
```
Returns the next object to be operated upon.
This method is dependant on a user-defined method, detailed below.

```csharp
void Dispose()
```
Calls all worker threads to stop running, waits for them all to stop, and then destroys all workers.

#### Abstract methods

```csharp
void complete()
```
Optional behavior to be executed once all worker threads complete their tasks and synchronize with the distributor.

```csharp
T? getFromCollection(int i)
```
Override to return an item from the data structure at index 'i'. Users may assume this code runs in a single thread at once, with no resource fighting.

```csharp
bool isComplete(int i);
```
Override to signify whether or not index 'i' is a valid index - indicating whether or not there remains more work to be done.

>## abstract Worker\<T>

An abstract class representing a thread made to collaborate with other workers through a distributor. When implementing, a number of methods must be completed.

#### Default methods

```csharp
Worker(Distributor<T> client)
```
Default constructor.

Initializes a worker class and connects it to the provided Distributor, allowing it to operate on work provided by the Distributor.

#### Abstract methods

```csharp
void ProcessObject(T? obj)
```

Override to provide behavior for operating on data provided by the Distributor.

The object provided by the Distributor will always be unique - no other worker thread will be operating on it simultaneously.