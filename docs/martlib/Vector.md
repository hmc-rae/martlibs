# Vector
A struct representing a vector in 2D space, complete with features such as translation, rotation, scaling, etc.

By hmc-rae

## Version
 - 1.0
	- First release
		- 1.0.1
			- Added ToString overloads
			- Added field `OrthogonalC`, which represents the clockwise rotation of the vector
			- Added field `Flip`, which represents the flipped version of the vector
			- Field `Orthogonal` now represents the counter-clockwise rotation of the vector (rotate by Unit Y vector)
		- 1.0.2
			- Added two new fields: `Radians` and `Degrees`, both of which convert the unit vector to & from the respective scalar values.
			- Defaulted the string representation to only print 2 decimal places. Max accuracy can be reached with `Vector.ToString(int decimal)`.
		- 1.0.3
			- Fields `Radians` and `Degrees` now return negative values as well, providing a continuous range from -PI/2 to PI/2, and -180 to 180 respectively.
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
An orthogonal projection of this vector, counter-clockwise.

```csharp
Vector OrthogonalC
```
An orthogonal projection of this vector, clockwise.

```csharp
Vector Flip
```
A flipped projection of this vector, so that rotating vector A by the flipped version of itself, the result will be <1, 0>

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
Vector Flip
```
A flipped version of the given vector, so that rotating a vector by its flipped variant results in `<1, 0>`.

```csharp
Vector Absolute
```
The absolute form of the current vector (both axis > 0)

```csharp
double Radians
```
The scalar value of the unit vector, in Radians. 

```csharp
double Degrees
```
The scalar value of the unit vector, in Degrees.

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