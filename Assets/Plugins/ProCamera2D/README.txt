## UPGRADE GUIDE ##

To upgrade to a new ProCamera2D version you should:
- Open a new empty scene
- Delete ProCamera2D folders ("Assets/ProCamera2D" and "Assets/Gizmos/ProCamera2D")
- Import the new package




## QUICK START ##

Select your game camera (called Main Camera by default) and drag the ProCamera2D component file to the Main Camera GameObject.
You can find the main file under "ProCamera2D/Runtime/ProCamera2D.cs".
You should now see the ProCamera2D component editor on the Inspector window when you select your Main Camera GameObject.




## USER GUIDE ##

For more information about all ProCamera2D features and how to use them please visit our User Guide at: 
http://www.procamera2d.com/user-guide




## SUPPORT ##

Do you have an issue you want to see resolved, a suggestion or you simply want to know if this is the right plugin for your game? Get in touch using any of the links below and we’ll do our best to get back to you ASAP.

Contact Form - http://www.procamera2d.com/support
Unity forums - http://goo.gl/n80dMb
Twitter - http://www.twitter.com/lpfonseca




## CHANGELOG ##

2.9.5
- Core - Implemented a method to allow stopping the screen size update coroutine
- Rooms - Implemented the above method to fix an issue where the screen size update would interfere with the transition

2.9.4
- Core - Only allow applying influences if the camera is active and enabled to avoid memory leaks

2.9.3
- Core - Implemented assembly definitions files - This forced the reorganization of folder structure
- PanAndZoom - Fixed an issue where the zoom speed was dependent on the framerate of the game

2.9.2
- Core - Minor tweaks and cleanup

2.9.1
- ContentFitter - Fixed an issue where the camera matrix wasn't reset properly after disabling the extension
- Rooms - Fixed transitions with a duration equal to zero not applying properly under some circumstances (again)
- NumericBoundaries - Fixed NaN error that could occur under rare circumstances
- NumericBoundaries - Added an option to delay the activation of the extension to avoid jittery motion

2.9.0
- Core - Support for Unity 2022.x
- Core - Fixed an issue where some users were getting timeout errors caused by the SRP preprocessor defines
- Core - Removed compatibility with Unity 5.5 and earlier
- Rails - Fixed nodes not displaying correctly under some Unity versions
- Rooms - Fixed transitions with a duration equal to zero not applying properly under some circumstances

2.8.2
- Core - Replaced all parent setting calls with SetParent
- Core - Implemented the ISerializationCallbackReceiver on the base extension class
- Rooms - Fixed an issue that prevented the transitions from working on rooms that were added at runtime
- Rooms - Fixed the transition events being incorrectly ordered on start
- Rails - Added the Shift as an alternative key to delete nodes

2.8.1
- Core - Support for Unity 2021.x
- Core - Fixed timeout issue when trying to find the current rendering pipeline
- Core - Auto-instantiate the unity events to avoid issues with users adding them through code

2.8.0
- Core - Removed support for the deprecated 3rd-party plugin 2D Toolkit (TK2D)
- Parallax - Added support for Universal Rendering Pipeline (URP)

2.7.3
- Core - Added a method (TranslateCamera) to move the camera while still retaining internal values (useful for games that have huge worlds and want to workaround the floating point precision issues)
- Rooms - Fixed an issue that prevented the size transition not occuring on certain occasions

2.7.2
- Shake - Added support to ignore delta time when using FixedUpdate
- Rails - Fixed an issue that didn't allow to set an offset to rails targets added through code
- Rails - Fixed an issue where the removed rails targets were not deleted
- Rails - Added support to adding targets with a gradual influence over time

2.7.1
- NumericBoundaries - Fixed a bug where setting the SoftArea to 0 would cause an invalid position for the camera

2.7.0
- Core - Support for the latest Unity version - 2019.3.1f1
- PanAndZoom - Fixed pan smoothness not automatically resetting after a zoom movement
- Timeline - Moved timeline related scripts to a .unitypackage to avoid conflicts in case the user doesn't have the Unity Timeline package 

2.6.12
- PanAndZoom - Added support for a "dead-zone" that prevents unwanted pan movements

2.6.11
- Cinematics - Fixed numeric boundaries not being taken into consideration on return

2.6.10
- Core - Updated support for Unity 2019.x
- Rooms - Improved support of multiple cameras and multiple Rooms extensions

2.6.9
- Added support for Unity 2019.1
- PanAndZoom - Mark the pan target object as DontDestroyOnLoad if the extension is also marked

2.6.8
- Core - Editor code optimizations
- Core - Added undo for adding camera targets
- Core - Added a warning for cross scene references
- Parallax - Added support for independent horizontal and vertical speeds per parallax layer
- Repeater - Added a method for changing the repeating object at runtime
- Shake - Added undo for changing presets lists
- Rooms - Added a parameter to the EnterRoom method to force the transition

2.6.7
- TransitionsFX - Fixed error on editor caused by removed property
- LimitDistance - Fixed bug that occurred on XZ and YZ orientations

2.6.6
- Core - Added an option to ignore the timeScale. This was possible before using ManualUpdate but wasn't very straightforward
- Rooms - Added support for having multiple Rooms extensions 
- ContentFitter - Corrected target size calculations on fixed width and height modes
- Shake - Updated StopShaking method for improved accuracy
- PanAndZoom - Added a parameter for automatic input detection (default) or choose between touch and/or mouse 

2.6.5
- Rooms - Added support for relative positioning the rooms, if the extension is added to an independent GameObject
- Rooms - Fixed a transition not triggering when the zoom was equal to the room size
- Rooms - The GetCameraSizeForRoom method is now public as well as the OriginalSize property
- PixelPerfect - Automatically call the ResizeCameraToPixelPerfect method after setting the zoom level at runtime

2.6.4
- PanAndZoom - Fixed support for WebGL on Unity 2018.x
- Rooms - Better error handling when a room is not found
- Core - Prevented a null reference exception on some occasions where the main component was destroyed before the extensions

2.6.3
- TriggerInfluence - Added mode option to choose which axes are influenced
- TransitionsFX - Fixed a shader compilation error on the PSVita
- GeometryBoundaries - The MoveInColliderBoundaries helper member is now public

2.6.2
- PanAndZoom - (Mobile) Fixed a slight pan movement that could occur while moving over uGUI
- PanAndZoom - Fixed the stutter that occured if there was a global offset applied to the core component
- ZoomToFit - Set the initial screen size from the core component for improved consistency

2.6.1
- PanAndZoom - Replaced platform dependent compilation with touch support verification for improved compatibility
- PanAndZoom - Added Pan start and finish events
- Triggers - Improved support for multi-scenes setups

2.6.0
- Unity 2018.1 - Added support for the latest beta
- Timeline - Added Timeline actions for adding and removing camera targets
- Cinematics - Fixed stuttering that occured with the final step of the cinematic when used together with the NumericBoundaries extension

2.5.5
- Core - Fixed null reference that could occur if an extension was not initialized properly due to the randomized script execution order

2.5.4
- LimitDistance - (Regression) Fixed incorrect reset of the smoothed camera position which could interfere with other extensions dependent on it
- PanAndZoom - Added two booleans (IsZooming and IsPanning) to know when the camera is zooming and/or panning

2.5.3
- PanAndZoom - Fixed bug that occurred on mobile when activating the extension at a later stage rather than at start
- PixelPerfectSprite - Fixed errors that could occur if there was no PixelPerfect extension enabled
- PixelPerfectSprite - Reduced number of warnings in case no ProCamera2D is found on the scene (a few still have to occur)
- TriggerBoundaries - Fixed disabling NumericBoundaries extension when no value is set

2.5.2
- Core - Added two new events - "OnUpdateScreenSizeFinished" and "OnDollyZoomFinished"
- Core - Code cleanup
- Parallax - Added compatibility with the ContentFitter extension
- TransitionsFX - Added option to use realtime transitions
- PanAndZoom - Added option to choose which mouse button to use for panning
- TriggerBoundaries - Fixed bug when comparing trigger boundaries that use the relative mode

2.5.1
- ContentFitter - Small tweaks

2.5.0
- ContentFitter - New extension! Fit anything on screen and easily control how it looks across different screen sizes

2.4.8
- LimitDistance - Added an option to use the targets position instead of the camera center for calculations
- LimitDistance - Fixed stutter when the camera target goes beyond the limits

2.4.7
- Core - Removed calls to static instance for easier support of multiple cameras setups
- PanAndZoom - Added support for multiple cameras setups
- PanAndZoom - Added support for multiple fingers panning
- PixelPerfect - Added a getter for the current viewport scale (can be useful for other components)

2.4.6
- LimitDistance - Added support for limiting the top, bottom, left and right directions instead of only vertically and horizontally
- LimitDistance - Fixed incorrect reset of the smoothed camera position which could interfere with other extensions dependent on it
- BaseTrigger - Disabling a trigger now calls the ExitedTrigger method for consistency
- ZoomToFit - Added a "CompensateForCameraPosition" property (on by default) that makes the calculations from the camera center and not the targets 
- PanAndZoom - Fixed wobbly zoom behaviour that could occur under certain circumstances
- PanAndZoom - Zooming while over pan edges is now supported
- PanAndZoom - 2DToolkit - Fixed zooming when the start zoom factor is different from one
- PlayMaker - Added the actions back to the package while transitioning to the new PlayMaker Ecosystem

2.4.5
- Rooms - Added a move gizmo so you can easily move rooms while in edit mode
- Rooms - Added a property (CurrentRoom) that returns the room the camera is currently in, if any

2.4.4
- PanAndZoom - Added an option to prevent panning and zooming if the user is pointing (or touching) on an UI element
- PanAndZoom - Fixed issue that could cause an instant camera movement after zooming

2.4.3
- Core - Added an IsMoving property
- Core - Added an overloaded AddCameraTargets method that takes a list of CameraTarget's
- PixelPerfect - Zoom is now applied additively for greater control
- Rooms - Added a "GetRoom" method that returns a room by its ID. Useful, for example, if you're trying to know information about a specific room size
- PlayMaker - Current actions were marked as obsolete. New and improved actions will be added directly to the PlayMaker ecosystem for improved integration

2.4.2
- Core - Fixed a OutOfRange error that could occur when removing targets at runtime
- Rooms - Added an extra parameter to the EnterRoom method that allows you to instantly enter a room (bypassing the configured transition)
- Rooms - Fixed a NaN error that could occur while transitioning between rooms
- Rooms - Added a PlayMaker action for the EnterRoom method
- PlayMaker - All actions now extend the FmsStateActionProCamera2DBase for consistency

2.4.1
- Core - Added a public getter property for the total influences currently applied to the camera
- PanAndZoom - Fixed small glitch when used together with the PointerInfluence extension
- Parallax - Fixed bug that caused the first camera clear flags to be set to Depth

2.4.0
- NumericBoundaries - Added an option to enable soft boundaries (smooth edges) for eased camera stops
- NumericBoundaries - Removed the elastic boundaries option

2.3.4
- Core - CalculateScreenSize method is now public. It should be called if the camera size is modified externally
- PixelPerfect - Fixed flickering that could occur in certain situations if the PPU was low
- PlayMaker - Updated actions to support the latest version

2.3.3
- Core - Added an option to switch between relative and world units offset
- CameraWindow - Added an option to switch between relative and world units position and size
- ZoomToFit - Added a gizmo of the targets' bounding box for easier debugging
- Shake - Fixed error when adding presets to a newly created extension

2.3.2
- Parallax - Added two parameters (FrontDepthStart and BackDepthStart) to customize the parallax cameras depth
- SpeedBasedZoom - Removed unused parameters
- Shake - Fixed error when using a constant shake preset with a delta time of zero
- Repeater - Automatically calculate the size of a repeated object if it's a Sprite
- Core - Prevented null reference error on extension editors under specific conditions

2.3.1
- Fix for ScriptableObjectUtility error

2.3.0
- Shake - Revamped extension (Smoother results, file presets, constant shakes support)
- Core - Automatically removes invalid camera targets
- PanAndZoom - Horizontal (left/right) and vertical (top/bottom) pan edges are now independent for greater flexibility
- Rooms - Added a method (RemoveRoom) to remove a room by its name
- Multiple - Added ProCamera2D's delta time to all SmoothDamp calls

2.2.9
- Core - Added a static property (Exists) to know if there's a ProCamera2D present in the scene
- Parallax - Added an option to toggle the parallax layers size adjustment
- TriggerZoom - The trigger now has precedence over other zoom extensions

2.2.8
- Core - Support for Unity 5.6 (beta 5)
- Core - Added an event (OnCameraResize<Vector2>) that fires when the camera resizes
- Core - Automatically detect screen resolution changes during runtime and update necessary values accordingly

2.2.7
- Core - Support for Unity 5.6 (beta 3)
- Repeater - Fixed a bug where an unnecessary object was created at edit time

2.2.6
- Rooms - Only add an initial room when creating the component, not if there's none present
- Core - Added some new methods: "ResetMovement", "ResetSize", "ResetExtensions" and "CenterOnTargets"
- PlayMaker - Added a new action: ShakeStop

2.2.5
- ForwardFocus - Added a parameter (MovementThreshold) that prevents the focus from being changed if the camera movement is smaller than it
- Rails - Added a new method (RemoveRailsTarget)
- Core - Improved error handling in case of missing ProCamera2D core component

2.2.4
- TriggerBoundaries - Fixed a bug that prevented them from working if no trigger was set as starting boundaries
- Cinematics - Fixed a bug on the editor that prevented it from showing the CinematicTargetReached UnityEvent

2.2.3
- Rooms - Removed the dependency from having the NumericBoundaries extension on the same GameObject
- Rooms - Added an event that fires when the camera target has exited all rooms
- Cinematics - Replaced standard C# events with Unity events for easier integration with the editor

2.2.2
- Core - Added a Dolly Zoom (Hitchcock effect) method. Created an example scene for it
- Rooms - Added a method to set the default numeric boundaries settings for when leaving all rooms
- PanAndZoom - Prevented unintentional pans when outside of the GameView on the editor

2.2.1
- Core - Greatly improved rendering performance of the editor
- PanAndZoom - Fixed an issue that prevented zooming after toggling the component
- Rooms - Added an ID property that can be used to identify each room
- Rooms - OnStartedTransition and OnFinishedTransition now send the previous room too
- Rooms - Editor tweaks

2.2.0
- New extension - Rooms! Easily create and manage multiple rooms on your scene
- Parallax extension - Added a new method (CalculateParallaxObjectsOffset) to manually recalculate parallax objects offset if needed
- PanAndZoom extension - Added a boolean (ResetPrevPanPoint) to prevent movement jumps after toggling the component
- Added links to documentation on core, extensions and triggers' editor windows
- Fixed an issue with the SpeedBasedZoom extension that caused an hard stop when reaching the maximum and minimum zoom values

2.1.5
- NumericBoundaries extension editor tweaks
- Fixed PanAndZoom extension bug when on the XZ or YZ axis
- Fixed shake not stopping to correct position when using the PixelPerfect extension
- Support for Unity 5.5

2.1.4
- The values on the EaseFromTo method are now automatically clamped to prevent overshooting
- Fixed missing static instance of the TransitionsFX extension on scene change
- Fixed typo on the AdjustCameraTargetInfluence core method

2.1.3
- Fixed PanAndZoom bug on touch devices that slowed down pan speed

2.1.2
- Added additional methods to the TransitionsFX extension to easily update the properties during runtime
- Fixed PanAndZoom bug that prevented it from working properly on the XZ axis

2.1.1
- Improved TransitionsFX performance
- Fixed memory allocation on the Repeater extension
- Fixed extremely quick transition routines not being cleared on certain circumstances
- Fixed bug in PanAndZoom extension that could prevent zoom when using negative speed values

2.1.0
- TransitionsFX extension now supports the use of textures for completely customizable transitions
- The drag speed of the PanAndZoom extension is now based on "real" world coordinates, giving it a much more natural feel on all platforms

2.0.5
- Support for removal and destruction of extensions during runtime
- Fixed a bug on the PanAndZoom extension where the camera could get stuck on its max/min zoom
- Improved TransitionsFX shaders compatibility with older devices
- Fire only one transition start/end event even if multiple occur during one complex animation
- Fixed a bug on the Cinematics extension when EaseInDuration is set to 0
- Fixed a bug on the Cinematics extension where it wouldn't return to origin if there was no camera target

2.0.4
- Added Pause/Unpause methods to the Cinematics extension
- Cinematics extension now supports parented cameras
- Fixed camera stutter if trying to zoom paste the limits with the PanAndZoom extension
- Fixed regression bug - TriggerBoundaries wouldn't override previous one if transition still occurring 

2.0.3
- Added an AutoScaleMode option to the PixelPerfect extension for better control over how the camera scales
- Allow the ClearFlags of the last camera on the Parallax extension to be manually set if needed
- Fixed regression - bug calculating camera size when using a 2DToolkit Pixels Per Meter camera

2.0.2
- Improved the LetterBox of the Cinematics extension so it doesn't render when it's set to 0
- Improved TriggerInfluence algorithm to provide smoother results when using the ExclusivePercentage parameter
- Improved LetterBox and TransitionsFX shaders
- Fixed camera start position when parented
- Fixed null reference bug on PixelPerfectSprite under certain circumstances  
- Fixed a bug where a PixelPerfectSprite could get misplaced on certain occasions
- Added the action "OnShakeCompleted" to the Shake extension to know when a shake has completed
- Reset Shake presets and Rails nodes from the editor after scene load

2.0.1
- Support for Unity 5.4.0
- Fixed skipping certain TriggerBoundaries transitions on some edges cases
- Fixed bug calculating camera size when using a 2DToolkit Pixels Per Meter camera
- Fixed a null reference when destroying a Pixel Perfect sprite

2.0.0
- New extension - TransitionsFX! Transition between scenes or camera positions with beautiful effects
- Major refactor focusing on code architecture and performance
- Moved LimitSpeed out of the core into a separate extension
- Moved LimitDistance out of the core into a separate extension
- Allow the camera to be parented to any kind of hierarchy
- Added the option to not snap the camera to the pixel grid when using SnapMovementToGrid on the PixelPerfect extension
- Added the option for the Shake extension to ignore the timeScale, allowing it to work even if the game is paused
- Added a "ApplyInfluenceIgnoringBoundaries" method to the Shake extension
- Fixed StopShaking method on the Shake extension
- More code comments

1.9.2
- Added a parameter to the PanAndZoom extension that allows to define how fast the camera inertia should stop once the user starts dragging after a previous pan movement
- NumericBoundaries extension editor tweaks

1.9.1
- Added an indication of the current camera velocity to the SpeedBasedZoom extension editor for reference
- Added Load/Save buttons to the presets list on the Shake extension for an easier control
- Fixed a bug that could cause a few extensions to start before the core is initialized
- Added a property to know if a TriggerBoundaries is the currently active one
- Use the original camera parent when parenting the camera for the Shake extension

1.9.0
- New extension - PanAndZoom! Move and/or zoom the camera with touch (on mobile) or with the mouse (on desktop)
- Re-enable triggers calculations after their GameObjects have been disabled and enabled again
- Added a Zoom method to the core
- Allow TriggerBoundaries to be defined as starting boundaries at run-time

1.8.1
- Added a Rect property to ProCamera2D core that will change its rect and the rect of parallax cameras if existent
- Fixed build error related to EditorPrefsX class on Windows Phone 8.1

1.8.0
- New extension - Repeater!
- The ProCamera2D editor list of available extensions and triggers is now dynamically populated.
- Improved Trigger Rails editor
- Name parallax cameras according to their speeds for easier identification
- Gizmos drawing optimizations
- Implemented ISerializationCallbackReceiver so ProCamera2D works even during runtime code reloads
- Changed the OnReset handler of BasePC2D to public
- Elastic numeric boundaries
- Added a method (RemoveCinematicTarget) to manually remove a target from the Cinematics plugin
- Added support for the Letterbox effect when using the Parallax extension

1.7.3
- Added a method (TestTrigger) to the BaseTrigger class that allows to manually force the trigger collision test
- Added a method (AddCinematicTarget) to the Cinematics extension that allows to manually add a new cinematic target at runtime
- Added a method (AddRailsTarget) to the Rails extension that allows to manually add a new rails target at runtime
- Fixed a bug that could cause a null reference after destroying an object that had a ProCamera2D extension

1.7.2
- Allow Rails extension to be added to a different GameObject than the one with ProCamera2D
- Support for Unity 5.3.0

1.7.1
- Triggers now dispatch public OnEnteredTrigger and OnExitedTrigger events
- Tweaked Rails triggers icons
- Added a Rails snapping parameter to ProCamera2D options panel
- Added a left and right handle to the Rails so it's easier to create new nodes

1.7.0
- New extension - Rails!
- New trigger - TriggerRails!
- Improved extensions architecture and performance
- Added a ManualUpdate option to the UpdateType  
- Fixed zoom size when not using Auto-Scale on the Pixel Perfect extension

1.6.3
- Added 3 new methods to the Core API - AddCameraTargets, RemoveAllCameraTargets and Reset
- Highlight correspondent GameObject when selecting a target from the camera targets list
- Allow the Cinematics extension to be called at the start of a scene
- Added the option to zoom by changing the FOV instead of moving the camera
- Fixed a bug where the Trigger Influence exclusive area wouldn't point to the correct position on the XZ and YZ axes
- Fixed a bug on the Zoom Trigger that could cause the camera to zoom weirdly when not using "SetSizeAsMultiplier" 

1.6.2
- Fixed a bug that would show the incorrect triggers gizmos position if using a Circle
- Only show the Zoom trigger preview size when selected
- On the Zoom and Influence triggers, when not using the targets mid point, use the provided target to calculate the distance to the center percentage
- Zoom trigger "Reset Size On Exit" is now "Reset Size On Leave" so it resets progressively as you leave the trigger instead of only once you exit
- Fixed a bug where after deleting boundaries related extensions / triggers the camera could get stuck at (0,0)

1.6.1
- The camera is only parented if using the Shake extension
- Fixed an issue where deleting Cinematic camera targets could throw an (harmless) error to the console
- When using the Cinematics extension, if a target has a Hold Duration less than 0, the camera will follow it indefinitely
- Added a method (GoToNextTarget) to the Cinematics extension that allows you to skip the current target
- Prevent the Game Viewport size on the Pixel Perfect extension to go below (1, 1)
- Fixed a bug where the TriggerBoundaries would have a missing reference if instantiated manually

1.6.0
- Moved Camera Window, Numeric Boundaries and Geometry Boundaries from the core into their own extensions, leaving the core as light as possible
- Added a new powerful extension (Cinematics) that replaces the CinematicFocusTarget
- Added a new demo (TopDownShooter) that shows how to use multiple ProCamera2D features in one simple game
- Tweaked Zoom-To-Fit extension to support camera boundaries
- Tweaked Speed-Based-Zoom extension to support camera boundaries
- Forward focus auto-adjusts to the camera size when zooming
- Gizmos for triggers are now shown as circles instead of spheres
- Added the option for triggers to be triggered by a specific GameObject instead of always using the camera targets mid-point
- Renamed Plugins to Extensions and Helpers to Triggers

1.5.3
- Upgraded Shake extension - Presets support, rotation and overall tweaks
- Zooming with perspective cameras is now made by moving the camera instead of changing the field of view to avoid distortion
- Fix TriggerBoundaries left and right gizmos incorrect size on XZ and YZ axis
- Maintain pixel-perfect during shakes

1.5.2
- Fixed bug when applying influences on XZ and YZ axis

1.5.1
- Added option to reset size when exiting a zoom trigger
- Added option to disable Zoom-To-Fit extension when there's only one target
- Disable Parallax extension toggle button when in perspective mode
- Fixed a bug when adding targets progressively
- Tweaked the CinematicFocusTrigger to take all influences in consideration when returning from the cinematic position
- Added an optional "duration" parameter to the RemoveCameraTarget method that allows to remove a camera target progressively by reducing it's influence over time
- Added an optional “duration” parameter to the UpdateScreenSize method that allows to manually update the screen size progressively

1.5.0
- Added a new extension, Speed Based Zoom that adjusts the zoom based on how fast the camera is moving.

1.4.1
- Fixed a bug where if the camera is a prefab it would loose its targets on scene load, if the changes weren't saved

1.4.0
- Pixel perfect support!! :)
- Fixed a few more bugs related to setting the Time.timeScale to 0 (Thanks to the users who reported this issue and helped solving it!)
- Added a user-guide link to the top of ProCamera2D editor for easier access to the documentation

1.3.1
- Added a ParallaxObject MonoBehaviour that makes a GameObject position on the scene view to match the same relative position to the main parallax layer during runtime.
- Fixed slight camera movement on start when no targets are added
- Fixed a bug where if Time.timeScale was put to 0 the camera would stop following its targets afterwards
- Fixed a few Playmaker actions descriptions
- Fixed target vertical offset when on XZ and YZ axis
- Fixed PointerInfluence extension on XZ and YZ axes

1.3.0
- Full compatibility with 2DToolkit!
- Added namespace to UpdateType and MovementAxis enums to avoid conflicts with other packages
- Added the option to set the TriggerZoom helper size as a direct value instead of as a multiplier
- Added a new method to stop shaking and a flag to check if the camera is currently shaking
- Fixed bug with InfluenceTrigger not smoothing correctly the value on first entrance if ExclusiveInfluencePercentage is 1

1.2.0
- Added support for perspective cameras
- Fixed bug with camera getting stuck when using Camera Window and Numeric Boundaries
- Fixed bug that made camera float away when using an offset on a specific axis and turned off following on that same axis

1.1.0
- Custom PlayMaker actions with full API support
- Fix for AdjustCameraTargetInfluence method when starting at values different than zero

1.0.0
- Public release