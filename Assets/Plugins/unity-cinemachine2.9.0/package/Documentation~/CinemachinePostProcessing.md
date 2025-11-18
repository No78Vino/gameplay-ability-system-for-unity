# Post Processing Extension

Use the Cinemachine Post Processing [extension](CinemachineVirtualCameraExtensions.md) to attach a Postprocessing V2 profile to a Virtual Camera.

**Note 1**: Unity recommends using Postprocessing V2 instead of Postprocessing V1.

**Note 2**: With HDRP and URP 7 and up, The PostProcessing package is deprecated, and is implemented natively by HDRP and URP.  In that case, please see the __CinemachineVolumeSettings__ extension.

The Cinemachine Post Processing extension holds a Post-Processing Profile asset to apply to a Virtual Camera when it is activated. If the camera is blending with another Virtual Camera, then the blend weight is applied to the Post Process effects also.

Before attaching post processing profiles to Virtual Cameras, you first need to set up your project to use post processing. 

To set up project to use Post Processing V2 with Cinemachine:

1. [Install](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html) the Postprocessing V2 package.

2. Select your Unity camera with Cinemachine Brain in the [Scene](https://docs.unity3d.com/Manual/UsingTheSceneView.html) view.

3. [Add the component](https://docs.unity3d.com/Manual/UsingComponents.html) named __Post-Process Layer__.  This will enable Post Process profiles to affect the Camera.

To add a Post Process profile to a Virtual Camera

4. Select your Virtual Camera in the [Scene](https://docs.unity3d.com/Manual/UsingTheSceneView.html) view or [Hierarchy](https://docs.unity3d.com/Manual/Hierarchy.html) window.

5. In the [Inspector](https://docs.unity3d.com/Manual/UsingTheInspector.html), choose __Add Extension > CinemachinePostProcessing__, then configre the Profile asset to have the effects you want when this virtual camera is live.

## Properties:

| **Property:** || **Function:** |
|:---||:---|
| __Profile__ || The Post-Processing profile to activate when this Virtual Camera is live. |
| __Focus Tracks Target__ || This is obsolete, please use __Focus Tracking__. |
| __Focus Tracking__ || If the profile has the appropriate overrides, will set the base focus distance to be the distance from the selected target to the camera.  The __Focus Offset__ field will then modify that distance. |
|| _None_ | No focus tracking |
|| _Look At Target_ | Focus offset is relative to the LookAt target |
|| _Follow Target_ | Focus offset is relative to the Follow target |
|| _Custom Target_ | Focus offset is relative to the Custom target |
|| _Camera_ | Focus offset is relative to the camera |
| __Focus Target__ || The target to use if __Focus Tracks Target__ is set to _Custom Target_ |
| __Focus Offset__ || Used when __Focus Tracking__ is not _None_.  Offsets the sharpest point away from the location of the focus target. |


