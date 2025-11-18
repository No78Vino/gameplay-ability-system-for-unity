# Cinemachine Confiner

Use the __Cinemachine Confiner__ [extension](CinemachineVirtualCameraExtensions.md) to limit the camera’s position to a volume or area.

Confiner operates in 2D or 3D mode.  The mode influences the kind of bounding shape it accepts. In 3D mode, the camera’s position in 3D is confined to a volume.  This also works for 2D games, but you need to take the depth into account.  In 2D mode, you don’t have to worry about depth.

For orthographic cameras, there is an additional option to confine the screen edges, not just the camera point.  This ensures that the entire screen area stays within the bounding area.

| **Property:** || **Function:** |
|:---|:---|:---|
| __Confine Mode__ || Operate using a 2D bounding area or a 3D bounding volume. |
| | _Confine 2D_ | Use a Collider2D bounding area. |
| | _Confine 3D_ | Use a 3D Collider bounding volume. |
| __Bounding Volume__ || The 3D volume to contain the camera in. This property is available when Confine Mode is set to Confine 3D. |
| __Bounding Shape 2D__ || The 2D area to contain the camera in. This property is available when Confine Mode is set to Confine 2D. |
| __Confine Screen Edges__ || Check to confine screen edges to the area when the camera is orthographic. When unchecked, confine only the camera center.  Has no effect if camera is in perspective mode. |
| __Damping__ || How gradually to return the camera to the bounding volume or area if it goes beyond the borders. Higher numbers are more gradual. |


