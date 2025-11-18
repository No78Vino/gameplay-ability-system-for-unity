# Tracked Dolly

This Virtual Camera __Body__ algorithm restricts the Virtual Camera to move along a predefined [path](CinemachineDolly.md). Use the __Path Position__ property to specify where to put the Virtual Camera on the path.

Use __Auto-Dolly__ mode to move the Virtual Camera to a position on the path that is closest to the __Follow__ target. When enabled, __Auto-Dolly__ automatically animates the position of the Virtual Camera to the position on the path that’s closest to the target.

**Tip**: Choose your path shapes with care when using Auto-Dolly mode. This becomes problematic on paths that form an arc around some point.  As an extreme example, consider a perfectly circular path with the __Follow__ target at the center. The closest point on the path becomes unstable because all points on the circular path are equally close to the target. In this situation, moving the __Follow__ target small distances can cause the camera to move large distances on the track.

## Properties:

| **Property:** || **Function:** |
|:---|:---|:---|
| __Path__ || The path that the camera moves along. This property must refer to a  [Cinemachine Path](CinemachinePath.md) or [Cinemachine Smooth Path](CinemachineSmoothPath.md). |
| __Path Position__ || The position along the path to place the camera. Animate this property directly or enable Auto-Dolly. The value is in the units specified by Position Units. |
| __Position Units__ || The unit of measure for Path Position.  |
| | _Path Units_ | Use waypoints along the path. The value 0 represents the first waypoint on the path, 1 is the second waypoint, and so on. |
| | _Distance_ | Use distance along the path. The path is sampled according to the Resolution property of the path. Cinemachine creates a distance lookup table, which it stores in an internal cache. |
| | _Normalized_ | Use the beginning and end of the path. The value 0 represents the beginning of the path, 1 is the end of the path. |
| __Path Offset__ || The position of the camera relative to the path. X is perpendicular to the path, Y is up, and Z is parallel to the path. Use this property to offset the camera from the path itself. |
| __X Damping__ || How responsively the camera tries to maintain its position in a direction perpendicular to the path. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly. Using different settings per axis can yield a wide range of camera behaviors. |
| __Y Damping__ || How responsively the camera tries to maintain its position in the path-local up direction. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.   |
| __Z Damping__ || How responsively the camera tries to maintain its position in a direction parallel to the path. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |
| __Camera Up__ || How to set the up vector for the Virtual Camera. This affects the screen composition because the camera Aim algorithms try to respect the up direction. |
| | _Default_ | Do not modify the up direction of the Virtual Camera. Instead, use the World Up Override property in Cinemachine Brain. |
| | _Path_ | Use the path’s up vector at the current point. |
| | _Path No Roll_ | Use the path’s up vector at the current point, but with the roll set to zero. |
| | _Follow Target_ | Use the up vector from the Follow target’s transform. |
| | _Follow Target No Roll_ | Use the up vector from the Follow target’s transform, but with the roll zeroed out. |
| __Pitch Damping__ || How responsively the camera tracks the target rotation’s x angle. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |
| __Yaw Damping__ || How responsively the camera tracks the target rotation’s y angle. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |
| __Roll Damping__ || How responsively the camera tracks the target rotation’s z angle. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  |
| __Auto Dolly__ || Controls how automatic dollying occurs. A Follow target is necessary to use this feature. |
| | _Enabled_ | Check to enable the automatic dolly. Note: this can have some performance impact, depending on the search resolution. |
| | _Position Offset_ | Offset, in position units, from the closest point on the path to the follow target. |
| | _Search Radius_ | The number of segments on either side of the current segment. Use 0 for the entire path.  Use a lower number when the path’s shape relative to the target position causes the closest point on the path to become unstable. |
| | _Search Resolution_ | Cinemachine searches a segment by dividing it into many straight pieces. The higher the number, the more accurate the result. However, performance is proportionally slower for higher numbers. |


