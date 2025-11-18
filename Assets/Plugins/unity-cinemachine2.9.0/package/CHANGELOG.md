# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.9.0-pre.6] - 2022-01-12
- Bugfix: Negative Near Clip Plane value is kept when camera is orthographic.
- Regression fix: could not change the projection of the main camera if a CM virtual camera is active.
- Regression fix: Axis input was ignoring CM's IgnoreTimeScale setting.
- Removed legacy .unitypackages
- New feature: CinemachineBrain may control other GameObject instead of the one it is attached to.
- Bugfix: Cinemachine assigns a default input controller delegate that returns 0 when the legacy input system is disabled.
- Cinemachine example scenes show informative text when used with Input System instead of throwing error messages.
- Regression fix: compilation errors when physics module is not present.
- GameObjects created with Gameobject menu items now follow Unity naming conventions.
- Regression fix: virtual cameras no longer forget that they are targeting groups on domain reload.
- Moved Cinemachine tools into the main Tools overlay (2022.1+), moved Freelook rig selection to a separate overlay, updated icons to support light and dark themes.
- Bugfix: 3rdPersonFollow logged console messages when looking straight up or down.
- BugFix: InputProvider no longer causes a tiny gc alloc every frame.
- Regression fix: CinemachineCollider smoothing time did not reset correctly, so it was working once only.
- Bugfix: Confiner2D now displays the calculated confining area when its vcam is selected.


## [2.9.0-pre.1] - 2021-10-26
- Added ability to directly set the active blend in CinemachineBrain.
- Bugfix: OnTargetObjectWarped() did not work properly for 3rdPersonFollow.
- Bugfix: POV did not properly handle overridden up.
- Regression fix: removed GC allocs in UpdateTargetCache.
- Bugfix: async scene load/unload could cause jitter.
- Bugfix: Input system should be read only once per render frame.
- Bugfix: Blends were sometimes incorrect when src or dst camera is looking along world up axis.
- Bugfix: Improve accuracy of Group Framing.
- New feature: Added scene view overlay tools for Cinemachine components.
- Regression fix: Lookahead works again.
- Cinemachine3rdPersonAim exposes AimTarget, which is the position of where the player would hit.


## [2.8.0] - 2021-07-13
- Bugfix: Freelook prefabs won't get corrupted after editing the Prefab via its instances.
- Bugfix: 3rdPersonFollow works with Aim components now. 
- Bugfix: Blends between vcams, that are rotated so that their up vector is different from World up, are correct now.
- Bugfix: POV recentering did not always recenter correctly, when an axis range was limited.
- Bugfix: Collider sometimes bounced a little when the camera radius was large.
- Bugfix: CinemachineVolumeSettings inspector was making the game view flicker.
- Bugfix: CinemachineVolumeSettings inspector displayed a misleading warning message with URP when focus tracking was enabled.
- Bugfix: Rapidly toggling active cameras before the blends were finished did not use the correct blend time.
- AimingRig sample scene updated with a better reactive crosshair design.
- Added API accessor for Active Blend in Clearshot and StateDrivenCamera. 
- Bugfix: Virtual Cameras were not updating in Edit mode when Brain's BlendUpdateMode was FixedUpdate.
- Bugfix: Lens mode override was not working correctly in all cases.
- Collider2D inspector: added warning when collider is of the wrong type.


## [2.8.0-pre.1] - 2021-04-21
- Switching targets (Follow, LookAt) is smooth by default. For the old behaviour, set PreviousStateIsValid to false after changing the targets.
- Bugfix: Reversing a blend in progress respects asymmetric blend times.
- Regression fix: CmPostProcessing and CmVolumeSettings components setting Depth of Field now works correctly with Framing Transposer. 
- Regression fix: 3rdPersonFollow kept player in view when Z damping was high.
- Regression fix: Physical camera properties were overwritten by vcams when "override mode: physical" was not selected.
- New sample scene: Boss cam demonstrates how to setup a camera that follows the player and looks at the player and the boss. Boss cam also shows examples of custom extensions.
- Added simplified modes to Impulse Source.
- Added secondary reaction settings to Impulse Listener.
- Added Storyboard support for ScreenSpaceOverlay and ScreenSpaceCamera camera render modes.
- Added DampingIntoCollision and DampingFromCollision properties to Cinemachine3rdPersonFollow to control how gradually the camera moves to correct for occlusions.
- Added CinemachineCore.OnTargetObjectWarped() to warp all vcams targeting an object.
- Added ability for vcam to have a negative near clip plane.
- Added Draggable Game Window Guides toggle in Cinemachine preferences. When disabled, Game Window guides are only for visualization.
- Added button to virtual camera inspectors to auto-generate the CinemachineInputProvider component if it is missing.
- Default PostProcessing profile priority is now configurable and defaults to 1000.
- Cinemachine3rdPersonFollow now operates without the physics module and without collision resolution.
- Bugfix: 3rdPersonFollow collision resolution failed when the camera radius was large.
- Bugfix: 3rdPersonFollow damping occured in world space instead of camera space.
- Bugfix: 3rdPersonFollow stuttered when Z damping was high.
- Regression fix: CinemachineInputProvider stopped providing input.
- Bugfix: Lens aspect and sensorSize were updated when lens OverrideMode != None.
- Bugfix: Changing targets on a live vcam misbehaved.
- Bugfix: Framing transposer did not handle empty groups.
- Bugfix: Interrupting a transition with InheritPosition enabled did not work.
- Bugfix: Cinemachine3rdPersonFollow handled collisions by default, now it is disabled by default.
- Bugfix: SaveDuringPlay saved some components that did not have the SaveDuringPlay attribute.
- Regression fix: Entries in the custom blends editor in CM Brain inspector were not selectable.
- GameView guides are drawn only if appropriate inspector subsection is expanded.
- FreeLook rigs are now organized in tabs in the inspector.
- New sample scene: **Boss cam** sample scene demonstrates a camera setup to follow the player and to look at the player and the boss. The scene provides  examples of custom extensions.
- New Sample scene: **2D zoom**, showing how to zoom an orthographic camera with mouse scroll.
- New Sample scene: **2D fighters**, showing how to add/remove targets gradually to/from a TargetGroup based on some conditions (here, it is the y coord of the players).
- Bugfix: CinemachineCollider's displacement damping was being calculated in world space instead of camera space.
- Bugfix: TrackedDolly sometimes introduced spurious rotations if Default Up and no Aim behaviour.
- Bugfix: 3rdPersonFollow's shoulder now changes smoothly with respect to world-up vector changes.


## [2.7.2] - 2021-02-15
- CinemachineConfiner2D now handles cases where camera window is oversized
- New sample scene (FadeOutNearbyObjects) demonstrating fade out effect for objects between camera and target using shaders. The example includes a cinemachine extension giving convenient control over the shader parameters
- Bugfix (1293429) - Brain could choose vcam with not the highest priority in some cases
- Bugfix: SaveDuringPlay also works on prefab instances
- Bugfix (1272146) - Adding vcam to a prefab asset no longer causes errors in console
- Bugfix (1290171) - Impulse manager was not cleared at playmode start
- Nested Scrub Bubble sample removed (filenames too long), available now as embedded package
- Compilation guards for physics, animation, and imgui. Cinemachine does not hard depend on anything now
- Bugfix: CM StoryBoard had a 1 pixel border
- Bugfix: CM StoryBoard lost viewport reference after hot reload
- Bugfix: FramingTransposer's TargetMovementOnly damping caused a flick.
- Bugfix: FreeLook small drift when no user input if SimpleFollowWithWorldUp
- Bugfix: InheritPosition did not work with SimpleFollow binding mode
- Bugfix: cleanup straggling post processing profiles when no active vcams
- Bugfix: Checking whether the Input Action passed to CinemachineInputHandler is enabled before using it.
- Bugfix: 3rdPersonFollow FOV was blended incorrectly when ReferenceLookAt was set to a faraway target
- Bugfix: Position predictor not properly reset
- Bugfix: Create via menu doesn't create as child of selected object
- Bugfix: Post-processing profiles not cleaned up when no active vcams
- Bugfix: Install CinemachineExamples Asset Package menu item was failing on 2018.4 / macOS
- New sample scene (2DConfinerComplex) demonstrating new CinemachineConfiner2D extension.
- Updated CharacterMovement2D script in 2D sample scenes (2DConfinedTargetGroup, 2DConfiner, 2DConfinerUndersized, 2DTargetGroup) to make jumping responsive. 
- Updated 2DConfinedTargetGroup and 2DConfiner scenes to use new CinemachineConfiner2D extension. 


## [2.7.1] - 2020-11-14
- New feature: CinemachineConfiner2D - Improved 2D confiner.
- Added ApplyAfter option to ImpulseListener, to add control over the ordering of extensions
- UI update - Moved Cinemachine menu to GameObject Create menu and Right Click context menu for Hierarchy.
- Virtual Camera Lens inspector supports display of Horizontal FOV
- Virtual Camera Lens can override orthographic and physical camera settings
- Bugfix (1060230) - lens inspector sometimes displayed ortho vs perspective incorrectly for a brief time
- Bugfix (1283984) - Error message when loading new scene with DontDestroyOnLoad
- bugfix (1284701) - Edge-case exception when vcam is deleted
- Storyboard Global Mute moved from Cinemachine menu to Cinemachine preferences.
- Bugfix - long-idle vcams when reawakened sometimes had a single frame with a huge deltaTime
- Bugfix - PostProcessing temporarily stopped being applied after exiting play mode


## [2.6.3] - 2020-09-16
- Regression fix (1274989) - OnTargetObjectWarped broken for OrbitalTransposer
- Bugfix (1276391) - CM Brain Reset did not reset Custom Blends asset in inspector
- Bugfix (1276343) - CM Brain inspector custom blends misaligned dropdown arrow
- Bugfix (1256530) - disallow multiple components where appropriate
- Bugfix: BlendList camera was incorrectly holding 0-length camera cuts
- Bugfix (1174993) - CM Brain logo was not added to Hierarchy next to Main Camera after adding vcam for the first time after importing CM.
- Bugfix (1100131) - Confiner is aware of 2D collider's offset attribute.



## [2.6.2] - 2020-09-02
### Bugfixes
- Regression fix: OnCameraCut Memory leak when using Cinemachine with PostProcessing package
- Bugfix (1272146): Checking for null pipeline, before drawing gizmos.
- Add support for disabling Physics module


## [2.6.1] - 2020-08-13
### Bugfixes
- Regression Fix: PostProcessing/VolumeSettings FocusTracksTarget was not accounting for lookAt target offset
- Regression fix: Confiner no longer confines noise and impulse
- Bugfix: StateDrivenCamera was choosing parent state if only 1 clip in blendstate, even though there was a vcam assigned to that clip
- Bugfix: vertical group composition was not composing properly
- Bugfix: CinemachineNewVirtualCamera.AddComponent() now works properly
- Bugfix: removed compile errors when Physics2D module is disabled
- Bugfix: brain updates on scene loaded or unloaded
- Bugfix (1252431): Fixed unnecessary GC Memory allocation every frame when using timeline  
- Bugfix (1260385): check for prefab instances correctly
- Bugfix (1266191) Clicking on foldout labels in preferences panel toggles their expanded state
- Bugfix (1266196) Composer target Size label in preferences panel was too big
- Bugfix: Scrubbing Cache was locking virtual camera transforms beyond the cache range
- Improved performance of path gizmo drawing
- Timeline Scrubbing Cache supports nested timelines, with some known limitations to be addressed with a future Timeline package release
- Added support for deterministic noise in the context of controlled rendering (via CinemachineCore.CurrentTimeOverride)
- Added Target Offset field to Framing Transposer
- Added Multi-object edit capabilities to virtual cameras and extensions 
- Added inspector button to clear the Scrubbing Cache


## [2.6.0] - 2020-06-04
### New Features and Bugfixes
- Added AxisState.IInputProvider API to better support custom input systems
- Added CinemachineInpiutProvider behaviour to support Unity's new input system
- Added Timeline Scrubbing cache: when enabled, simulates damping and noise when scrubbing in timeline
- Added ManualUpdate mode to the Brain, to allow for custom game loop logic
- VolumeSettings/PostProcessing: added ability to choose custom target for focus tracking
- Added CinemachineRecomposer for timeline-tweaking of procedural or recorded vcam Aim output
- Added GroupWeightManipulator for animating group member weights
- Impulse: Added PropagationSpeed, to allow the impulse to travel outward in a wave
- Impulse: added support for continuous impulses
- Added CinemachineIndependentImpulseListener, to give ImpulseListener ability to any game object
- Added 3rdPersonFollow and 3rdPersonAim for dead-accurate 3rd-person aiming camera
- Added ForceCameraPosition API of virtual cameras, to manually initialize a camera's position and rotation
- Added example scenes: Aiming Rig and Dual Target to show different 3rd person cmera styles
- FramingTransposer does its work after Aim, so it plays better with Aim components.
- Framing Transposer: add Damped Rotations option.  If unchecked, changes to the vcam's rotation will bypass Damping, and only target motion will be damped.
- Refactored Lookahead - better stability.  New behaviour may require some parameter adjustment in existing content
- Composer and Framing Transposer: improved handling at edge of hard zone (no juddering)
- Orbital Transposer / FreeLook: improved damping when target is moving
- CustomBlends editor UX improvements: allow direct editing of vcam names, as well as dropdown
- Add Convert to TargetGroup option on LookAt and Follow target fields
- Confiner: improved stability when ConfineScreenEdges is selected and confing shape is too small
- Extensions now have PrePipelineMutateState callback
- CinemachineCore.UniformDeltaTimeOverride works in Edit mode
- Added TargetAttachment property to vcams.  Normally 1, this can be used to relax attention to targets - effectively a damping override
- Bugfix: Blend Update Method handling was incorrect and caused judder in some circumstances
- Bugfix: VolumeSettings blending was popping when weight was epsilon if volume altered a non-lerpable value
- Bugfix (1234813) - Check for deleted freelooks
- Bugfix (1219867) - vcam popping on disable if blending
- Bugfix (1214301, 1213836) - disallow structural change when editing vcam prefabs
- Bugfix (1213471, 1213434): add null check in editor
- Bugfix (1213488): no solo for prefab vcams
- Bugfix (1213819): repaintGameView on editor change
- Bugfix (1217306): target group position drifting when empty or when members are descendants of the group
- Bugfix (1218695): Fully qualify UnityEditor.Menu to avoid compile errors in some circumstances
- Bugfix (1222740): Binding Modes, that don't have control over axis value range, are not affected by it. 
- Bugfix (1227606): Timeline preview and playmode not the same for composer with hand-animated rotations
- Bugfix: Confiner's cache is reset, when bounding shape/volume is changed.
- Bugfix (1232146): Vcam no longer jerks at edge of confiner bound box.
- Bugfix (1234966): CompositeCollider scale was applied twice.


## [2.5.0] - 2020-01-15
### Support HDRP 7 and URP simultaneously
- Accommodate simultaneous precesnce of HDRP and URP
- Regression fix: Axis was always recentered in Edit mode, even if recentering is off


## [2.4.0] - 2020-01-10
### HDRP 7 support and bugfixes
- Storyboard: added global mute function
- New vcams are by default created matching the scene view camera
- Added ApplyBeforeBody option to POV component, to support working with FramingTransposer
- Added RectenterTarget to POV component
- Added OnTransitionFromCamera callback to extensions
- Added Damping to SameAsFollowTarget and HardLockToTarget components
- URP 7.1.3: added CinemachinePixelPerfect extension
- Added Speed Mode to AxisState, to support direct axis control without max speed
- New example scene: OverTheShoulderAim illustrating how to do over-the-shoulder TPS cam, with Normal and Aim modes
- Impulse Manager: added option to ignore timescale
- Framing Transposer: added OnTransition handling for camera rotation if InheritPosition
- Upgrade to support HDRP and Universal RP 7.0.0 API
- Upgrade to support HDRP and Universal RP 7.1.0 API
- Removed Resources diretories
- Sample scenes now available via package manager
- Added optional "Display Name" field to Cinemachine Shot in Timeline
- Added "Adopt Current Camera Settings" item to vcam inspector context menu
- Composer and FramingTransposer: allow the dead zone to extend to 2, and the Screen x,Y can range from -0.5 to 1.5
- HDRP: lens presets include physical settings if physical camera
- Regression Fix: Framing Transposer: ignore LookAt target.  Use Follow exclusively
- Bugfix: Framing Transposer was not handling dynamic changes to FOV properly
- Bugfix: PostProcessing extension was not handling standby update correctly when on Manager Vcams
- Bugfix: PostProcessing extension was leaking a smallamounts of memory when scenes were unloaded
- Bugfixes: (fogbugz 1193311, 1193307, 1192423, 1192414): disallow presets for vcams
- Bugfix: In some heading modes, FreeLook was improperly modifying the axes when activated
- Bugfix: Orbital transposer was improperly filtering the heading in TargetForward heading mode
- Bugfix: added EmbeddedAssetHelper null check
- Bugfix: composer screen guides drawn in correct place for physical camera
- Bugfix: FreeLook was not respecting wait time for X axis recentering
- Bugfix: FreeLook X axis was not always perfectly synched between rigs
- Bugfix (fogbugz 1176866): Collider: clean up static RigidBody on exit
- Bugfix (fogbugz 1174180): framing transposer wrong ortho size calculation
- Bugfix (fogbugz 1158509): Split brain.UpdateMethod into VcamUpdateMethod and BrainUpdateMethod, to make blending work correctly
- Bugfix (fogbugz 1162074): Framing transposer and group transposer only reached half maximum ortho size 
- Bugfix (fogbugz 1165599): Transposer: fix gimbal lock issue in LockToTargetWithWorldUp
- Bugfix: VolumeSettings: handle layermask in HDAdditionalCameraData
- Bugfix: use vcam's up when drawing gizmos (orbital transposer and free look)


## [2.3.4] - 2019-05-22
### PostProcessing V3 and bugfixes
- Added support for PostProcessing V3 - now called CinemachineVolumeSttings
- Added CinemachineCore.GetBlendOverride delegate to allow applications to override any vcam blend when it happens
- When a blend is cancelled by the opposite blend, reduce the blend time
- Orthographic cameras allow a Near Clip of 0
- Timeline won't auto-create CM brains when something dragged onto it
- Confiner: Improvement in automatic path invalidation when number of path points path changes
- Added CinemachineInpuitAxisDriver utility for overriding the default AxisState behaviour
- CinemachineCameraOffset: added customizable stage for when to apply the offset
- Added Loop option to BlendList Camera
- Improved Lookahead: does not automatically recenter
- Brain no longer applies time scaling to fixed delta
- Added dependency on Unity.ugui (2019.2 and up)
- Bugfix: potential endless loop when using Ignore tag in Collider
- Bugfix: Allow externally-driven FeeLook XAxis to work properly with SimpleFollow
- Bugfix: vcams with noise would sometimes show one noiseless frame when they were activated and standby update was not Always
- Bugfix: Generate a cut event if cutting to a blend-in-progess (fogbugz 1150847)
- Bugfix: reset lens shift if not physical camera
- Bugfix: Collider must consider actual target position, not lookahead position
- Bugfix: FreeLook heading RecenterNow was not working
- Bugfix: lookahead now takes the overridden Up into account
- Bugfix: screen composer guides drawn in wrong place for picture-in-picture
- Bugfix: FreeLook now draws only 1 active composer guide at a time (fogbugz 1138263)
- Bugfix: cameras sometimes snapped when interrupting blends
- Bugfix: Path handles no longer scale with the path object
- Bugfix: Framing Transposer Center on Activate was not working properly (fogbugz 1129824)
- Bugfix: FreeLook inherit position
- Bugfix: collider was pushing camera too far if there were multiple overlapping obstacles
- Bugfix: use IsAssignableFrom instead of IsSubclass in several places
- Bugfix: when interrupting a blend in progress, Cut was not respected
- Bugfix: collider minimum occlusion time and smoothing time interaction
- Bugfix: TargetGroup.RemoveMember error (fogbugz 1119028)
- Bugfix: TargetGroup member lerping jerk when member weight near 0
- Bugfix: Transposer angular damping should be 0 only if binding mode not LockToTarget

## [2.3.3] - 2019-01-08
### Temporary patch to get around a Unity bug in conditional dependencies
- Removed Cinemachine.Timeline namespace, as a workaround for fogbugz 1115321

## [2.3.1] - 2019-01-07
### Bugfixes
- Added timeline dependency
- OnTargetObjectWarped no longer generates garbage

## [2.3.0] - 2018-12-20
### Support for Unity 2019.1
- Added dependency on new unity.timeline
- Added conditional dependence on PostProcessingV2
- No copying CM gizmo into assets folder
- FreeLook: if inherit position from similar FreeLooks, bypass damping 
- Timeline: improve handling when vcam values are tweaked inside shot inspector (fogbugz 1109024)

## [2.2.8] - 2018-12-10
### Bugfixes, optimizations, and some experimental stuff
- Transposer: added Angular Damping Mode, to support quaternion calculations in gimbal-lock situations
- Framing Transposer and Group Transposer: group composing bugfixes, respect min/max limits
- Added ConemachineCameraOffset extension, to offset the camera a fixed distance at the end of the pipeline
- Dolly Cart: added support for LateUpdate
- State-driven-camera: added [NoSaveDuringPlay] to Animated Target and Layer Index
- Added AxisState.Recentering.RecenterNow() API call to skip wait time and start recentering now (if enabled)
- Added NoLens blend hint, to leave camera Lens settings alone
- Updated documentation (corrections, and relocation to prevent importing)
- Upgrade: added support for nested prefabs in Unity 2018.3 (fogbugz 1077395)
- Optimization: position predictor is more efficient
- Optimization: Composer caches some calculations 
- Optimization: Fix editor slowdown when Lens Presets asset is missing
- Experimental: Optional new damping algorithm: attempt to reduce sensitivity to variable framerate
- Experimental: Optional new extra-efficient versions of vcam and FreeLook (not back-compatible)
- Timeline: play/pause doesn't kick out the timeline vcam
- Path editor: make sure game view gets updated when a path waypoint is dragged in the scene view
- Composer guides are shown even if Camera is attached to a renderTexture
- Bugfix: allow impulse definition to be a non-public field (property drawer was complaining)
- Bugfix: added null check for when there is no active virtual camera
- Bugfix: CollisionImpulseSource typo in detection of 2D collider
- Bugfix: PasteComponentValues to prefab vcams and FreeLooks were corrupting scene and prefabs
- Bugfix: Timeline mixer was glitching for single frames at the end of blends
- Bugfix: Added OnTransitionFromCamera() to POV and OrbitalTransposer, to transition axes intelligently
- Regression fix: if no active vcam, don't set the Camera's transform

## [2.2.7] - 2018-07-24
### Mostly bugfixes
- Bugfix: fogbugz case 1053595: Cinemachine Collider leaves hidden collider at origin that interferes with scene objects
- Bugfix: fogbugz case 1063754: empty target group produces console messages
- Bugfix: FreeLook Paste Component Values now pastes the CM subcomponents as well
- Bugfix: added extra null checks to support cases where current vcam is dynamically deleted
- Bugfix: reset BlendList when enabled
- Regression fix: FreeLook axis values get transferred when similar vcams transition
- Bugfix: cutting to BlendList vcam sometimes produced a few bad frames
- Bugfix: smart update tracks the targets more efficiently and correctly, and supports RigidBody interpolation (2018.2 and up)
- Enhancement: POV component interprets POV as relative to parent transform if there is one
- API change: OnCameraLive and CameraActivated events take outgoing vcam also as parameter (may be null)

## [2.2.0] - 2018-06-18
### Impulse Module and More
- New Cinemachine Impulse module for event-driven camera shakes
- New Event Helper script CinemachineTriggerAction takes action on Collider and Collider2D enter/exit events, and exposes them as UnityEvents
- New performance-tuning feature: Standby Update.  Controls how often to update the vcam when it's in Standby.  
- New NoiseSettings editor with signal preview
- Added Focal Length or Named FOV presets for Camera Lens
- Added support for Physical Camera: focal length and Lens Offset
- New improved Group framing algorithm: tighter group framing in GroupComposer and FramingTransposer
- Collider: now returns TargetIsObscured if the target is offscreen (great for cameras with fixed aim)
- Collider: added Minimum Occlusion Time setting, to ignore fleeting obstructions
- Collider: added Transparent Layers mask, to specify solid objects that don't obstruct view
- Collider: damping will no longer take the camera through obstacles
- Collider: Added separate damping setting for when target is being occluded vs when camera is being returned to its normal position
- Collider: added Smoothing setting, to reduce camera jumpiness in environements with lots of obstacles
- NoiseSettings: added checkbox for pure sine-wave instead of Perlin wave
- If no LookAt target, PostProcessing FocusTracksTarget offset is relative to camera
- TrackedDolly: Default up mode sets Up to World Up
- Virtual Camera: New Transitions section in inspector that gives more control over blending:
  - Blend Hint provides some control over how the position and rotation are interpolated
  - Inherit Position checkbox to ensure smooth positional handoff from outgoing camera
  - OnCameraLive event gets fired when the camera activates.  Useful for custom handlers.
- Added ScreenSpaceAimWhenTargetsDiffer as a vcam blend hint.  This influences what happens when blending between vcams with different LookAt targets
- Increased stability of vcams with very small FOVs
- Framing Transposer no longer requires LookAt to be null
- LensSettings Aspect, Orthographic, IsPhysicalCamera, SensorSize properties no longer internal
- Noise Profiles: don't magically create assets.  Prompt user for filename and location of new or cloned profiles
- Refactored interaction between timeline and CM brain, to improve handling of edge cases (fogbugz case 1048497)
- Bugfix: StateDrivenCamera Editor was not finding states if target was OverrideController
- Bugfix when dragging orbital transposer transform: take bias into account
- Bugfix: SaveDuringPlay was not handling asset fields correctly - was sometimes crushing assets
- Bugfix: SimpleFollow transposers were not initilizing their position correctly at game start
- Bugfix: Timeline with CM shot was causing jitter in some FixedUpdate situations
- Bugfix: Multiple brains with heterogeneous update methods were not behaving correctly.  CM will now support this, but you must make sure that the brains have different layer masks.
- Example scenes now include use of CinemachineTriggerAction script.  

## [2.1.13] - 2018-05-09
### Removed dependency on nonexistant Timeline package, minor bugfixes
- Bugfix: Custom Blends "Any to Any" was not working (regression)
- Bugfix: Composer was sometimes getting wrong aspect if multiple brains with different aspect ratios
- Bugfix: could not drag vcam transforms if multiple inspectors and one is hidden
- Bugfix: Framing Transposer initializes in the wrong place - noticeable if dead zone

## [2.1.12] - 2018-02-26
### Storyboard, Bugfixes and other enhancements.  Also some restructuring for Package Manager
- Project restructure: Removed Base, Timeline, and PostFX folders from project root.  PostProcessing code must now be manually imported from Cinemachine menu.  No more dependencies on scripting defines.
- New Storyboard extension, to display images over the vcams.  Comes with a Waveform monitor window for color grading
- New option to specify vcam position blend style: linear, spherical, or cylindrical, based on LookAt target
- Added API to support seamless position warping of target objects: OnTargetObjectWarped().
- Added support for custom blend curves
- Lookahead: added Ignore Y Axis Movement option
- Added support for cascading blends (i.e. blending from mid-blend looks better)
- POV/Orbital/FreeLook axis: exposed Min, Max, and Wrap in the UI, for customized axis range
- FreeLook: added Y Axis recentering
- POV: Added recentering feature to both axes
- Path: Added Normalized Path units option: 0 is start of path, 1 is end.
- Path: added length display in inspector
- Timeline Clip Editor: vcam sections are now collapsible
- API enhancement: added Finalize to Pipeline stages, called even for manager-style vcams
- Bugfix: PostProcessing V2 DoF blending works better
- Bugfix: OrbitalTransposer works better with WorldUp overrides
- Bugfix: Remove StateDrivenCamera "not playing a controller" warning
- Bugfix: Handle exceptions thrown by assemblies that don't want to be introspected
- Bugfix: vcams following physics objects incorrectly snapped to origin after exiting play mode
- Bugfix: predictor now supports time pause
- Bugfix: Moved StartCoroutine in Brain to OnEnable()
- Bugfix: Collider was causing problems in Physics on Android platforms
- Bugfix: dragging a vcam's position updtaes prefabs properly
- Bugfix: All extension now respect the "enabled" checkbox
- Bugfix: Undo for Extasion add will no longer generate null references

## [2.1.10] - 2017-11-28
### This is the first UPM release of *Unity Package Cinemachine*.
- New Aim component: Same As Follow Target simply uses the same orientation as the Follow target
- Perlin Noise component: added inspector UI to clone or locate existing Noise profiles, and to create new ones
- Noise Presets were moved outside of the Examples folder
- Example Assets are now included as embedded package, not imported by default
- Bugfix: FreeLook with PositionDelta was not properly updating the heading
- Bugfix: Transitioning between FreeLooks simetimes caused a short camera freeze
- Bugfix: Added some null checks to FreeLook, to prevent error messages at build time

## [2.1.9] - 2017-11-17
### Initial version.
*Version 2.1.9 cloned from private development repository, corresponding to package released on the asset store*
