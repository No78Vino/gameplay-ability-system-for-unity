# Cinemachine Volume Settings Extension

Use the Cinemachine Volume Settings [extension](CinemachineVirtualCameraExtensions.md) to attach an HDRP/URP VolumeSettings profile to a Virtual Camera.

The Cinemachine Volume Settings extension holds a Volume Settings Profile asset to apply to a Virtual Camera when it is activated. If the camera is blending with another Virtual Camera, then the blend weight is applied to the Volume Settings effects also.

To add a Volume Settings profile to a Virtual Camera

1. Select your Virtual Camera in the [Scene](https://docs.unity3d.com/Manual/UsingTheSceneView.html) view or [Hierarchy](https://docs.unity3d.com/Manual/Hierarchy.html) window.

2. In the [Inspector](https://docs.unity3d.com/Manual/UsingTheInspector.html), choose __Add Extension > CinemachineVolumeSettings__, then configure the Profile asset to have the effects you want when this virtual camera is live.

## Properties:

| **Property:** || **Function:** |
|:---||:---|
| __Profile__ || The Volume Settings profile to activate when this Virtual Camera is live. |
| __Focus Tracks Target__ || This is obsolete, please use __Focus Tracking__. |
| __Focus Tracking__ || If the profile has the appropriate overrides, will set the base focus distance to be the distance from the selected target to the camera.  The __Focus Offset__ field will then modify that distance. |
|| _None_ | No focus tracking |
|| _Look At Target_ | Focus offset is relative to the LookAt target |
|| _Follow Target_ | Focus offset is relative to the Follow target |
|| _Custom Target_ | Focus offset is relative to the Custom target |
|| _Camera_ | Focus offset is relative to the camera |
| __Focus Target__ || The target to use if __Focus Tracks Target__ is set to _Custom Target_ |
| __Focus Offset__ || Used when __Focus Tracking__ is not _None_.  Offsets the sharpest point away from the location of the focus target. |


