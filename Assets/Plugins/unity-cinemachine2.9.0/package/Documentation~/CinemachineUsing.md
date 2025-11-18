# Using Cinemachine

Using Cinemachine requires a new way of thinking about working with cameras. For example, you might have invested heavily in carefully scripted camera behaviors. However, Cinemachine can give the same results, if not better, in less time.


## Virtual Cameras

Cinemachine does not create new cameras. Instead, it directs a single Unity camera for multiple shots. You compose these shots with __Virtual Cameras__. Virtual Cameras move and rotate the Unity camera and control its settings.

The Virtual Cameras are separate GameObjects from the Unity Camera, and behave independently. They are not nested within each other. For example, a Scene might look like this:

![A Scene containing a Unity camera with Cinemachine Brain (blue) and multiple Virtual Cameras (red)](images/CinemachineSceneHierarchy.png)

The main tasks that the Virtual Camera does for you:

* Positions the Unity camera in the Scene.
* Aims the Unity camera at something.
* Adds procedural noise to the Unity camera. Noise simulates things like handheld effects or vehicle shakes.

Cinemachine encourages you to create many Virtual Cameras. The Virtual Camera is designed to consume little processing power. If your Scene is performance-sensitive, deactivate all but the essential Virtual Cameras at any given moment for best performance.

It is recommended that you use a single Virtual Camera for a single shot. Take advantage of this to create dramatic or subtle cuts or blends. Examples:

* For a cutscene where two characters exchange dialog, use three Virtual Cameras: one camera for a mid-shot of both characters, and separate Virtual Cameras for a close-up of each character. Use Timeline to synchronize audio with the Virtual Cameras.

* Duplicate an existing Virtual Camera so that both Virtual Cameras are in the same position in the Scene. For the second Virtual Camera, change the FOV or composition. When a player enters a trigger volume, Cinemachine blends from the first to the second Virtual Camera to emphasize a change in action.

One Virtual Camera has control of the Unity camera at any point in time. This is the __live__ Virtual Camera. The exception to this rule is when a blend occurs from one Virtual Camera to the next. During the blend, both Virtual Cameras are live.

## Cinemachine Brain

The Cinemachine Brain is a component in the Unity Camera itself. The Cinemachine Brain monitors all active Virtual Cameras in the Scene. To specify the next live Virtual Camera, you [activate or deactivate](https://docs.unity3d.com/Manual/DeactivatingGameObjects.html) the desired Virtual Camera's game object. Cinemachine Brain then chooses the most recently activated Virtual Camera with the same or higher priority as the live Virtual Camera.  It performs a cut or blend between the previous and new Virtual Cameras.

**Tip**: Use Cinemachine Brain to respond to dynamic game events in real time. It allows your game logic to control the camera by manipulating priorities. This is particularly useful for live gameplay, where action isn’t always predictable. Use [Timeline](CinemachineTimeline.md) to choreograph cameras in predictable situations, like cutscenes. Timeline overrides the Cinemachine Brain priority system to give you precise, to-the-frame camera control.

## Moving and aiming

Use the [__Body__ properties](CinemachineVirtualCameraBody.md) in a Virtual Camera to specify how to move it in the Scene. Use the [__Aim__ properties](CinemachineVirtualCameraAim.md) to specify how to rotate it.

A Virtual Camera has two targets:

* The __Follow__ target specifies a GameObject for the Virtual Camera to move with.
* The __Look At__ target specifies the GameObject to aim at.

Cinemachine includes a variety of procedural algorithms to control moving and aiming. Each algorithm solves a specific problem, and has properties to customize the algorithm for your specific needs. Cinemachine implements these algorithms as `CinemachineComponent` objects. Use the `CinemachineComponent` class to implement a custom moving or aiming behavior.

The __Body__ properties offer the following procedural algorithms for moving the Virtual Camera in a Scene:

* __Transposer__: Move in a fixed relationship to the __Follow__ target, with optional damping.
* __Do Nothing__: Do not move the Virtual Camera.
* __Framing Transposer__: Move in a fixed screen-space relationship to the __Follow__ target, with optional damping.
* __Orbital Transposer__: Move in a variable relationship to the __Follow__ target, optionally accepting player input.
* __Tracked Dolly__: Move along a predefined path.
* __Hard Lock to Target__: Use the same position at the __Follow__ target.

The __Aim__ properties offer the following procedural algorithms for rotating a Virtual Camera to face the __Look At__ target:

* __Composer__: Keep the __Look At__ target in the camera frame, with compositional constraints.
* __Group Composer__: Keep multiple __Look At__ targets in the camera frame.
* __Do Nothing__: Do not rotate the Virtual Camera.
* __POV__: Rotate the Virtual Camera based on the user’s input.
* __Same As Follow Target__: Set the camera’s rotation to the rotation of the __Follow__ target.
* __Hard Look At__: Keep the __Look At__ target in the center of the camera frame.


## Composing a shot

The [__Framing Transposer__](CinemachineBodyFramingTransposer.md), [__Composer__](CinemachineAimComposer.md), and [__Group Composer__](CinemachineAimGroupComposer.md) algorithms define areas in the camera frame for you to compose a shot:

* __Dead zone__: The area of the frame that Cinemachine keeps the target in.

* __Soft zone__: If the target enters this region of the frame, the camera will re-orient to put it back in the dead zone.  It will do this slowly or quickly, according to the time specified in the Damping settings.

* __Screen__: The screen position of the center of the dead zone.  0.5 is the center of the screen.

* __Damping__: Simulates the lag that a real camera operator introduces while operating a heavy physical camera. Damping specifies quickly or slowly the camera reacts when the target enters the __soft zone__ while the camera tracks the target. Use small numbers to simulate a more responsive camera, rapidly moving or aiming the camera to keep the target in the __dead zone__. Larger numbers simulate heavier cameras, The larger the value, the more Cinemachine allows the target to enter the soft zone.

The __Game Window Guides__ gives an interactive, visual indication of these areas. The guides appear as tinted areas in the [Game view](https://docs.unity3d.com/Manual/GameView.html).

![Game Window Guides gives a visual indication of the damping, screen, soft zone, and dead zone](images/CinemachineGameWindowGuides.png)

The clear area indicates the __dead zone__. The blue-tinted area indicates the __soft zone__. The position of the soft and dead zones indicates the __screen__ position. The red-tinted area indicates the __no pass__ area, which the target never enters. The yellow square indicates the target.

Adjust these areas to get a wide range of camera behaviors. To do this, drag their edges in the Game view or edit their properties in the Inspector window. For example, use larger damping values to simulate a larger, heavier camera, or enlarge the __soft zone__ and __dead zone__ to create an area in the middle of the camera frame that is immune to target motion. Use this feature for things like animation cycles, where you don’t want the camera to track the target if it moves just a little.

## Using noise to simulate camera shake

Real-world physical cameras are often heavy and cumbersome. They are hand-held by the camera operator or mounted on unstable objects like moving vehicles. Use [Noise properties](CinemachineVirtualCameraNoise.md) to simulate these real-world qualities for cinematic effect. For example, you could add a camera shake when following a running character to immerse the player in the action.

At each frame update, Cinemachine adds noise separately from the movement of the camera to follow a target. Noise does not influence the camera’s position in future frames. This separation ensures that properties like __damping__ behave as expected.