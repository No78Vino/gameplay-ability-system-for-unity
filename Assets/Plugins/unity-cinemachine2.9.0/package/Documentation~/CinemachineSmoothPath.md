# Cinemachine Smooth Path

A __Cinemachine Smooth Path__ is a component that defines a world-space path, consisting of an array of waypoints. Each waypoint has position and roll settings. Cinemachine uses Bezier interpolation to calculate positions between the waypoints to get a smooth and continuous path. The path passes through all waypoints. Unlike Cinemachine Path, first and second order continuity is guaranteed, which means that not only the positions but also the angular velocities of objects animated along the path will be smooth and continuous.

## Properties:

| **Property:** || **Function:** |
|:---|:---|:---|
| __Resolution__ || Path samples per waypoint. Cinemachine uses this value to calculate path distances. |
| __Looped__ || If checked, then the path ends are joined to form a continuous loop. |
| __Resolution__ || Path samples per waypoint. This is used for calculating path distances. |
| __Appearance__ || The settings that control how the path appears in the Scene view. |
| | _Path Color_ | The color of the path when it is selected. |
| | _Inactive Path Color_ | The color of the path when it is not selected. |
| | _Width_ | The width of the railroad tracks that represent the path. |
| __Looped__ || Check to join the ends of the path to form a continuous loop. |
| __Path Length__ || The length of the path in distance units (read-only). |
| __Selected Waypoint__ || Properties for the waypoint you selected in the Scene view or in the Waypoints list. |
| __Waypoints__ || The list of waypoints that define the path. They are interpolated using a bezier curve. |
| | _Position_ | Position in path-local space. |
| | _Roll_ | Defines the roll of the path at this waypoint. The other orientation axes are inferred from the tangent and world up. |


