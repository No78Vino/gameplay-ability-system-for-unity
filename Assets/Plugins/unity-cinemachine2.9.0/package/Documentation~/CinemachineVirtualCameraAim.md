# Aim properties

Use the Aim properties to specify how to rotate the Virtual Camera. To change the camera’s position, use the [Body properties](CinemachineVirtualCameraBody.md).

![Aim properties, with the Composer algorithm (red)](images/CinemachineAim.png)

* [__Do Nothing__](CinemachineAimDoNothing.md): Do not procedurally rotate the Virtual Camera.


- [__Composer__](CinemachineAimComposer.md): Keep the __Look At__ target in the camera frame.
- [__Group Composer__](CinemachineAimGroupComposer.md): Keep multiple __Look At__ targets in the camera frame.
- [__Hard Look At__](CinemachineAimHardLook.md): Keep the __Look At__ target in the center of the camera frame.
- [__POV__](CinemachineAimPOV.md): Rotate the Virtual Camera based on the user’s input.
- [__Same As Follow Target__](CinemachineAimSameAsFollow.md): Set the camera’s rotation to the rotation of the __Follow__ target.