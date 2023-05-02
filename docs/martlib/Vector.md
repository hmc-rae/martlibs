# Vector
A struct representing a vector in 2D space, complete with features such as translation, rotation, scaling, etc.

By hmc-rae

## Version
 - 1.0
	- First release

## Usage

### Operators
```
v + v       Translate/Add				<a.X + b.X, a.Y + b.Y>
v - v       Translate/Subtract				<a.X - b.X, a.Y - b.Y>

v * v       Multiply terms				<a.X * b.X, a.Y * b.Y>
v * s       Multiply by scalar     			<a.X * s, a.Y * s>

v / v       Divide terms				<a.X / b.X, a.Y / b.Y>
v / s       Divide by scalar				<a.X / s, a.Y / s>

v % v       Modulo terms				<a.X % b.X, a.Y % b.Y>
v % s       Modulo by scalar				<a.X % s, a.Y % s>

v ^ v       Project/rotate				(b.Unit.Orthogonal) * a.Y) + (b.Unit * a.X)

==          Equivalence
!=          Nonequivalence
<           Strictly less than (both axes)
<=          Less than (either axis)
>           Strictly greater than (both axes)
>=          Greater than (either axis)
```

### Fields
```csharp
double X, Y
```
The X & Y coordinates that make up the vector.

```csharp
Vector Orthogonal
```
An orthogonal (rotated by 90 degrees, clockwise) version of this vector.

```csharp
double SqrMagnitude
```
The square magnitude of the vector

```csharp
double Magnitude
```
The magnitude of the vector

```csharp
Vector UnitVector
```
The unit form of the current vector (reduced to a magnitude of 1)

```csharp
Vector Inverse
```
The inverted form of the current vector so that `Inverse + Normal = <0, 0>`

```csharp
Vector Absolute
```
The absolute form of the current vector (both axis > 0)

### Methods

```csharp
Vector(double x, double y)
```
Default constructor - creates a vector with the given coordinates.


```csharp
void Set(double x, double y)
```
Sets both coordinates to the new provided coordinates.

```csharp
Vector Power(double e)
```
Raises both terms of the vector to the given exponent.

```csharp
bool Within(Vector heading, Vector off)
```
Returns true if the calling vector has an angle deviance from the heading vector that is *less than* the given offset vector.
Both vectors `heading` and `off` should be in unit vector form.
The calling vector should be in unit vector form.