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
	- 0.1.2
		- InputManager now properly reports movement of the mouse on the screen
		- CameraComponents now can translate a vector representing a 'real' position on the screen to a position relative to themselves within their MapRegion via `GetRelativeMousePosition`.
		- CameraComponents can now set the size of their MapRegion through `SetUnitsPerPixel`.
		- New functions `AwaitTick` and `AwaitFrame` added to BehaviorComponent: now, objects can wait for another object to either complete or start execution of their tick/frame behaviors.
	- 0.1.3
		- Added new rendercomponent: TextRenderer, which can display text on the screen.
		- Added new method to CameraComponent: `GetRealMousePosition`, which gets the mouse position adjusted through their lens to the 'real' space of the game scene.
		- `CameraComponent.GetRelativeMousePosition` now returns the mouse position adjusted purely to the camera.
		- BehaviorComponent has a new method, `Destroy(BehaviorComponent)` and `Destroy(GameObject)`, which will flag a game object to be deleted at EoF.
		- Prefabs can now be generated via `Prefab.GeneratePrefab(GameObject, string)`
	- 0.1.4
		- New function - `EnqueueFunction(Func<GameObject, int>)` added to behavior components. Any source can add a function to the component's local queue to be executed once
			the component next sees fit.
			By default, this will at least call once it's begun completion - good way to get a callback from a freshly made prefab, for example.
		- Reworked how Cameras work with other things. Cameras now have a `PixelsPerUnit` property, which automatically is applied to simple renders like BoxRenderers. 
		- `CameraComponent.GetMappedPosition(Vector)` will no longer return (NaN, NaN) if trying to map to something that's at the same position as the camera in question. 
		- TextRenderers can consult Cameras to figure out their (vertical) render size in pixels, roughly, with `TextRenderer.UnitSize`.
		- GUIEditor has begun development. Expect radical changes.
		- New attributes, `EditorVisible` and `EditorHidden`. Attach these to a field OR property to make them visible / hidden in the GUI game scene editor.