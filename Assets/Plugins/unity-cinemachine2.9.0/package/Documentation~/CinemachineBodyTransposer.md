# Transposer

This Virtual Camera __Body__ algorithm moves the Virtual Camera in a fixed offset to the __Follow__ target. It also applies damping.

The fixed offset can be interpreted in various ways, depending on the Binding Mode.

## Properties:

| **Property:** || **Function:** |
|:---|:---|:---|
| __[Binding Mode](CinemachineBindingModes.md)__ || The coordinate space to use to interpret the offset from the target. |
| | _Lock To Target On Assign_ | Makes the orientation of the virtual camera match the local frame of the Follow target, at the moment when the virtual camera is activated or when the target is assigned. This offset remains constant in world space. The camera does not rotate along with the target. |
| | _Lock To Target With World Up_ | Makes the virtual camera use the local frame of the Follow target with tilt and roll set to 0. This binding mode ignores all target rotations except yaw. |
| | _Lock To Target No Roll_ | Makes the virtual camera use the local frame of the Follow target, with roll set to 0. |
| | _Lock To Target_ | Makes the virtual camera use the local frame of the Follow target. When the target rotates, the camera moves with it to maintain the offset and to maintain the same view of the target. |
| | _World Space_ | The offset is interpreted in world space relative to the origin of the Follow target. The camera will not change position when the target rotates. |
| | _Simple Follow With World Up_ | Simple follow with world up interprets the offset and damping values in camera-local space. This mode emulates the action a human camera operator would take when instructed to follow a target. The camera attempts to move as little as possible to maintain the same distance from the target; the direction of the camera with regard to the target does not matter. Regardless of the orientation of the target, the camera tries to preserve the same distance and height from it. |
| __Follow Offset__ || The distance to maintain the Virtual Camera relative to the Follow target. Set X, Y, and Z to 0 to place the camera at the centre of the target. The default is 0, 0, -10, respectively, which places the camera behind the target. |
| __X Damping__ || How responsively the camera tries to maintain the offset in the x-axis. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |
| __Y Damping__ || How responsively the camera tries to maintain the offset in the y-axis. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.   |
| __Z Damping__ || How responsively the camera tries to maintain the offset in the z-axis. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.   |
| __Pitch Damping__ || How responsively the camera tracks the target rotation’s x angle. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |
| __Yaw Damping__ || How responsively the camera tracks the target rotation’s y angle. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |
| __Roll Damping__ || How responsively the camera tracks the target rotation’s z angle. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |



