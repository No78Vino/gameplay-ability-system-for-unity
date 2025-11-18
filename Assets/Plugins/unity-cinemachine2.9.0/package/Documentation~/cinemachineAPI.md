
### AxisState

_Type:_ struct

_Namespace:_ Cinemachine


Axis state for defining how to react to player input.  The settings here control the responsiveness of the axis to player input.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **Value** | Single | The current value of the axis. |
| **m_MaxSpeed** | Single | The maximum speed of this axis in units/second. |
| **m_AccelTime** | Single | The amount of time in seconds it takes to accelerate to MaxSpeed with the supplied Axis at its maximum value. |
| **m_DecelTime** | Single | The amount of time in seconds it takes to decelerate the axis to zero if the supplied axis is in a neutral position. |
| **m_InputAxisName** | String | The name of this axis as specified in Unity Input manager.  Setting to an empty string will disable the automatic updating of this axis. |
| **m_InputAxisValue** | Single | The value of the input axis.  A value of 0 means no input.  You can drive this directly from a custom input system, or you can set the Axis Name and have the value driven by the internal Input Manager. |
| **m_InvertInput** | Boolean | If checked, then the raw value of the input axis will be inverted before it is used. |
| **m_MinValue** | Single | The minimum value for the axis. |
| **m_MaxValue** | Single | The maximum value for the axis. |
| **m_Wrap** | Boolean | If checked, then the axis will wrap around at the min/max values, forming a loop. |


#### Methods

``Void Validate()``

Call from OnValidate: Make sure the fields are sensible.

``Boolean Update(Single deltaTime)``

Updates the state of this axis based on the axis defined by AxisState.m_AxisName.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **deltaTime** | Single | Delta time in seconds. |

_Returns:_ Returns true if this axis' input was non-zero this Update, false otherwise.


### AxisState.Recentering

_Type:_ struct

_Namespace:_ Cinemachine


Helper for automatic axis recentering.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_enabled** | Boolean | If checked, will enable automatic recentering of the axis.  If unchecked, recenting is disabled. |
| **m_WaitTime** | Single | If no user input has been detected on the axis, the axis will wait this long in seconds before recentering. |
| **m_RecenteringTime** | Single | Maximum angular speed of recentering.  Will accelerate into and decelerate out of this. |


#### Methods

``Void Validate()``

Call this from OnValidate().

``Void CancelRecentering()``

Cancel any recentering in progress.

``Void DoRecentering(AxisState& axis, Single deltaTime, Single recenterTarget)``

Bring the axis back to the centered state.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **axis** | AxisState& |  |
| **deltaTime** | Single |  |
| **recenterTarget** | Single |  |



### CinemachineBasicMultiChannelPerlin

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


As a part of the Cinemachine Pipeline implementing the Noise stage, this component adds Perlin Noise to the Camera state, in the Correction channel of the CameraState.

The noise is created by using a predefined noise profile asset.  This defines the shape of the noise over time.  You can scale this in amplitude or in time, to produce a large family of different noises using the same profile.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ True if the component is valid, i.e.  it has a noise definition and is enabled. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Noise stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_NoiseProfile** | NoiseSettings | The asset containing the Noise Profile.  Define the frequencies and amplitudes there to make a characteristic noise profile.  Make your own or just use one of the many presets. |
| **m_AmplitudeGain** | Single | Gain to apply to the amplitudes defined in the NoiseSettings asset.  1 is normal.  Setting this to 0 completely mutes the noise. |
| **m_FrequencyGain** | Single | Scale factor to apply to the frequencies defined in the NoiseSettings asset.  1 is normal.  Larger magnitudes will make the noise shake more rapidly. |


#### Methods

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Applies noise to the Correction channel of the CameraState if the delta time is greater than 0.  Otherwise, does nothing.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | How much to advance the perlin noise generator.  Noise is only applied if this value is greater than or equal to 0. |



### CinemachineBlendDefinition

_Type:_ struct

_Namespace:_ Cinemachine


Definition of a Camera blend.  This struct holds the information necessary to generate a suitable AnimationCurve for a Cinemachine Blend.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **BlendCurve** | AnimationCurve | _[Get]_ A normalized AnimationCurve specifying the interpolation curve for this camera blend.  Y-axis values must be in range [0,1] (internally clamped within Blender) and time must be in range of [0, 1]. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Style** | Style | Shape of the blend curve.<br>_Possible Values:_<br>- **Cut**: Zero-length blend.<br>- **EaseInOut**: S-shaped curve, giving a gentle and smooth transition.<br>- **EaseIn**: Linear out of the outgoing shot, and easy into the incoming.<br>- **EaseOut**: Easy out of the outgoing shot, and linear into the incoming.<br>- **HardIn**: Easy out of the outgoing, and hard into the incoming.<br>- **HardOut**: Hard out of the outgoing, and easy into the incoming.<br>- **Linear**: Linear blend.  Mechanical-looking.<br>- **Custom**: Custom blend curve.<br> |
| **m_Time** | Single | Duration of the blend, in seconds. |
| **m_CustomCurve** | AnimationCurve | A user-defined AnimationCurve, used only if style is Custom.  Curve MUST be normalized, i.e.  time range [0...1], value range [0...1]. |



### CinemachineBlenderSettings

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ ScriptableObject


Asset that defines the rules for blending between Virtual Cameras.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_CustomBlends** | CustomBlend[] | The array containing explicitly defined blends between two Virtual Cameras. |


#### Methods

``AnimationCurve GetBlendCurveForVirtualCameras(String fromCameraName, String toCameraName, AnimationCurve defaultCurve)``

Attempts to find a blend curve which matches the to and from cameras as specified.  If no match is found, the function returns either the default blend for this Blender or NULL depending on the state of returnDefaultOnNoMatch.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **fromCameraName** | String | The game object name of the from camera. |
| **toCameraName** | String | The game object name of the to camera. |
| **defaultCurve** | AnimationCurve | Curve to return if no curve found.  Can be NULL. |



### CinemachineBlenderSettings.CustomBlend

_Type:_ struct

_Namespace:_ Cinemachine


Container specifying how two specific Cinemachine Virtual Cameras blend together.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_From** | String | When blending from this camera. |
| **m_To** | String | When blending to this camera. |
| **m_Blend** | CinemachineBlendDefinition | Blend curve definition. |



### CinemachineBlendListCamera

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineVirtualCameraBase

_Implements:_ ICinemachineCamera


This is a virtual camera "manager" that owns and manages a collection of child Virtual Cameras.  When the camera goes live, these child vcams are enabled, one after another, holding each camera for a designated time.  Blends between cameras are specified.  The last camera is held indefinitely.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **Description** | String | _[Get]_ Gets a brief debug description of this virtual camera, for use when displaying debug info. |
| **LiveChild** | ICinemachineCamera | _[Get,Set]_ Get the current "best" child virtual camera, that would be chosen if the State Driven Camera were active. |
| **LiveChildOrSelf** | ICinemachineCamera | _[Get]_ Return the live child. |
| **State** | CameraState | _[Get]_ The State of the current live child. |
| **LookAt** | Transform | _[Get,Set]_ Get the current LookAt target.  Returns parent's LookAt if parent is non-null and no specific LookAt defined for this camera. |
| **Follow** | Transform | _[Get,Set]_ Get the current Follow target.  Returns parent's Follow if parent is non-null and no specific Follow defined for this camera. |
| **ChildCameras** | CinemachineVirtualCameraBase[] | _[Get]_ The list of child cameras.  These are just the immediate children in the hierarchy. |
| **IsBlending** | Boolean | _[Get]_ Is there a blend in progress? |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_LookAt** | Transform | Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all of the children define targets of their own. |
| **m_Follow** | Transform | Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all of the children define targets of their own. |
| **m_ShowDebugText** | Boolean | When enabled, the current child camera and blend will be indicated in the game window, for debugging. |
| **m_EnableAllChildCameras** | Boolean | Force all child cameras to be enabled.  This is useful if animating them in Timeline, but consumes extra resources. |
| **m_ChildCameras** | CinemachineVirtualCameraBase[] | Internal API for the editor.  Do not use this field. |
| **m_Instructions** | Instruction[] | The set of instructions for enabling child cameras. |
| **CinemachineGUIDebuggerCallback** | Action | This is deprecated.  It is here to support the soon-to-be-removed Cinemachine Debugger in the Editor. |
| **m_ExcludedPropertiesInInspector** | String[] | Inspector control - Use for hiding sections of the Inspector UI. |
| **m_LockStageInInspector** | Stage[] | Inspector control - Use for enabling sections of the Inspector UI. |
| **m_Priority** | Int32 | The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority. |


#### Methods

``virtual Boolean IsLiveChild(ICinemachineCamera vcam)``

Check whether the vcam a live child of this camera.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | ICinemachineCamera | The Virtual Camera to check. |

_Returns:_ True if the vcam is currently actively influencing the state of this vcam.
``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the vcam that a target got warped, so that the vcam can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``virtual Void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, Single deltaTime)``

Notification that this virtual camera is going live.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **fromCam** | ICinemachineCamera | The camera being deactivated.  May be null. |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than or equal to 0). |

``virtual Void InternalUpdateCameraState(Vector3 worldUp, Single deltaTime)``

Called by CinemachineCore at designated update time so the vcam can position itself and track its targets.  This implementation updates all the children, chooses the best one, and implements any required blending.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than or equal to 0). |

``protected virtual Void OnEnable()``

Makes sure the internal child cache is up to date.

``Void OnTransformChildrenChanged()``

Makes sure the internal child cache is up to date.

``protected virtual Void OnGUI()``

Displays the current active camera on the game screen, if requested.

``Void ValidateInstructions()``

Internal API for the inspector editor.



### CinemachineBrain

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ MonoBehaviour


CinemachineBrain is the link between the Unity Camera and the Cinemachine Virtual Cameras in the scene.  It monitors the priority stack to choose the current Virtual Camera, and blend with another if necessary.  Finally and most importantly, it applies the Virtual Camera state to the attached Unity Camera.

The CinemachineBrain is also the place where rules for blending between virtual cameras are defined.  Camera blending is an interpolation over time of one virtual camera position and state to another.  If you think of virtual cameras as cameramen, then blending is a little like one cameraman smoothly passing the camera to another cameraman.  You can specify the time over which to blend, as well as the blend curve shape.  Note that a camera cut is just a zero-time blend.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **OutputCamera** | Camera | _[Get]_ Get the Unity Camera that is attached to this GameObject.  This is the camera that will be controlled by the brain. |
| **PostProcessingComponent** | Component | _[Get,Set]_ Internal support for opaque post-processing module. |
| **SoloCamera** | ICinemachineCamera | _(static)_ _[Get,Set]_ API for the Unity Editor.  Show this camera no matter what.  This is static, and so affects all Cinemachine brains. |
| **DefaultWorldUp** | Vector3 | _[Get]_ Get the default world up for the virtual cameras. |
| **IsBlending** | Boolean | _[Get]_ Is there a blend in progress? |
| **ActiveBlend** | CinemachineBlend | _[Get]_ Get the current blend in progress.  Returns null if none. |
| **ActiveVirtualCamera** | ICinemachineCamera | _[Get]_ Get the current active virtual camera. |
| **CurrentCameraState** | CameraState | _[Get]_ The current state applied to the unity camera (may be the result of a blend). |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_ShowDebugText** | Boolean | When enabled, the current camera and blend will be indicated in the game window, for debugging. |
| **m_ShowCameraFrustum** | Boolean | When enabled, the camera's frustum will be shown at all times in the scene view. |
| **m_IgnoreTimeScale** | Boolean | When enabled, the cameras will always respond in real-time to user input and damping, even if the game is running in slow motion. |
| **m_WorldUpOverride** | Transform | If set, this object's Y axis will define the worldspace Up vector for all the virtual cameras.  This is useful for instance in top-down game environments.  If not set, Up is worldspace Y.  Setting this appropriately is important, because Virtual Cameras don't like looking straight up or straight down. |
| **m_UpdateMethod** | UpdateMethod | Use FixedUpdate if all your targets are animated during FixedUpdate (e.g.  RigidBodies), LateUpdate if all your targets are animated during the normal Update loop, and SmartUpdate if you want Cinemachine to do the appropriate thing on a per-target basis.  SmartUpdate is the recommended setting.<br>_Possible Values:_<br>- **FixedUpdate**: Virtual cameras are updated in sync with the Physics module, in FixedUpdate.<br>- **LateUpdate**: Virtual cameras are updated in MonoBehaviour LateUpdate.<br>- **SmartUpdate**: Virtual cameras are updated according to how the target is updated.<br> |
| **m_DefaultBlend** | CinemachineBlendDefinition | The blend that is used in cases where you haven't explicitly defined a blend between two Virtual Cameras. |
| **m_CustomBlends** | CinemachineBlenderSettings | This is the asset that contains custom settings for blends between specific virtual cameras in your scene. |
| **m_CameraCutEvent** | BrainEvent | This event will fire whenever a virtual camera goes live and there is no blend. |
| **m_CameraActivatedEvent** | VcamEvent | This event will fire whenever a virtual camera goes live.  If a blend is involved, then the event will fire on the first frame of the blend. |


#### Methods

``static Color GetSoloGUIColor()``

API for the Unity Editor.

_Returns:_ Color used to indicate that a camera is in Solo mode.
``Boolean IsLive(ICinemachineCamera vcam)``

True if the ICinemachineCamera the current active camera, or part of a current blend, either directly or indirectly because its parents are live.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | ICinemachineCamera | The camera to test whether it is live. |

_Returns:_ True if the camera is live (directly or indirectly) or part of a blend in progress.


### CinemachineClearShot

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineVirtualCameraBase

_Implements:_ ICinemachineCamera


Cinemachine ClearShot is a "manager camera" that owns and manages a set of Virtual Camera gameObject children.  When Live, the ClearShot will check the children, and choose the one with the best quality shot and make it Live.

This can be a very powerful tool.  If the child cameras have CinemachineCollider extensions, they will analyze the scene for target obstructions, optimal target distance, and other items, and report their assessment of shot quality back to the ClearShot parent, who will then choose the best one.  You can use this to set up complex multi-camera coverage of a scene, and be assured that a clear shot of the target will always be available.

If multiple child cameras have the same shot quality, the one with the highest priority will be chosen.

You can also define custom blends between the ClearShot children.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **Description** | String | _[Get]_ Gets a brief debug description of this virtual camera, for use when displaying debug info. |
| **LiveChild** | ICinemachineCamera | _[Get,Set]_ Get the current "best" child virtual camera, that would be chosen if the ClearShot camera were active. |
| **State** | CameraState | _[Get]_ The CameraState of the currently live child. |
| **LiveChildOrSelf** | ICinemachineCamera | _[Get]_ Return the live child. |
| **LookAt** | Transform | _[Get,Set]_ Get the current LookAt target.  Returns parent's LookAt if parent is non-null and no specific LookAt defined for this camera. |
| **Follow** | Transform | _[Get,Set]_ Get the current Follow target.  Returns parent's Follow if parent is non-null and no specific Follow defined for this camera. |
| **IsBlending** | Boolean | _[Get]_ Is there a blend in progress? |
| **ChildCameras** | CinemachineVirtualCameraBase[] | _[Get]_ The list of child cameras.  These are just the immediate children in the hierarchy. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_LookAt** | Transform | Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all children specify targets of their own. |
| **m_Follow** | Transform | Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all children specify targets of their own. |
| **m_ShowDebugText** | Boolean | When enabled, the current child camera and blend will be indicated in the game window, for debugging. |
| **m_ChildCameras** | CinemachineVirtualCameraBase[] | Internal API for the editor.  Do not use this filed. |
| **m_ActivateAfter** | Single | Wait this many seconds before activating a new child camera. |
| **m_MinDuration** | Single | An active camera must be active for at least this many seconds. |
| **m_RandomizeChoice** | Boolean | If checked, camera choice will be randomized if multiple cameras are equally desirable.  Otherwise, child list order and child camera priority will be used. |
| **m_DefaultBlend** | CinemachineBlendDefinition | The blend which is used if you don't explicitly define a blend between two Virtual Cameras. |
| **m_CustomBlends** | CinemachineBlenderSettings | This is the asset which contains custom settings for specific blends. |
| **CinemachineGUIDebuggerCallback** | Action | This is deprecated.  It is here to support the soon-to-be-removed Cinemachine Debugger in the Editor. |
| **m_ExcludedPropertiesInInspector** | String[] | Inspector control - Use for hiding sections of the Inspector UI. |
| **m_LockStageInInspector** | Stage[] | Inspector control - Use for enabling sections of the Inspector UI. |
| **m_Priority** | Int32 | The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority. |


#### Methods

``virtual Boolean IsLiveChild(ICinemachineCamera vcam)``

Check whether the vcam a live child of this camera.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | ICinemachineCamera | The Virtual Camera to check. |

_Returns:_ True if the vcam is currently actively influencing the state of this vcam.
``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the vcam that a target got warped, so that the vcam can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``virtual Void InternalUpdateCameraState(Vector3 worldUp, Single deltaTime)``

Internal use only.  Called by CinemachineCore at designated update time so the vcam can position itself and track its targets.  This implementation updates all the children, chooses the best one, and implements any required blending.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than 0). |

``protected virtual Void OnEnable()``

Makes sure the internal child cache is up to date.

``Void OnTransformChildrenChanged()``

Makes sure the internal child cache is up to date.

``protected virtual Void OnGUI()``

Displays the current active camera on the game screen, if requested.

``Void ResetRandomization()``

If RandomizeChoice is enabled, call this to re-randomize the children next frame.  This is useful if you want to freshen up the shot.

``virtual Void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, Single deltaTime)``

Notification that this virtual camera is going live.  This implementation resets the child randomization.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **fromCam** | ICinemachineCamera | The camera being deactivated.  May be null. |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than or equal to 0). |



### CinemachineCollider

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineExtension


An add-on module for Cinemachine Virtual Camera that post-processes the final position of the virtual camera.  Based on the supplied settings, the Collider will attempt to preserve the line of sight with the LookAt target of the virtual camera by moving away from objects that will obstruct the view.

Additionally, the Collider can be used to assess the shot quality and report this as a field in the camera State.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **DebugPaths** | List`1 | _[Get]_ Inspector API for debugging collision resolution path. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_CollideAgainst** | LayerMask | The Unity layer mask against which the collider will raycast. |
| **m_IgnoreTag** | String | Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag. |
| **m_MinimumDistanceFromTarget** | Single | Obstacles closer to the target than this will be ignored. |
| **m_AvoidObstacles** | Boolean | When enabled, will attempt to resolve situations where the line of sight to the target is blocked by an obstacle. |
| **m_DistanceLimit** | Single | The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used. |
| **m_CameraRadius** | Single | Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera. |
| **m_Strategy** | ResolutionStrategy | The way in which the Collider will attempt to preserve sight of the target.<br>_Possible Values:_<br>- **PullCameraForward**<br>- **PreserveCameraHeight**<br>- **PreserveCameraDistance**<br> |
| **m_MaximumEffort** | Int32 | Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.  In most environments, 4 is enough. |
| **m_Damping** | Single | The gradualness of collision resolution.  Higher numbers will move the camera more gradually away from obstructions. |
| **m_OptimalTargetDistance** | Single | If greater than zero, a higher score will be given to shots when the target is closer to this distance.  Set this to zero to disable this feature. |


#### Methods

``Boolean IsTargetObscured(ICinemachineCamera vcam)``

See whether an object is blocking the camera's view of the target.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | ICinemachineCamera | The virtual camera in question.  This might be different from the virtual camera that owns the collider, in the event that the camera has children. |

_Returns:_ True if something is blocking the view.
``Boolean CameraWasDisplaced(CinemachineVirtualCameraBase vcam)``

See whether the virtual camera has been moved by the collider.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase | The virtual camera in question.  This might be different from the virtual camera that owns the collider, in the event that the camera has children. |

_Returns:_ True if the virtual camera has been displaced due to collision or target obstruction.
``protected virtual Void OnDestroy()``

Cleanup.

``protected virtual Void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, Stage stage, CameraState& state, Single deltaTime)``

Callback to to the collision resolution and shot evaluation.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase |  |
| **stage** | Stage |  |
| **state** | CameraState& |  |
| **deltaTime** | Single |  |



### CinemachineComposer

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


This is a CinemachineComponent in the Aim section of the component pipeline.  Its job is to aim the camera at the vcam's LookAt target object, with configurable offsets, damping, and composition rules.

The composer does not change the camera's position.  It will only pan and tilt the camera where it is, in order to get the desired framing.  To move the camera, you have to use the virtual camera's Body section.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a LookAt defined. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Aim stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |
| **TrackedPoint** | Vector3 | _[Get]_ Internal API for inspector. |
| **SoftGuideRect** | Rect | _[Get,Set]_ Internal API for the inspector editor. |
| **HardGuideRect** | Rect | _[Get,Set]_ Internal API for the inspector editor. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **OnGUICallback** | Action | Used by the Inspector Editor to display on-screen guides. |
| **m_TrackedObjectOffset** | Vector3 | Target offset from the target object's center in target-local space.  Use this to fine-tune the tracking target position when the desired area is not the tracked object's center. |
| **m_LookaheadTime** | Single | This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly. |
| **m_LookaheadSmoothing** | Single | Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag. |
| **m_LookaheadIgnoreY** | Boolean | If checked, movement along the Y axis will be ignored for lookahead calculations. |
| **m_HorizontalDamping** | Single | How aggressively the camera tries to follow the target in the screen-horizontal direction.  Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone.  Larger numbers give a more heavy slowly responding camera.  Using different vertical and horizontal settings can yield a wide range of camera behaviors. |
| **m_VerticalDamping** | Single | How aggressively the camera tries to follow the target in the screen-vertical direction.  Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone.  Larger numbers give a more heavy slowly responding camera.  Using different vertical and horizontal settings can yield a wide range of camera behaviors. |
| **m_ScreenX** | Single | Horizontal screen position for target.  The camera will rotate to position the tracked object here. |
| **m_ScreenY** | Single | Vertical screen position for target, The camera will rotate to position the tracked object here. |
| **m_DeadZoneWidth** | Single | Camera will not rotate horizontally if the target is within this range of the position. |
| **m_DeadZoneHeight** | Single | Camera will not rotate vertically if the target is within this range of the position. |
| **m_SoftZoneWidth** | Single | When target is within this region, camera will gradually rotate horizontally to re-align towards the desired position, depending on the damping speed. |
| **m_SoftZoneHeight** | Single | When target is within this region, camera will gradually rotate vertically to re-align towards the desired position, depending on the damping speed. |
| **m_BiasX** | Single | A non-zero bias will move the target position horizontally away from the center of the soft zone. |
| **m_BiasY** | Single | A non-zero bias will move the target position vertically away from the center of the soft zone. |


#### Methods

``protected virtual Vector3 GetLookAtPointAndSetTrackedPoint(Vector3 lookAt)``

Apply the target offsets to the target location.  Also set the TrackedPoint property, taking lookahead into account.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **lookAt** | Vector3 | The unoffset LookAt point. |

_Returns:_ The LookAt point with the offset applied.
``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the us that a target got warped, so that we can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``virtual Void PrePipelineMutateCameraState(CameraState& curState)``




| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& |  |

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Applies the composer rules and orients the camera accordingly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for calculating damping.  If less than zero, then target will snap to the center of the dead zone. |



### CinemachineConfiner

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineExtension


An add-on module for Cinemachine Virtual Camera that post-processes the final position of the virtual camera.  It will confine the virtual camera's position to the volume specified in the Bounding Volume field.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ Check if the bounding volume is defined. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_ConfineMode** | Mode | The confiner can operate using a 2D bounding shape or a 3D bounding volume.<br>_Possible Values:_<br>- **Confine2D**<br>- **Confine3D**<br> |
| **m_BoundingVolume** | Collider | The volume within which the camera is to be contained. |
| **m_BoundingShape2D** | Collider2D | The 2D shape within which the camera is to be contained. |
| **m_ConfineScreenEdges** | Boolean | If camera is orthographic, screen edges will be confined to the volume.  If not checked, then only the camera center will be confined. |
| **m_Damping** | Single | How gradually to return the camera to the bounding volume if it goes beyond the borders.  Higher numbers are more gradual. |


#### Methods

``Boolean CameraWasDisplaced(CinemachineVirtualCameraBase vcam)``

See whether the virtual camera has been moved by the confiner.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase | The virtual camera in question.  This might be different from the virtual camera that owns the confiner, in the event that the camera has children. |

_Returns:_ True if the virtual camera has been repositioned.
``protected virtual Void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, Stage stage, CameraState& state, Single deltaTime)``

Callback to to the camera confining.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase |  |
| **stage** | Stage |  |
| **state** | CameraState& |  |
| **deltaTime** | Single |  |

``Void InvalidatePathCache()``

Call this if the bounding shape's points change at runtime.



### CinemachineDollyCart

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ MonoBehaviour


This is a very simple behaviour that constrains its transform to a CinemachinePath.  It can be used to animate any objects along a path, or as a Follow target for Cinemachine Virtual Cameras.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Path** | CinemachinePathBase | The path to follow. |
| **m_UpdateMethod** | UpdateMethod | When to move the cart, if Velocity is non-zero.<br>_Possible Values:_<br>- **Update**<br>- **FixedUpdate**<br> |
| **m_PositionUnits** | PositionUnits | How to interpret the Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.<br>_Possible Values:_<br>- **PathUnits**<br>- **Distance**<br>- **Normalized**<br> |
| **m_Speed** | Single | Move the cart with this speed along the path.  The value is interpreted according to the Position Units setting. |
| **m_Position** | Single | The position along the path at which the cart will be placed.  This can be animated directly or, if the velocity is non-zero, will be updated automatically.  The value is interpreted according to the Position Units setting. |



### CinemachineExternalCamera

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineVirtualCameraBase

_Implements:_ ICinemachineCamera


This component will expose a non-cinemachine camera to the cinemachine system, allowing it to participate in blends.  Just add it as a component alongside an existing Unity Camera component.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **State** | CameraState | _[Get]_ Get the CameraState, as we are able to construct one from the Unity Camera. |
| **LookAt** | Transform | _[Get,Set]_ The object that the camera is looking at. |
| **Follow** | Transform | _[Get,Set]_ This vcam defines no targets. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_LookAt** | Transform | The object that the camera is looking at.  Setting this will improve the quality of the blends to and from this camera. |
| **m_PositionBlending** | PositionBlendMethod | Hint for blending positions to and from this virtual camera.<br>_Possible Values:_<br>- **Linear**<br>- **Spherical**<br>- **Cylindrical**<br> |
| **CinemachineGUIDebuggerCallback** | Action | This is deprecated.  It is here to support the soon-to-be-removed Cinemachine Debugger in the Editor. |
| **m_ExcludedPropertiesInInspector** | String[] | Inspector control - Use for hiding sections of the Inspector UI. |
| **m_LockStageInInspector** | Stage[] | Inspector control - Use for enabling sections of the Inspector UI. |
| **m_Priority** | Int32 | The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority. |


#### Methods

``virtual Void InternalUpdateCameraState(Vector3 worldUp, Single deltaTime)``

Internal use only.  Do not call this method.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 |  |
| **deltaTime** | Single |  |



### CinemachineFollowZoom

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineExtension


An add-on module for Cinemachine Virtual Camera that adjusts the FOV of the lens to keep the target object at a constant size on the screen, regardless of camera and target position.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Width** | Single | The shot width to maintain, in world units, at target distance. |
| **m_Damping** | Single | Increase this value to soften the aggressiveness of the follow-zoom.  Small numbers are more responsive, larger numbers give a more heavy slowly responding camera. |
| **m_MinFOV** | Single | Lower limit for the FOV that this behaviour will generate. |
| **m_MaxFOV** | Single | Upper limit for the FOV that this behaviour will generate. |


#### Methods

``protected virtual Void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, Stage stage, CameraState& state, Single deltaTime)``

Callback to preform the zoom adjustment.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase |  |
| **stage** | Stage |  |
| **state** | CameraState& |  |
| **deltaTime** | Single |  |



### CinemachineFramingTransposer

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


This is a Cinemachine Component in the Body section of the component pipeline.  Its job is to position the camera in a fixed screen-space relationship to the vcam's Follow target object, with offsets and damping.

The camera will be first moved along the camera Z axis until the Follow target is at the desired distance from the camera's X-Y plane.  The camera will then be moved in its XY plane until the Follow target is at the desired point on the camera's screen.

The FramingTansposer will only change the camera's position in space.  It will not re-orient or otherwise aim the camera.

For this component to work properly, the vcam's LookAt target must be null.  The Follow target will define what the camera is looking at.

If the Follow target is a CinemachineTargetGroup, then additional controls will be available to dynamically adjust the camera's view in order to frame the entire group.

Although this component was designed for orthographic cameras, it works equally well with perspective cameras and can be used in 3D environments.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **SoftGuideRect** | Rect | _[Get,Set]_ Internal API for the inspector editor. |
| **HardGuideRect** | Rect | _[Get,Set]_ Internal API for the inspector editor. |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a valid Follow target. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Body stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |
| **TrackedPoint** | Vector3 | _[Get]_ Internal API for inspector. |
| **m_LastBounds** | Bounds | _[Get]_ For editor visulaization of the calculated bounding box of the group. |
| **m_lastBoundsMatrix** | Matrix4x4 | _[Get]_ For editor visualization of the calculated bounding box of the group. |
| **TargetGroup** | CinemachineTargetGroup | _[Get]_ Get Follow target as CinemachineTargetGroup, or null if target is not a group. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **OnGUICallback** | Action | Used by the Inspector Editor to display on-screen guides. |
| **m_LookaheadTime** | Single | This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly. |
| **m_LookaheadSmoothing** | Single | Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag. |
| **m_LookaheadIgnoreY** | Boolean | If checked, movement along the Y axis will be ignored for lookahead calculations. |
| **m_XDamping** | Single | How aggressively the camera tries to maintain the offset in the X-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_YDamping** | Single | How aggressively the camera tries to maintain the offset in the Y-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_ZDamping** | Single | How aggressively the camera tries to maintain the offset in the Z-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_ScreenX** | Single | Horizontal screen position for target.  The camera will move to position the tracked object here. |
| **m_ScreenY** | Single | Vertical screen position for target, The camera will move to position the tracked object here. |
| **m_CameraDistance** | Single | The distance along the camera axis that will be maintained from the Follow target. |
| **m_DeadZoneWidth** | Single | Camera will not move horizontally if the target is within this range of the position. |
| **m_DeadZoneHeight** | Single | Camera will not move vertically if the target is within this range of the position. |
| **m_DeadZoneDepth** | Single | The camera will not move along its z-axis if the Follow target is within this distance of the specified camera distance. |
| **m_UnlimitedSoftZone** | Boolean | If checked, then soft zone will be unlimited in size. |
| **m_SoftZoneWidth** | Single | When target is within this region, camera will gradually move horizontally to re-align towards the desired position, depending on the damping speed. |
| **m_SoftZoneHeight** | Single | When target is within this region, camera will gradually move vertically to re-align towards the desired position, depending on the damping speed. |
| **m_BiasX** | Single | A non-zero bias will move the target position horizontally away from the center of the soft zone. |
| **m_BiasY** | Single | A non-zero bias will move the target position vertically away from the center of the soft zone. |
| **m_GroupFramingMode** | FramingMode | What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both.<br>_Possible Values:_<br>- **Horizontal**: Consider only the horizontal dimension.  Vertical framing is ignored.<br>- **Vertical**: Consider only the vertical dimension.  Horizontal framing is ignored.<br>- **HorizontalAndVertical**: The larger of the horizontal and vertical dimensions will dominate, to get the best fit.<br>- **None**: Don't do any framing adjustment.<br> |
| **m_AdjustmentMode** | AdjustmentMode | How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.<br>_Possible Values:_<br>- **ZoomOnly**<br>- **DollyOnly**<br>- **DollyThenZoom**<br> |
| **m_GroupFramingSize** | Single | The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc. |
| **m_MaxDollyIn** | Single | The maximum distance toward the target that this behaviour is allowed to move the camera. |
| **m_MaxDollyOut** | Single | The maximum distance away the target that this behaviour is allowed to move the camera. |
| **m_MinimumDistance** | Single | Set this to limit how close to the target the camera can get. |
| **m_MaximumDistance** | Single | Set this to limit how far from the target the camera can get. |
| **m_MinimumFOV** | Single | If adjusting FOV, will not set the FOV lower than this. |
| **m_MaximumFOV** | Single | If adjusting FOV, will not set the FOV higher than this. |
| **m_MinimumOrthoSize** | Single | If adjusting Orthographic Size, will not set it lower than this. |
| **m_MaximumOrthoSize** | Single | If adjusting Orthographic Size, will not set it higher than this. |


#### Methods

``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the us that a target got warped, so that we can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Positions the virtual camera according to the transposer rules.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for damping.  If less than 0, no damping is done. |



### CinemachineFreeLook

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineVirtualCameraBase

_Implements:_ ICinemachineCamera


A Cinemachine Camera geared towards a 3rd person camera experience.  The camera orbits around its subject with three separate camera rigs defining rings around the target.  Each rig has its own radius, height offset, composer, and lens settings.  Depending on the camera's position along the spline connecting these three rigs, these settings are interpolated to give the final camera position and state.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **RigNames** | String[] | _(static)_ _[Get]_ Names of the 3 child rigs. |
| **State** | CameraState | _[Get]_ The camera state, which will be a blend of the child rig states. |
| **LookAt** | Transform | _[Get,Set]_ Get the current LookAt target.  Returns parent's LookAt if parent is non-null and no specific LookAt defined for this camera. |
| **Follow** | Transform | _[Get,Set]_ Get the current Follow target.  Returns parent's Follow if parent is non-null and no specific Follow defined for this camera. |
| **LiveChildOrSelf** | ICinemachineCamera | _[Get]_ Returns the rig with the greatest weight. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_LookAt** | Transform | Object for the camera children to look at (the aim target). |
| **m_Follow** | Transform | Object for the camera children wants to move with (the body target). |
| **m_PositionBlending** | PositionBlendMethod | Hint for blending positions to and from this virtual camera.<br>_Possible Values:_<br>- **Linear**<br>- **Spherical**<br>- **Cylindrical**<br> |
| **m_CommonLens** | Boolean | If enabled, this lens setting will apply to all three child rigs, otherwise the child rig lens settings will be used. |
| **m_Lens** | LensSettings | Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active. |
| **m_YAxis** | AxisState | The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs. |
| **m_YAxisRecentering** | Recentering | Controls how automatic recentering of the Y axis is accomplished. |
| **m_XAxis** | AxisState | The Horizontal axis.  Value is -180...180.  This is passed on to the rigs' OrbitalTransposer component. |
| **m_Heading** | Heading | The definition of Forward.  Camera will follow behind. |
| **m_RecenterToTargetHeading** | Recentering | Controls how automatic recentering of the X axis is accomplished. |
| **m_BindingMode** | BindingMode | The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.<br>_Possible Values:_<br>- **LockToTargetOnAssign**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame at the moment when the virtual camera was enabled, or when the target was assigned.<br>- **LockToTargetWithWorldUp**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame, with the tilt and roll zeroed out.<br>- **LockToTargetNoRoll**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame, with the roll zeroed out.<br>- **LockToTarget**: Camera will be bound to the Follow target using the target's local frame.<br>- **WorldSpace**: Camera will be bound to the Follow target using a world space offset.<br>- **SimpleFollowWithWorldUp**: Offsets will be calculated relative to the target, using Camera-local axes.<br> |
| **m_SplineCurvature** | Single | Controls how taut is the line that connects the rigs' orbits, which determines final placement on the Y axis. |
| **m_Orbits** | Orbit[] | The radius and height of the three orbiting rigs. |
| **CinemachineGUIDebuggerCallback** | Action | This is deprecated.  It is here to support the soon-to-be-removed Cinemachine Debugger in the Editor. |
| **m_ExcludedPropertiesInInspector** | String[] | Inspector control - Use for hiding sections of the Inspector UI. |
| **m_LockStageInInspector** | Stage[] | Inspector control - Use for enabling sections of the Inspector UI. |
| **m_Priority** | Int32 | The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority. |


#### Methods

``protected virtual Void OnValidate()``

Enforce bounds for fields, when changed in inspector.

``CinemachineVirtualCamera GetRig(Int32 i)``

Get a child rig.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **i** | Int32 | Rig index.  Can be 0, 1, or 2. |

_Returns:_ The rig, or null if index is bad.
``protected virtual Void OnEnable()``

Updates the child rig cache.

``protected virtual Void OnDestroy()``

Makes sure that the child rigs get destroyed in an undo-firndly manner.  Invalidates the rig cache.

``virtual Boolean IsLiveChild(ICinemachineCamera vcam)``

Check whether the vcam a live child of this camera.  Returns true if the child is currently contributing actively to the camera state.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | ICinemachineCamera | The Virtual Camera to check. |

_Returns:_ True if the vcam is currently actively influencing the state of this vcam.
``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the vcam that a target got warped, so that the vcam can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``virtual Void InternalUpdateCameraState(Vector3 worldUp, Single deltaTime)``

Internal use only.  Called by CinemachineCore at designated update time so the vcam can position itself and track its targets.  All 3 child rigs are updated, and a blend calculated, depending on the value of the Y axis.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than 0). |

``virtual Void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, Single deltaTime)``

If we are transitioning from another FreeLook, grab the axis values from it.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **fromCam** | ICinemachineCamera | The camera being deactivated.  May be null. |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than or equal to 0). |

``Vector3 GetLocalPositionForCameraFromInput(Single t)``

Returns the local position of the camera along the spline used to connect the three camera rigs.  Does not take into account the current heading of the camera (or its target).


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **t** | Single | The t-value for the camera on its spline.  Internally clamped to the value [0,1]. |

_Returns:_ The local offset (back + up) of the camera WRT its target based on the supplied t-value.


### CinemachineGroupComposer

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComposer


This is a CinemachineComponent in the Aim section of the component pipeline.  Its job is to aim the camera at a target object, with configurable offsets, damping, and composition rules.

In addition, if the target is a CinemachineTargetGroup, the behaviour will adjust the FOV and the camera distance to ensure that the entire group of targets is framed properly.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **TargetGroup** | CinemachineTargetGroup | _[Get]_ Get LookAt target as CinemachineTargetGroup, or null if target is not a group. |
| **m_LastBounds** | Bounds | _[Get]_ For editor visulaization of the calculated bounding box of the group. |
| **m_lastBoundsMatrix** | Matrix4x4 | _[Get]_ For editor visualization of the calculated bounding box of the group. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_GroupFramingSize** | Single | The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc. |
| **m_FramingMode** | FramingMode | What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both.<br>_Possible Values:_<br>- **Horizontal**: Consider only the horizontal dimension.  Vertical framing is ignored.<br>- **Vertical**: Consider only the vertical dimension.  Horizontal framing is ignored.<br>- **HorizontalAndVertical**: The larger of the horizontal and vertical dimensions will dominate, to get the best fit.<br> |
| **m_FrameDamping** | Single | How aggressively the camera tries to frame the group.  Small numbers are more responsive, rapidly adjusting the camera to keep the group in the frame.  Larger numbers give a more heavy slowly responding camera. |
| **m_AdjustmentMode** | AdjustmentMode | How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.<br>_Possible Values:_<br>- **ZoomOnly**<br>- **DollyOnly**<br>- **DollyThenZoom**<br> |
| **m_MaxDollyIn** | Single | The maximum distance toward the target that this behaviour is allowed to move the camera. |
| **m_MaxDollyOut** | Single | The maximum distance away the target that this behaviour is allowed to move the camera. |
| **m_MinimumDistance** | Single | Set this to limit how close to the target the camera can get. |
| **m_MaximumDistance** | Single | Set this to limit how far from the target the camera can get. |
| **m_MinimumFOV** | Single | If adjusting FOV, will not set the FOV lower than this. |
| **m_MaximumFOV** | Single | If adjusting FOV, will not set the FOV higher than this. |
| **m_MinimumOrthoSize** | Single | If adjusting Orthographic Size, will not set it lower than this. |
| **m_MaximumOrthoSize** | Single | If adjusting Orthographic Size, will not set it higher than this. |
| **OnGUICallback** | Action | Used by the Inspector Editor to display on-screen guides. |
| **m_TrackedObjectOffset** | Vector3 | Target offset from the target object's center in target-local space.  Use this to fine-tune the tracking target position when the desired area is not the tracked object's center. |
| **m_LookaheadTime** | Single | This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly. |
| **m_LookaheadSmoothing** | Single | Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag. |
| **m_LookaheadIgnoreY** | Boolean | If checked, movement along the Y axis will be ignored for lookahead calculations. |
| **m_HorizontalDamping** | Single | How aggressively the camera tries to follow the target in the screen-horizontal direction.  Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone.  Larger numbers give a more heavy slowly responding camera.  Using different vertical and horizontal settings can yield a wide range of camera behaviors. |
| **m_VerticalDamping** | Single | How aggressively the camera tries to follow the target in the screen-vertical direction.  Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone.  Larger numbers give a more heavy slowly responding camera.  Using different vertical and horizontal settings can yield a wide range of camera behaviors. |
| **m_ScreenX** | Single | Horizontal screen position for target.  The camera will rotate to position the tracked object here. |
| **m_ScreenY** | Single | Vertical screen position for target, The camera will rotate to position the tracked object here. |
| **m_DeadZoneWidth** | Single | Camera will not rotate horizontally if the target is within this range of the position. |
| **m_DeadZoneHeight** | Single | Camera will not rotate vertically if the target is within this range of the position. |
| **m_SoftZoneWidth** | Single | When target is within this region, camera will gradually rotate horizontally to re-align towards the desired position, depending on the damping speed. |
| **m_SoftZoneHeight** | Single | When target is within this region, camera will gradually rotate vertically to re-align towards the desired position, depending on the damping speed. |
| **m_BiasX** | Single | A non-zero bias will move the target position horizontally away from the center of the soft zone. |
| **m_BiasY** | Single | A non-zero bias will move the target position vertically away from the center of the soft zone. |


#### Methods

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Applies the composer rules and orients the camera accordingly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for calculating damping.  If less than zero, then target will snap to the center of the dead zone. |



### CinemachineHardLockToTarget

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


This is a CinemachineComponent in the Aim section of the component pipeline.  Its job is to place the camera on the Follow Target.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a LookAt defined. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Aim stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |


#### Methods

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Applies the composer rules and orients the camera accordingly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for calculating damping.  If less than zero, then target will snap to the center of the dead zone. |



### CinemachineHardLookAt

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


This is a CinemachineComponent in the Aim section of the component pipeline.  Its job is to aim the camera hard at the LookAt target.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a LookAt defined. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Aim stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |


#### Methods

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Applies the composer rules and orients the camera accordingly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for calculating damping.  If less than zero, then target will snap to the center of the dead zone. |



### CinemachineMixingCamera

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineVirtualCameraBase

_Implements:_ ICinemachineCamera


CinemachineMixingCamera is a "manager camera" that takes on the state of the weighted average of the states of its child virtual cameras.

A fixed number of slots are made available for cameras, rather than a dynamic array.  We do it this way in order to support weight animation from the Timeline.  Timeline cannot animate array elements.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **State** | CameraState | _[Get]_ The blended CameraState. |
| **LookAt** | Transform | _[Get,Set]_ Not used. |
| **Follow** | Transform | _[Get,Set]_ Not used. |
| **LiveChildOrSelf** | ICinemachineCamera | _[Get]_ Return the live child. |
| **ChildCameras** | CinemachineVirtualCameraBase[] | _[Get]_ Get the cached list of child cameras.  These are just the immediate children in the hierarchy.  Note: only the first entries of this list participate in the final blend, up to MaxCameras. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Weight0** | Single | The weight of the first tracked camera. |
| **m_Weight1** | Single | The weight of the second tracked camera. |
| **m_Weight2** | Single | The weight of the third tracked camera. |
| **m_Weight3** | Single | The weight of the fourth tracked camera. |
| **m_Weight4** | Single | The weight of the fifth tracked camera. |
| **m_Weight5** | Single | The weight of the sixth tracked camera. |
| **m_Weight6** | Single | The weight of the seventh tracked camera. |
| **m_Weight7** | Single | The weight of the eighth tracked camera. |
| **CinemachineGUIDebuggerCallback** | Action | This is deprecated.  It is here to support the soon-to-be-removed Cinemachine Debugger in the Editor. |
| **m_ExcludedPropertiesInInspector** | String[] | Inspector control - Use for hiding sections of the Inspector UI. |
| **m_LockStageInInspector** | Stage[] | Inspector control - Use for enabling sections of the Inspector UI. |
| **m_Priority** | Int32 | The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority. |


#### Methods

``Single GetWeight(Int32 index)``

Get the weight of the child at an index.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **index** | Int32 | The child index.  Only immediate CinemachineVirtualCameraBase children are counted. |

_Returns:_ The weight of the camera.  Valid only if camera is active and enabled.
``Void SetWeight(Int32 index, Single w)``

Set the weight of the child at an index.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **index** | Int32 | The child index.  Only immediate CinemachineVirtualCameraBase children are counted. |
| **w** | Single | The weight to set.  Can be any non-negative number. |

``Single GetWeight(CinemachineVirtualCameraBase vcam)``

Get the weight of the child CinemachineVirtualCameraBase.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase | The child camera. |

_Returns:_ The weight of the camera.  Valid only if camera is active and enabled.
``Void SetWeight(CinemachineVirtualCameraBase vcam, Single w)``

Set the weight of the child CinemachineVirtualCameraBase.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase | The child camera. |
| **w** | Single | The weight to set.  Can be any non-negative number. |

``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the vcam that a target got warped, so that the vcam can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``protected virtual Void OnEnable()``

Makes sure the internal child cache is up to date.

``Void OnTransformChildrenChanged()``

Makes sure the internal child cache is up to date.

``protected virtual Void OnValidate()``

Makes sure the weights are non-negative.

``virtual Boolean IsLiveChild(ICinemachineCamera vcam)``

Check whether the vcam a live child of this camera.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | ICinemachineCamera | The Virtual Camera to check. |

_Returns:_ True if the vcam is currently actively influencing the state of this vcam.
``protected Void InvalidateListOfChildren()``

Invalidate the cached list of child cameras.

``protected Void ValidateListOfChildren()``

Rebuild the cached list of child cameras.

``virtual Void InternalUpdateCameraState(Vector3 worldUp, Single deltaTime)``

Internal use only.  Do not call this method.  Called by CinemachineCore at designated update time so the vcam can position itself and track its targets.  This implementation computes and caches the weighted blend of the tracked cameras.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than 0). |



### CinemachineOrbitalTransposer

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineTransposer


This is a CinemachineComponent in the Body section of the component pipeline.  Its job is to position the camera in a variable relationship to a the vcam's Follow target object, with offsets and damping.

This component is typically used to implement a camera that follows its target.  It can accept player input from an input device, which allows the player to dynamically control the relationship between the camera and the target, for example with a joystick.

The OrbitalTransposer introduces the concept of __Heading__, which is the direction in which the target is moving, and the OrbitalTransposer will attempt to position the camera in relationship to the heading, which is by default directly behind the target.  You can control the default relationship by adjusting the Heading Bias setting.

If you attach an input controller to the OrbitalTransposer, then the player can also control the way the camera positions itself in relation to the target heading.  This allows the camera to move to any spot on an orbit around the target.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Heading** | Heading | The definition of Forward.  Camera will follow behind. |
| **m_RecenterToTargetHeading** | Recentering | Automatic heading recentering.  The settings here defines how the camera will reposition itself in the absence of player input. |
| **m_XAxis** | AxisState | Heading Control.  The settings here control the behaviour of the camera in response to the player's input. |
| **m_HeadingIsSlave** | Boolean | Drive the x-axis setting programmatically.  Automatic heading updating will be disabled. |
| **m_BindingMode** | BindingMode | The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.<br>_Possible Values:_<br>- **LockToTargetOnAssign**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame at the moment when the virtual camera was enabled, or when the target was assigned.<br>- **LockToTargetWithWorldUp**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame, with the tilt and roll zeroed out.<br>- **LockToTargetNoRoll**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame, with the roll zeroed out.<br>- **LockToTarget**: Camera will be bound to the Follow target using the target's local frame.<br>- **WorldSpace**: Camera will be bound to the Follow target using a world space offset.<br>- **SimpleFollowWithWorldUp**: Offsets will be calculated relative to the target, using Camera-local axes.<br> |
| **m_FollowOffset** | Vector3 | The distance vector that the transposer will attempt to maintain from the Follow target. |
| **m_XDamping** | Single | How aggressively the camera tries to maintain the offset in the X-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_YDamping** | Single | How aggressively the camera tries to maintain the offset in the Y-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_ZDamping** | Single | How aggressively the camera tries to maintain the offset in the Z-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_PitchDamping** | Single | How aggressively the camera tries to track the target rotation's X angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |
| **m_YawDamping** | Single | How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |
| **m_RollDamping** | Single | How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |


#### Methods

``protected virtual Void OnValidate()``



``Single UpdateHeading(Single deltaTime, Vector3 up, AxisState& axis)``

Update the X axis and calculate the heading.  This can be called by a delegate with a custom axis.  Used for damping.  If less than 0, no damping is done.World Up, set by the CinemachineBrainAxis value.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **deltaTime** | Single | Used for damping.  If less than 0, no damping is done. |
| **up** | Vector3 | World Up, set by the CinemachineBrain. |
| **axis** | AxisState& |  |

_Returns:_ Axis value.
``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the us that a target got warped, so that we can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Positions the virtual camera according to the transposer rules.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for damping.  If less than 0, no damping is done. |



### CinemachineOrbitalTransposer.Heading

_Type:_ struct

_Namespace:_ Cinemachine


How the "forward" direction is defined.  Orbital offset is in relation to the forward direction.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Definition** | HeadingDefinition | How 'forward' is defined.  The camera will be placed by default behind the target.  PositionDelta will consider 'forward' to be the direction in which the target is moving.<br>_Possible Values:_<br>- **PositionDelta**: Target heading calculated from the difference between its position on the last update and current frame.<br>- **Velocity**: Target heading calculated from its Rigidbody's velocity.  If no Rigidbody exists, it will fall back to HeadingDerivationMode.PositionDelta.<br>- **TargetForward**: Target heading calculated from the Target Transform's euler Y angle.<br>- **WorldForward**: Default heading is a constant world space heading.<br> |
| **m_VelocityFilterStrength** | Int32 | Size of the velocity sampling window for target heading filter.  This filters out irregularities in the target's movement.  Used only if deriving heading from target's movement (PositionDelta or Velocity). |
| **m_Bias** | Single | Where the camera is placed when the X-axis value is zero.  This is a rotation in degrees around the Y axis.  When this value is 0, the camera will be placed behind the target.  Nonzero offsets will rotate the zero position around the target. |



### CinemachinePath

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachinePathBase


Defines a world-space path, consisting of an array of waypoints, each of which has position, tangent, and roll settings.  Bezier interpolation is performed between the waypoints, to get a smooth and continuous path.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **MinPos** | Single | _[Get]_ The minimum value for the path position. |
| **MaxPos** | Single | _[Get]_ The maximum value for the path position. |
| **Looped** | Boolean | _[Get]_ True if the path ends are joined to form a continuous loop. |
| **DistanceCacheSampleStepsPerSegment** | Int32 | _[Get]_ When calculating the distance cache, sample the path this many times between points. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Looped** | Boolean | If checked, then the path ends are joined to form a continuous loop. |
| **m_Waypoints** | Waypoint[] | The waypoints that define the path.  They will be interpolated using a bezier curve. |
| **m_Resolution** | Int32 | Path samples per waypoint.  This is used for calculating path distances. |
| **m_Appearance** | Appearance | The settings that control how the path will appear in the editor scene view. |


#### Methods

``virtual Vector3 EvaluatePosition(Single pos)``

Get a worldspace position of a point along the path.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **pos** | Single | Postion along the path.  Need not be normalized. |

_Returns:_ World-space position of the point along at path at pos.
``virtual Vector3 EvaluateTangent(Single pos)``

Get the tangent of the curve at a point along the path.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **pos** | Single | Postion along the path.  Need not be normalized. |

_Returns:_ World-space direction of the path tangent.  Length of the vector represents the tangent strength.
``virtual Quaternion EvaluateOrientation(Single pos)``

Get the orientation the curve at a point along the path.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **pos** | Single | Postion along the path.  Need not be normalized. |

_Returns:_ World-space orientation of the path, as defined by tangent, up, and roll.


### CinemachinePath.Waypoint

_Type:_ struct

_Namespace:_ Cinemachine


A waypoint along the path.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **position** | Vector3 | Position in path-local space. |
| **tangent** | Vector3 | Offset from the position, which defines the tangent of the curve at the waypoint.  The length of the tangent encodes the strength of the bezier handle.  The same handle is used symmetrically on both sides of the waypoint, to ensure smoothness. |
| **roll** | Single | Defines the role of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up. |



### CinemachinePathBase.Appearance

_Type:_ class

_Namespace:_ Cinemachine


This class holds the settings that control how the path will appear in the editor scene view.  The path is not visible in the game view.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **pathColor** | Color | The color of the path itself when it is active in the editor. |
| **inactivePathColor** | Color | The color of the path itself when it is inactive in the editor. |
| **width** | Single | The width of the railroad-tracks that are drawn to represent the path. |



### CinemachinePOV

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


This is a CinemachineComponent in the Aim section of the component pipeline.  Its job is to aim the camera in response to the user's mouse or joystick input.

The composer does not change the camera's position.  It will only pan and tilt the camera where it is, in order to get the desired framing.  To move the camera, you have to use the virtual camera's Body section.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a LookAt defined. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Aim stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_VerticalAxis** | AxisState | The Vertical axis.  Value is -90..90.  Controls the vertical orientation. |
| **m_VerticalRecentering** | Recentering | Controls how automatic recentering of the Vertical axis is accomplished. |
| **m_HorizontalAxis** | AxisState | The Horizontal axis.  Value is -180..180.  Controls the horizontal orientation. |
| **m_HorizontalRecentering** | Recentering | Controls how automatic recentering of the Horizontal axis is accomplished. |


#### Methods

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Applies the axis values and orients the camera accordingly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for calculating damping.  Not used. |



### CinemachineSameAsFollowTarget

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


This is a CinemachineComponent in the Aim section of the component pipeline.  Its job is to match the orientation of the Follow target.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a Follow target defined. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Aim stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |


#### Methods

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Orients the camera to match the Follow target's orientation.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Not used. |



### CinemachineSmoothPath

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachinePathBase


Defines a world-space path, consisting of an array of waypoints, each of which has position and roll settings.  Bezier interpolation is performed between the waypoints, to get a smooth and continuous path.  The path will pass through all waypoints, and (unlike CinemachinePath) first and second order continuity is guaranteed.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **MinPos** | Single | _[Get]_ The minimum value for the path position. |
| **MaxPos** | Single | _[Get]_ The maximum value for the path position. |
| **Looped** | Boolean | _[Get]_ True if the path ends are joined to form a continuous loop. |
| **DistanceCacheSampleStepsPerSegment** | Int32 | _[Get]_ When calculating the distance cache, sample the path this many times between points. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Looped** | Boolean | If checked, then the path ends are joined to form a continuous loop. |
| **m_Waypoints** | Waypoint[] | The waypoints that define the path.  They will be interpolated using a bezier curve. |
| **m_Resolution** | Int32 | Path samples per waypoint.  This is used for calculating path distances. |
| **m_Appearance** | Appearance | The settings that control how the path will appear in the editor scene view. |


#### Methods

``virtual Void InvalidateDistanceCache()``

Call this if the path changes in such a way as to affect distances or other cached path elements.

``virtual Vector3 EvaluatePosition(Single pos)``

Get a worldspace position of a point along the path.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **pos** | Single | Postion along the path.  Need not be normalized. |

_Returns:_ World-space position of the point along at path at pos.
``virtual Vector3 EvaluateTangent(Single pos)``

Get the tangent of the curve at a point along the path.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **pos** | Single | Postion along the path.  Need not be normalized. |

_Returns:_ World-space direction of the path tangent.  Length of the vector represents the tangent strength.
``virtual Quaternion EvaluateOrientation(Single pos)``

Get the orientation the curve at a point along the path.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **pos** | Single | Postion along the path.  Need not be normalized. |

_Returns:_ World-space orientation of the path, as defined by tangent, up, and roll.


### CinemachineSmoothPath.Waypoint

_Type:_ struct

_Namespace:_ Cinemachine


A waypoint along the path.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **position** | Vector3 | Position in path-local space. |
| **roll** | Single | Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up. |



### CinemachineStateDrivenCamera

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineVirtualCameraBase

_Implements:_ ICinemachineCamera


This is a virtual camera "manager" that owns and manages a collection of child Virtual Cameras.  These child vcams are mapped to individual states in an animation state machine, allowing you to associate specific vcams to specific animation states.  When that state is active in the state machine, then the associated camera will be activated.

You can define custom blends and transitions between child cameras.

In order to use this behaviour, you must have an animated target (i.e.  an object animated with a state machine) to drive the behaviour.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **Description** | String | _[Get]_ Gets a brief debug description of this virtual camera, for use when displaying debug info. |
| **LiveChild** | ICinemachineCamera | _[Get,Set]_ Get the current "best" child virtual camera, that would be chosen if the State Driven Camera were active. |
| **LiveChildOrSelf** | ICinemachineCamera | _[Get]_ Return the live child. |
| **State** | CameraState | _[Get]_ The State of the current live child. |
| **LookAt** | Transform | _[Get,Set]_ Get the current LookAt target.  Returns parent's LookAt if parent is non-null and no specific LookAt defined for this camera. |
| **Follow** | Transform | _[Get,Set]_ Get the current Follow target.  Returns parent's Follow if parent is non-null and no specific Follow defined for this camera. |
| **ChildCameras** | CinemachineVirtualCameraBase[] | _[Get]_ The list of child cameras.  These are just the immediate children in the hierarchy. |
| **IsBlending** | Boolean | _[Get]_ Is there a blend in progress? |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_LookAt** | Transform | Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all of the children define targets of their own. |
| **m_Follow** | Transform | Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all of the children define targets of their own. |
| **m_AnimatedTarget** | Animator | The state machine whose state changes will drive this camera's choice of active child. |
| **m_LayerIndex** | Int32 | Which layer in the target state machine to observe. |
| **m_ShowDebugText** | Boolean | When enabled, the current child camera and blend will be indicated in the game window, for debugging. |
| **m_EnableAllChildCameras** | Boolean | Force all child cameras to be enabled.  This is useful if animating them in Timeline, but consumes extra resources. |
| **m_ChildCameras** | CinemachineVirtualCameraBase[] | Internal API for the editor.  Do not use this field. |
| **m_Instructions** | Instruction[] | The set of instructions associating virtual cameras with states.  These instructions are used to choose the live child at any given moment. |
| **m_DefaultBlend** | CinemachineBlendDefinition | The blend which is used if you don't explicitly define a blend between two Virtual Camera children. |
| **m_CustomBlends** | CinemachineBlenderSettings | This is the asset which contains custom settings for specific child blends. |
| **m_ParentHash** | ParentHash[] | Internal API for the Inspector editor. |
| **CinemachineGUIDebuggerCallback** | Action | This is deprecated.  It is here to support the soon-to-be-removed Cinemachine Debugger in the Editor. |
| **m_ExcludedPropertiesInInspector** | String[] | Inspector control - Use for hiding sections of the Inspector UI. |
| **m_LockStageInInspector** | Stage[] | Inspector control - Use for enabling sections of the Inspector UI. |
| **m_Priority** | Int32 | The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority. |


#### Methods

``virtual Boolean IsLiveChild(ICinemachineCamera vcam)``

Check whether the vcam a live child of this camera.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | ICinemachineCamera | The Virtual Camera to check. |

_Returns:_ True if the vcam is currently actively influencing the state of this vcam.
``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the vcam that a target got warped, so that the vcam can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``virtual Void InternalUpdateCameraState(Vector3 worldUp, Single deltaTime)``

Internal use only.  Do not call this method.  Called by CinemachineCore at designated update time so the vcam can position itself and track its targets.  This implementation updates all the children, chooses the best one, and implements any required blending.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 | Default world Up, set by the CinemachineBrain. |
| **deltaTime** | Single | Delta time for time-based effects (ignore if less than or equal to 0). |

``protected virtual Void OnEnable()``

Makes sure the internal child cache is up to date.

``Void OnTransformChildrenChanged()``

Makes sure the internal child cache is up to date.

``protected virtual Void OnGUI()``

Displays the current active camera on the game screen, if requested.

``static String CreateFakeHashName(Int32 parentHash, String stateName)``

API for the inspector editor.  Animation module does not have hashes for state parents, so we have to invent them in order to implement nested state handling.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **parentHash** | Int32 |  |
| **stateName** | String |  |

``Void ValidateInstructions()``

Internal API for the inspector editor.



### CinemachineStoryboard

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineExtension


An add-on module for Cinemachine Virtual Camera that places an image in screen space over the camera's output.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_ShowImage** | Boolean | If checked, the specified image will be displayed as an overlay over the virtual camera's output. |
| **m_Image** | Texture | The image to display. |
| **m_Aspect** | FillStrategy | How to handle differences between image aspect and screen aspect.<br>_Possible Values:_<br>- **BestFit**<br>- **CropImageToFit**<br>- **StretchToFit**<br> |
| **m_Alpha** | Single | The opacity of the image.  0 is transparent, 1 is opaque. |
| **m_Center** | Vector2 | The screen-space position at which to display the image.  Zero is center. |
| **m_Rotation** | Vector3 | The screen-space rotation to apply to the image. |
| **m_Scale** | Vector2 | The screen-space scaling to apply to the image. |
| **m_SyncScale** | Boolean | If checked, X and Y scale are synchronized. |
| **m_MuteCamera** | Boolean | If checked, Camera transform will not be controlled by this virtual camera. |
| **m_SplitView** | Single | Wipe the image on and off horizontally. |


#### Methods

``protected virtual Void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, Stage stage, CameraState& state, Single deltaTime)``

Standard CinemachineExtension callback.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **vcam** | CinemachineVirtualCameraBase |  |
| **stage** | Stage |  |
| **state** | CameraState& |  |
| **deltaTime** | Single |  |

``protected virtual Void OnDestroy()``



``protected virtual Void ConnectToVcam(Boolean connect)``




| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **connect** | Boolean |  |



### CinemachineTargetGroup

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ MonoBehaviour


Defines a group of target objects, each with a radius and a weight.  The weight is used when calculating the average position of the target group.  Higher-weighted members of the group will count more.  The bounding box is calculated by taking the member positions, weight, and radii into account.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **BoundingBox** | Bounds | _[Get]_ The axis-aligned bounding box of the group, computed using the targets positions and radii. |
| **IsEmpty** | Boolean | _[Get]_ Return true if there are no members with weight > 0. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_PositionMode** | PositionMode | How the group's position is calculated.  Select GroupCenter for the center of the bounding box, and GroupAverage for a weighted average of the positions of the members.<br>_Possible Values:_<br>- **GroupCenter**: Group position will be the center of the group's axis-aligned bounding box.<br>- **GroupAverage**: Group position will be the weighted average of the positions of the members.<br> |
| **m_RotationMode** | RotationMode | How the group's rotation is calculated.  Select Manual to use the value in the group's transform, and GroupAverage for a weighted average of the orientations of the members.<br>_Possible Values:_<br>- **Manual**: Manually set in the group's transform.<br>- **GroupAverage**: Weighted average of the orientation of its members.<br> |
| **m_UpdateMethod** | UpdateMethod | When to update the group's transform based on the position of the group members.<br>_Possible Values:_<br>- **Update**<br>- **FixedUpdate**<br>- **LateUpdate**<br> |
| **m_Targets** | Target[] | The target objects, together with their weights and radii, that will contribute to the group's average position, orientation, and size. |


#### Methods

``Bounds GetViewSpaceBoundingBox(Matrix4x4 mView)``

The axis-aligned bounding box of the group, in a specific reference frame.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **mView** | Matrix4x4 | The frame of reference in which to compute the bounding box. |

_Returns:_ The axis-aligned bounding box of the group, in the desired frame of reference.


### CinemachineTargetGroup.Target

_Type:_ struct

_Namespace:_ Cinemachine


Holds the information that represents a member of the group.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The target objects.  This object's position and orientation will contribute to the group's average position and orientation, in accordance with its weight. |
| **weight** | Single | How much weight to give the target when averaging.  Cannot be negative. |
| **radius** | Single | The radius of the target, used for calculating the bounding box.  Cannot be negative. |



### CinemachineTrackedDolly

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


A Cinemachine Virtual Camera Body component that constrains camera motion to a CinemachinePath.  The camera can move along the path.

This behaviour can operate in two modes: manual positioning, and Auto-Dolly positioning.  In Manual mode, the camera's position is specified by animating the Path Position field.  In Auto-Dolly mode, the Path Position field is animated automatically every frame by finding the position on the path that's closest to the virtual camera's Follow target.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a path. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Body stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Path** | CinemachinePathBase | The path to which the camera will be constrained.  This must be non-null. |
| **m_PathPosition** | Single | The position along the path at which the camera will be placed.  This can be animated directly, or set automatically by the Auto-Dolly feature to get as close as possible to the Follow target.  The value is interpreted according to the Position Units setting. |
| **m_PositionUnits** | PositionUnits | How to interpret Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.<br>_Possible Values:_<br>- **PathUnits**<br>- **Distance**<br>- **Normalized**<br> |
| **m_PathOffset** | Vector3 | Where to put the camera relative to the path position.  X is perpendicular to the path, Y is up, and Z is parallel to the path.  This allows the camera to be offset from the path itself (as if on a tripod, for example). |
| **m_XDamping** | Single | How aggressively the camera tries to maintain its position in a direction perpendicular to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_YDamping** | Single | How aggressively the camera tries to maintain its position in the path-local up direction.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_ZDamping** | Single | How aggressively the camera tries to maintain its position in a direction parallel to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_CameraUp** | CameraUpMode | How to set the virtual camera's Up vector.  This will affect the screen composition, because the camera Aim behaviours will always try to respect the Up direction.<br>_Possible Values:_<br>- **Default**: Leave the camera's up vector alone.  It will be set according to the Brain's WorldUp.<br>- **Path**: Take the up vector from the path's up vector at the current point.<br>- **PathNoRoll**: Take the up vector from the path's up vector at the current point, but with the roll zeroed out.<br>- **FollowTarget**: Take the up vector from the Follow target's up vector.<br>- **FollowTargetNoRoll**: Take the up vector from the Follow target's up vector, but with the roll zeroed out.<br> |
| **m_PitchDamping** | Single | How aggressively the camera tries to track the target rotation's X angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |
| **m_YawDamping** | Single | How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |
| **m_RollDamping** | Single | How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |
| **m_AutoDolly** | AutoDolly | Controls how automatic dollying occurs.  A Follow target is necessary to use this feature. |


#### Methods

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Positions the virtual camera according to the transposer rules.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for damping.  If less that 0, no damping is done. |



### CinemachineTrackedDolly.AutoDolly

_Type:_ struct

_Namespace:_ Cinemachine


Controls how automatic dollying occurs.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Enabled** | Boolean | If checked, will enable automatic dolly, which chooses a path position that is as close as possible to the Follow target.  Note: this can have significant performance impact. |
| **m_PositionOffset** | Single | Offset, in current position units, from the closest point on the path to the follow target. |
| **m_SearchRadius** | Int32 | Search up to how many waypoints on either side of the current position.  Use 0 for Entire path. |
| **m_SearchResolution** | Int32 | We search between waypoints by dividing the segment into this many straight pieces.  The higher the number, the more accurate the result, but performance is proportionally slower for higher numbers. |



### CinemachineTransposer

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineComponentBase


This is a CinemachineComponent in the Body section of the component pipeline.  Its job is to position the camera in a fixed relationship to the vcam's Follow target object, with offsets and damping.

The Transposer will only change the camera's position in space.  It will not re-orient or otherwise aim the camera.  To that, you need to instruct the vcam in the Aim section of its pipeline.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **EffectiveOffset** | Vector3 | _[Get]_ Get the target offset, with sanitization. |
| **IsValid** | Boolean | _[Get]_ True if component is enabled and has a valid Follow target. |
| **Stage** | Stage | _[Get]_ Get the Cinemachine Pipeline stage that this component implements.  Always returns the Body stage.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_BindingMode** | BindingMode | The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.<br>_Possible Values:_<br>- **LockToTargetOnAssign**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame at the moment when the virtual camera was enabled, or when the target was assigned.<br>- **LockToTargetWithWorldUp**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame, with the tilt and roll zeroed out.<br>- **LockToTargetNoRoll**: Camera will be bound to the Follow target using a frame of reference consisting of the target's local frame, with the roll zeroed out.<br>- **LockToTarget**: Camera will be bound to the Follow target using the target's local frame.<br>- **WorldSpace**: Camera will be bound to the Follow target using a world space offset.<br>- **SimpleFollowWithWorldUp**: Offsets will be calculated relative to the target, using Camera-local axes.<br> |
| **m_FollowOffset** | Vector3 | The distance vector that the transposer will attempt to maintain from the Follow target. |
| **m_XDamping** | Single | How aggressively the camera tries to maintain the offset in the X-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_YDamping** | Single | How aggressively the camera tries to maintain the offset in the Y-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_ZDamping** | Single | How aggressively the camera tries to maintain the offset in the Z-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors. |
| **m_PitchDamping** | Single | How aggressively the camera tries to track the target rotation's X angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |
| **m_YawDamping** | Single | How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |
| **m_RollDamping** | Single | How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera. |


#### Methods

``protected virtual Void OnValidate()``

Derived classes should call this from their OnValidate() implementation.

``virtual Void MutateCameraState(CameraState& curState, Single deltaTime)``

Positions the virtual camera according to the transposer rules.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& | The current camera state. |
| **deltaTime** | Single | Used for damping.  If less than 0, no damping is done. |

``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the us that a target got warped, so that we can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |

``protected Void InitPrevFrameStateInfo(CameraState& curState, Single deltaTime)``

Initializes the state for previous frame if appropriate.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **curState** | CameraState& |  |
| **deltaTime** | Single |  |

``protected Void TrackTarget(Single deltaTime, Vector3 up, Vector3 desiredCameraOffset, Vector3& outTargetPosition, Quaternion& outTargetOrient)``

Positions the virtual camera according to the transposer rules.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **deltaTime** | Single | Used for damping.  If less than 0, no damping is done. |
| **up** | Vector3 | Current camera up. |
| **desiredCameraOffset** | Vector3 | Where we want to put the camera relative to the follow target. |
| **outTargetPosition** | Vector3& | Resulting camera position. |
| **outTargetOrient** | Quaternion& | Damped target orientation. |

``Vector3 GeTargetCameraPosition(Vector3 worldUp)``

Internal API for the Inspector Editor, so it can draw a marker at the target.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 |  |

``Quaternion GetReferenceOrientation(Vector3 worldUp)``

Internal API for the Inspector Editor, so it can draw a marker at the target.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 |  |



### CinemachineVirtualCamera

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ CinemachineVirtualCameraBase

_Implements:_ ICinemachineCamera


This behaviour is intended to be attached to an empty Transform GameObject, and it represents a Virtual Camera within the Unity scene.

The Virtual Camera will animate its Transform according to the rules contained in its CinemachineComponent pipeline (Aim, Body, and Noise).  When the virtual camera is Live, the Unity camera will assume the position and orientation of the virtual camera.

A virtual camera is not a camera.  Instead, it can be thought of as a camera controller, not unlike a cameraman.  It can drive the Unity Camera and control its position, orientation, lens settings, and PostProcessing effects.  Each Virtual Camera owns its own Cinemachine Component Pipeline, through which you provide the instructions for dynamically tracking specific game objects.

A virtual camera is very lightweight, and does no rendering of its own.  It merely tracks interesting GameObjects, and positions itself accordingly.  A typical game can have dozens of virtual cameras, each set up to follow a particular character or capture a particular event.

A Virtual Camera can be in any of three states:

* **Live**: The virtual camera is actively controlling the Unity Camera.  The virtual camera is tracking its targets and being updated every frame. 
* **Standby**: The virtual camera is tracking its targets and being updated every frame, but no Unity Camera is actively being controlled by it.  This is the state of a virtual camera that is enabled in the scene but perhaps at a lower priority than the Live virtual camera. 
* **Disabled**: The virtual camera is present but disabled in the scene.  It is not actively tracking its targets and so consumes no processing power.  However, the virtual camera can be made live from the Timeline.

The Unity Camera can be driven by any virtual camera in the scene.  The game logic can choose the virtual camera to make live by manipulating the virtual cameras' enabled flags and their priorities, based on game logic.

In order to be driven by a virtual camera, the Unity Camera must have a CinemachineBrain behaviour, which will select the most eligible virtual camera based on its priority or on other criteria, and will manage blending.

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **State** | CameraState | _[Get]_ The CameraState object holds all of the information necessary to position the Unity camera.  It is the output of this class. |
| **LookAt** | Transform | _[Get,Set]_ Get the LookAt target for the Aim component in the CinemachinePipeline.  If this vcam is a part of a meta-camera collection, then the owner's target will be used if the local target is null. |
| **Follow** | Transform | _[Get,Set]_ Get the Follow target for the Body component in the CinemachinePipeline.  If this vcam is a part of a meta-camera collection, then the owner's target will be used if the local target is null. |
| **UserIsDragging** | Boolean | _[Get,Set]_ API for the editor, to make the dragging of position handles behave better. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_LookAt** | Transform | The object that the camera wants to look at (the Aim target).  If this is null, then the vcam's Transform orientation will define the camera's orientation. |
| **m_Follow** | Transform | The object that the camera wants to move with (the Body target).  If this is null, then the vcam's Transform position will define the camera's position. |
| **m_PositionBlending** | PositionBlendMethod | Hint for blending positions to and from this virtual camera.<br>_Possible Values:_<br>- **Linear**<br>- **Spherical**<br>- **Cylindrical**<br> |
| **m_Lens** | LensSettings | Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active. |
| **CinemachineGUIDebuggerCallback** | Action | This is deprecated.  It is here to support the soon-to-be-removed Cinemachine Debugger in the Editor. |
| **m_ExcludedPropertiesInInspector** | String[] | Inspector control - Use for hiding sections of the Inspector UI. |
| **m_LockStageInInspector** | Stage[] | Inspector control - Use for enabling sections of the Inspector UI. |
| **m_Priority** | Int32 | The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority. |


#### Methods

``virtual Void InternalUpdateCameraState(Vector3 worldUp, Single deltaTime)``

Internal use only.  Do not call this method.  Called by CinemachineCore at the appropriate Update time so the vcam can position itself and track its targets.  This class will invoke its pipeline and generate a CameraState for this frame.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **worldUp** | Vector3 |  |
| **deltaTime** | Single |  |

``protected virtual Void OnEnable()``

Make sure that the pipeline cache is up-to-date.

``protected virtual Void OnDestroy()``

Calls the DestroyPipelineDelegate for destroying the hidden child object, to support undo.

``protected virtual Void OnValidate()``

Enforce bounds for fields, when changed in inspector.

``Void InvalidateComponentPipeline()``

Editor API: Call this when changing the pipeline from the editor.  Will force a rebuild of the pipeline cache.

``Transform GetComponentOwner()``

Get the hidden CinemachinePipeline child object.

``CinemachineComponentBase[] GetComponentPipeline()``

Get the component pipeline owned by the hidden child pipeline container.  For most purposes, it is preferable to use the GetCinemachineComponent method.

``CinemachineComponentBase GetCinemachineComponent(Stage stage)``

Get the component set for a specific stage.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **stage** | Stage | The stage for which we want the component.<br>_Possible Values:_<br>- **Body**<br>- **Aim**<br>- **Noise**<br>- **Finalize**<br> |

_Returns:_ The Cinemachine component for that stage, or null if not defined.
``T GetCinemachineComponent[T]()``

Get an existing component of a specific type from the cinemachine pipeline.

``T AddCinemachineComponent[T]()``

Add a component to the cinemachine pipeline.

``Void DestroyCinemachineComponent[T]()``

Remove a component from the cinemachine pipeline.

``virtual Void OnTargetObjectWarped(Transform target, Vector3 positionDelta)``

This is called to notify the vcam that a target got warped, so that the vcam can update its internal state to make the camera also warp seamlessly.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **target** | Transform | The object that was warped. |
| **positionDelta** | Vector3 | The amount the target's position changed. |



### LensSettings

_Type:_ struct

_Namespace:_ Cinemachine


Describes the FOV and clip planes for a camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **FieldOfView** | Single | This is the camera view in vertical degrees.  For cinematic people, a 50mm lens on a super-35mm sensor would equal a 19.6 degree FOV. |
| **OrthographicSize** | Single | When using an orthographic camera, this defines the half-height, in world coordinates, of the camera view. |
| **NearClipPlane** | Single | This defines the near region in the renderable range of the camera frustum.  Raising this value will stop the game from drawing things near the camera, which can sometimes come in handy.  Larger values will also increase your shadow resolution. |
| **FarClipPlane** | Single | This defines the far region of the renderable range of the camera frustum.  Typically you want to set this value as low as possible without cutting off desired distant objects. |
| **Dutch** | Single | Camera Z roll, or tilt, in degrees. |


#### Methods

``static LensSettings FromCamera(Camera fromCamera)``

Creates a new LensSettings, copying the values from the supplied Camera.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **fromCamera** | Camera | The Camera from which the FoV, near and far clip planes will be copied. |

``static LensSettings Lerp(LensSettings lensA, LensSettings lensB, Single t)``

Linearly blends the fields of two LensSettings and returns the result.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **lensA** | LensSettings | The LensSettings to blend from. |
| **lensB** | LensSettings | The LensSettings to blend to. |
| **t** | Single | The interpolation value.  Internally clamped to the range [0,1]. |

_Returns:_ Interpolated settings.
``Void Validate()``

Make sure lens settings are sane.  Call this from OnValidate().



### NoiseSettings

_Type:_ class

_Namespace:_ Cinemachine

_Inherits:_ ScriptableObject


This is an asset that defines a noise profile.  A noise profile is the shape of the noise as a function of time.  You can build arbitrarily complex shapes by combining different base perlin noise frequencies at different amplitudes.

The frequencies and amplitudes should be chosen with care, to ensure an interesting noise quality that is not obviously repetitive.

As a mathematical side-note, any arbitrary periodic curve can be broken down into a series of fixed-amplitude sine-waves added together.  This is called fourier decomposition, and is the basis of much signal processing.  It doesn't really have much to do with this asset, but it's super interesting!

#### Properties


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **PositionNoise** | TransformNoiseParams[] | _[Get]_ Gets the array of positional noise channels for this NoiseSettings. |
| **OrientationNoise** | TransformNoiseParams[] | _[Get]_ Gets the array of orientation noise channels for this NoiseSettings. |


#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **m_Position** | TransformNoiseParams[] | These are the noise channels for the virtual camera's position.  Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3. |
| **m_Orientation** | TransformNoiseParams[] | These are the noise channels for the virtual camera's orientation.  Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3. |


#### Methods

``Void CopyFrom(NoiseSettings other)``

Clones the contents of the other asset into this one.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **other** | NoiseSettings |  |

``static Vector3 GetCombinedFilterResults(TransformNoiseParams[] noiseParams, Single time, Vector3 timeOffsets)``

Get the noise signal value at a specific time.


| _Param_ | _Type_ | _Description_ |
| --- | --- | --- |
| **noiseParams** | TransformNoiseParams[] | The parameters that define the noise function. |
| **time** | Single | The time at which to sample the noise function. |
| **timeOffsets** | Vector3 | Start time offset for each channel. |

_Returns:_ The 3-channel noise signal value at the specified time.


### NoiseSettings.NoiseParams

_Type:_ struct

_Namespace:_ Cinemachine


Describes the behaviour for a channel of noise.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **Amplitude** | Single | The amplitude of the noise for this channel.  Larger numbers vibrate higher. |
| **Frequency** | Single | The frequency of noise for this channel.  Higher magnitudes vibrate faster. |



### NoiseSettings.TransformNoiseParams

_Type:_ struct

_Namespace:_ Cinemachine


Contains the behaviour of noise for the noise module for all 3 cardinal axes of the camera.

#### Fields


| _Name_ | _Type_ | _Description_ |
| --- | --- | --- |
| **X** | NoiseParams | Noise definition for X-axis. |
| **Y** | NoiseParams | Noise definition for Y-axis. |
| **Z** | NoiseParams | Noise definition for Z-axis. |


