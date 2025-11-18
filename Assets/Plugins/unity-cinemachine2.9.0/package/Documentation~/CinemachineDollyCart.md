# Cinemachine Dolly Cart

__Cinemachine Dolly Cart__ is a component that constrains the transform of its GameObject to a __Cinemachine Path__ or __Cinemachine Smooth Path__. Use it to animate a GameObject along a path, or as a __Follow__ target for Virtual Cameras.

## Properties:

| **Property:** || **Function:** |
|:---|:---|:---
| __Path__ || The path to follow. |
| __Update Method__ || When to move the cart when velocity is non-zero. Use __Update__ for normal `MonoBehaviour` updating, __Fixed Update__ for updates in sync with the Physics module, `FixedUpdate()`. |
| __Position Units__ || The unit of measure for __Position__.  |
| | _Path Units_ | Use waypoints along the path. The value 0 represents the first waypoint on the path, 1 is the second waypoint, and so on. |
| | _Distance_ | Use distance along the path. The path is sampled according to the Resolution property of the path. Cinemachine creates a distance lookup table, which it stores in an internal cache. |
| | _Normalized_ | Use the beginning and end of the path. The value 0 represents the beginning of the path, 1 is the end of the path. |
| __Speed__ || Move the cart with this speed. The value is interpreted according to __Position Units__. |
| __Position__ || The position along the path to place the cart. This can be animated directly or, if the speed is non-zero, will be updated automatically. The value is interpreted according to __Position Units__. |
