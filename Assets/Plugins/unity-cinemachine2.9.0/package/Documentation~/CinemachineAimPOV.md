# POV

This Virtual Camera __Aim__ algorithm aims the camera in response to the user’s input.

## Properties:

| **Property:** || **Function:** |
|:---|:---|:---|
| __Vertical Axis__ || Controls the vertical orientation of the Virtual Camera’s aim.  |
| | _Value_ | The current value of the axis to aim the camera at, in degrees. Accepted values are -90 to 90.  |
| | _Value Range_ | The minimum and maximum values for the vertical axis of the Virtual Camera. |
| | _Wrap_ | If checked, the axis wraps around the Value Range values, forming a loop. |
| | _Max Speed_ | The maximum speed of this axis in degrees/second, or the multipler for the input value if Speed Mode is set to _InputValueGain_. |
| | _Speed Mode_ | How the axis responds to input. _MaxSpeed_ (the default) clamps the maximum speed at which the axis can change, regardless of the input.  _Input Value Gain_ multiplies the input value by MaxSpeed. |
| | _Accel Time_ | The amount of time in seconds it takes to accelerate to Max Speed with the supplied axis at its maximum value. |
| | _Decel Time_| The amount of time in seconds it takes to decelerate the axis to zero if the supplied axis is in a neutral position. |
| | _Input Axis Name_ | The name of this axis as specified in Unity Input Manager. To disable the automatic updating of this axis, set this property to an empty string. |
| | _Input Axis Value_ | The value of the input axis. A value of 0 means no input. You can drive this directly from a custom input system, or you can set the Input Axis Name and have the value driven by the Unity Input Manager. |
| | _Invert_ | Check to invert the raw value of the input axis before it is used. |
| __Vertical Recentering__ || Controls automatic vertical recentering when the player gives no input. |
| | _Enabled_ | Check to enable automatic vertical recentering. |
| | _Wait Time_ | If no user input has been detected on the vertical axis, the camera waits this long in seconds before recentering. |
| | _Recentering Time_ | Maximum angular speed of recentering. Accelerates into and decelerates out of this. |
| __Horizontal Axis__ || Controls the horizontal orientation.  |
| | _Value_ | The current value of the axis, in degrees. Accepted values are -180 to 180. |
| | _Value Range_ | The minimum and maximum values for the axis. |
| | _Wrap_ | If checked, the axis wraps around the Value Range values, forming a loop. |
| | _Max Speed_ | The maximum speed of this axis in degrees/second, or the multipler for the input value if Speed Mode is set to _InputValueGain_. |
| | _Speed Mode_ | How the axis responds to input.  _MaxSpeed_ (the default) clamps the maximum speed at which the axis can change, regardless of the input.  _Input Value Gain_ multiplies the input value by MaxSpeed. |
| | _Accel Time_ | The amount of time in seconds it takes to accelerate to Max Speed with the supplied Axis at its maximum value. |
| | _Decel Time_ | The amount of time in seconds it takes to decelerate the axis to zero if the supplied axis is in a neutral position. |
| | _Input Axis Name_ | The name of this axis as specified in the Unity Input Manager. Set this property to an empty string to disable automatic update of this axis. |
| | _Input Axis Value_ | The value of the input axis. A value of 0 means no input. You can drive this directly from a custom input system, or you can set the Input Axis Name and have the value driven by the Unity Input Manager. |
| | _Invert_ | Check to invert the raw value of the input axis before it is used. |
| __Horizontal Recentering__ || Controls automatic vertical recentering when the player gives no input. |
| | _Enabled_ | Check to enable automatic vertical recentering. |
| | _Wait Time_ | If no user input has been detected on the vertical axis, the camera waits this long in seconds before recentering. |
| | _Recentering Time_ | Maximum angular speed of recentering. Accelerates into and decelerates out of this. |

