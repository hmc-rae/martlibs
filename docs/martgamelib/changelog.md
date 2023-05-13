# martGameLib
The backend of a 2D game engine.

By hmc-rae.

## Changelog

 - 0.1
	- First Release (Early Alpha)
		- The engine will now run and allow basic manipulation, though very bare bones. All scenes must be created manually by user code - scenes cannot be made automatically yet.
		- InputManager not completely functional
		- No visual editor as of yet.
		- All components that need to read files to load data will now no longer attempt to read if the directory does not exist. This is a stopgap measure.
		- Transform.Rotation is now properly set to UNIT_X instead of ZERO.
		- Rotations during render were aligned incorrectly, meaning objects would spin inversely to the camera rotation. Fixed.
	- 0.1.1
		- Overhaul of InputManager;
			- Reworked how keys are registered
			- Added new function range, KeyBool, which returns a boolean representing the state of the key referenced.
			- Changed how default key state functions work - they now return an integer (1 or 0).
			- Added new function KeyDelta, returning 1 if one key is pressed, -1 if the other, and 0 if both or none.
			- The above points apply to mouse buttons too.
		- Fixed rotation & rendering
			- Rotations would appear inversely to how they actually are, according to a unit vector.