# Cinemachine Clear Shot Camera

The __Cinemachine ClearShot Camera__ component chooses among its children Virtual Cameras for the best quality shot of the target. Use Clear Shot to set up complex multi-camera coverage of a Scene to guarantee a clear view of the target.

This can be a very powerful tool. Virtual Camera children with [Cinemachine Collider](CinemachineCollider.md) extensions analyze the Scene for target obstructions, optimal target distance, and so on. Clear Shot uses this information to choose the best child to activate.

**Tip:** To use a single [Cinemachine Collider](CinemachineCollider.md) for all Virtual Camera children, add a Cinemachine Collider extension to the ClearShot GameObject instead of each of its Virtual Camera children. This Cinemachine Collider extension applies to all of the children, as if each of them had that Collider as its own extension.

If multiple child cameras have the same shot quality, the Clear Shot camera chooses the one with the highest priority.

You can also define custom blends between the ClearShot children.

## Properties:

| **Property:** | **Function:** |
|:---|:---|
| **Game Window Guides** | Enables the displays of overlays in the Game window. Adjust the color and opacity in Cinemachine Preferences. This is a global setting, shared by all virtial cameras. |
| **Save During Play** | When enabled, virtual camera setting changes made during Play mode are propagated back to the scene when Play mode is exited.  This is a global setting, shared by all objects that support Save During Play. |
| __Look At__ | The default target GameObject that the children Virtual Camera move with. The Clear Shot camera uses this target when the child does not specify this target. May be empty if all of the children define targets of their own. |
| __Follow__ | The target GameObject to aim the Unity camera at. The Clear Shot camera uses this target when the child does not specify this target. May be empty if all of the children define targets of their own. |
| __Show Debug Text__ | Check to display a textual summary of the live Virtual Camera and blend in the Game view. |
| __Activate After__ | Wait this many seconds before activating a new child camera. |
| __Min Duration__ | An active camera must be active for at least this many seconds, unless a higher-priority camera becomes active. |
| __Randomize Choice__ | Check to choose a random camera if multiple cameras have equal shot quality. Uncheck to use the order of the child Virtual Cameras and their priorities. |
| __Default Blend__ | The blend to use when you haven’t explicitly defined a blend between two Virtual Cameras. |
| __Priority__ | Determine which camera becomes active based on the state of other cameras and this camera. Higher numbers have greater priority. |


