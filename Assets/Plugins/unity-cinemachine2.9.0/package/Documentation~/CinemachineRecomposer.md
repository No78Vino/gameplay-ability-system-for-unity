# Cinemachine Recomposer Extension

Use the Cinemachine Recomposer [extension](CinemachineVirtualCameraExtensions.md) is an add-on module for Cinemachine Virtual Camera that adds a final tweak to the camera composition.  It is intended for use in a Timeline context, where you want to hand-adjust the output of procedural or recorded camera aiming.

All of these properties can be animated within the Timeline.

## Properties:

| **Property:** | **Function:** |
|:---|:---|
| __Apply After__ | The Volume Settings profile to activate when this Virtual Camera is live. |
| _Body_ | Camera has been positioned but not yet rotated |
|  _Aim_ | Camera has been rotated and positioned, but no noise or collision resolution applied |
|  _Noise_ | Camera has been positioned, rotated, and noise and other corrections applied |
|  _Finalize_ | Default setting.  Applied after all standard virtual camera processing has occurred |
| __Tilt__ | Add a vertical rotation to the camera's current rotation |
| __Pan__ | Add a horizontal rotation to the camera's current rotation |
| __Dutch__ | Add a tilt (local Z rotation) to the current camera's rotation |
| __Zoom Scale__ | Scale the current zoom |
| __Follow Attachment__ | When this is less than 1, damping on the Follow target is increased.  When the value is zero, damping is infinite - effectively "letting go" of the target |
| __Look At Attachment__ | When this is less than 1, damping on the Look At target is increased.  When the value is zero, damping is infinite - effectively "letting go" of the target |


