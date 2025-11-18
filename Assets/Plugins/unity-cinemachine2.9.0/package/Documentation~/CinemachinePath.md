# Cinemachine Path

__Cinemachine Path__ is a component that defines a world-space path, consisting of an array of waypoints. Each waypoint specifies position, tangent, and roll settings. Bezier interpolation is performed between the waypoints, to get a smooth and continuous path.

**Tip**: While the path position will always be smooth and continuous, it is still possible to get jarring movement when animating along the path. This happens when tangents aren’t set to ensure continuity of both the first and second order derivatives. It is not easy to get this right.  To avoid this potentially jarring movement, use Cinemachine Smooth Path. CinemachineSmoothPath sets the tangents automatically to ensure complete smoothness.

## Properties:

| **Property:** || **Function:** |
|:---|:---|:---|
| __Resolution__ || Path samples per waypoint. Cinemachine uses this value to limit the granularity when calculating path distances. The crosshatches on the path gizmo in the scene view reflect this value|
| __Appearance__ || The settings that control how the path appears in the Scene view. |
| | _Path Color_ | The color of the path when it is selected. |
| | _Inactive Path Color_ | The color of the path when it is not selected. |
| | _Width_ | The width of the railroad tracks that represent the path. |
| __Looped__ || Check to join the ends of the path to form a continuous loop. |
| __Path Length__ || The length of the path in distance units (read-only). |
| __Selected Waypoint__ || Properties for the waypoint you selected in the Scene view or in the Waypoints list. |
| __Prefer Tangent Drag__ || Check to use the Gizmo for the tangent of a waypoint when the Gizmos for the tangent and position coincide in the Scene view.  |
| __Path Details__ || The list of waypoints that define the path. |
| | _Position_ | Position in path-local space (i.e. relative to the transform of the path object itself) |
| | _Tangent_ | Offset from the position, which defines the tangent of the curve at the waypoint. The length of the tangent encodes the strength of the bezier handle. The same handle is used symmetrically on both sides of the waypoint, to ensure smoothness. |
| | _Roll_ | The roll of the path at this waypoint. The other orientation axes are inferred from the tangent and world up. |


