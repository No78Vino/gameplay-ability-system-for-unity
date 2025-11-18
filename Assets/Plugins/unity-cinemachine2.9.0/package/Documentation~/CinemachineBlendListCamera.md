# Cinemachine Blend List Camera

The __Cinemachine Blend List Camera__ component executes a sequence of blends or cuts among its child Virtual Cameras.

When the Blend List camera is activated, it executes its list of instructions, activating the first child Virtual Camera in the list, holding for a designated time, then cutting or blending to the next child, and so on. The Blend List camera holds the last Virtual Camera until Cinemachine Brain or Timeline deactivates the Blend List camera.

**Tip**: Use a Blend List Camera instead of  [Timeline](CinemachineTimeline.md) for simpler, automatic sequences.

## Properties:

| **Property:** | **Function:** |
|:---|:---|
| __Solo__ | Toggles whether or not the Blend List camera is temporarily live. Use this property to get immediate visual feedback in the [Game view](https://docs.unity3d.com/Manual/GameView.html) to adjust the Virtual Camera. |
| __Game Window Guides__ | Toggles the visibility of compositional guides in the Game view. This property applies to all Virtual Cameras. |
| __Save During Play__ | Check to [apply the changes while in Play mode](CinemachineSavingDuringPlay.md).  Use this feature to fine-tune a Virtual Camera without having to remember which properties to copy and paste. This property applies to all Virtual Cameras. |
| __Priority__ | The importance of this Blend List camera for choosing the next shot. A higher value indicates a higher priority. Cinemachine Brain chooses the next live Virtual Camera from all Virtual Cameras that are activated and have the same or higher priority as the current live Virtual Camera. This property has no effect when using a Virtual Camera with Timeline. |
| **Standby Update** | Controls how often the virtual camera is updated when the virtual camera is not live. |
| **Loop** | When enabled, the child virtual cameras will cycle indefintely instead of stopping on the last virtual camera in the list. |
| __Look At__ | The default target GameObject that the children Virtual Camera move with. The Blend List camera uses this target when the child does not specify this target. May be empty if all of the children define targets of their own. |
| __Follow__ | The default target GameObject to aim the Unity camera at. The Blend List camera uses this target when the child does not specify this target. May be empty if all of the children define targets of their own. |
| __Show Debug Text__ | Check to display a textual summary of the live Virtual Camera and blend in the view. |
| __Enable All Child Cameras__ | Check to activate all child cameras. This is useful if animating them in Timeline, but consumes extra resources. |
| __Instructions__ | The set of instructions for enabling child cameras. |
