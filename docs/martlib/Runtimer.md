# Runtimer
A class dedicated to tracking time in floats. Useful for framerate management in games.

By hmc-rae

## Features
 - DeltaTime
 - Easy start, stop, restart, and wait for time to elapse
 
## Version
 - 1.1
	- LastFrameRate, a measure of the estimated framerate as per last frame performance, added
	- FrameRate can be accessed & changed.
 - 1.0
	- First release

## Usage

```csharp
Runtimer(long targetMS)
```
Creates a new instance of a Runtimer, with a provided target milliseconds to wait.

```csharp
Runtimer(float frameRate)
```
Creates a new instance of a RunTimer, which aims to run at the provided framerate.

```csharp
void Wait()
```
Waits until the target milliseconds have elapsed.

```csharp
void Restart()
```
Restarts the timer and calculates the DeltaTime value.

```csharp 
void Stop()
```
Stops the timer and calculates the DeltaTime value.

```csharp
void Start()
```
Starts the timer.

### Fields
```csharp
double DeltaTime
```
The difference in time from the last end to the current end, in a double. For example, 1 second is 1.0.

```csharp
double lastFrameRate
```
The projected frame rate as of the last interval.

```csharp
float FrameRate
```
The target frame rate.