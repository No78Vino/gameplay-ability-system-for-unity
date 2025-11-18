# Binding Modes

The binding mode defines the coordinate space Unity uses to interpret the camera offset from the target and the damping.
</br></br>

### Lock To Target

Makes the virtual camera use the local frame of the Follow target. When the target rotates, the camera rotates with it to maintain the offset and to maintain the same view of the target.

|                                           |                                   |
| ------------------------------------------------- | --------------------------------------------------- |
| Start </br>![](images/cm-binding-mode-lock-target-start.png) | Pitch, 45 degrees </br> ![](images/cm-binding-mode-lock-target-pitch45.png) |
| Yaw, 45 degrees </br> ![](images/cm-binding-mode-lock-target-yaw45.png) | Roll, 45 degrees </br>![](images/cm-binding-mode-lock-target-roll45.png) |

</br></br>
### Lock To Target No Roll



Makes the virtual camera use the local frame of the Follow target, with roll set to 0.

|                                                    |                                            |
| --------------------------------------------------------- | ----------------------------------------------------------- |
| Start  </br> ![](images/cm-binding-mode-lock-target-no-roll-start.png) | Pitch, 45 degrees </br>![](images/cm-binding-mode-lock-target-no-roll-pitch45.png) |
| Yaw, 45 degrees </br>![](images/cm-binding-mode-lock-target-no-roll-yaw45.png) | Roll, 45 degrees </br>![](images/cm-binding-mode-lock-target-no-roll-roll45.png) |



</br></br>

### Lock To Target On Assign



Makes the orientation of the virtual camera match the local frame of the Follow target, at the moment when the virtual camera is activated or when the target is assigned. This offset remains constant in world space. The camera does not rotate along with the target.



|                                                        |                                             |
| ----------------------------------------------------------- | ------------------------------------------------------------ |
| Start </br> ![](images/cm-binding-mode-lock-target-on-assign-start.png) | Pitch, 45 degrees </br>![](images/cm-binding-mode-lock-target-on-assign-pitch45.png) |
| Yaw, 45 degrees </br>![](images/cm-binding-mode-lock-target-on-assign-yaw45.png) | Roll, 45 degrees </br>![](images/cm-binding-mode-lock-target-on-assign-roll45.png) |


</br></br>
### Lock To Target With World Up



Makes the virtual camera use the local frame of the Follow target with tilt and roll set to 0. This binding mode ignores all target rotations except yaw.



|                                                        |                                             |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| Start </br> ![](images/cm-binding-mode-lock-target-world-up-start.png) | Pitch, 45 degrees </br>![](images/cm-binding-mode-lock-target-world-up-pitch45.png) |
| Yaw, 45 degrees </br>![](images/cm-binding-mode-lock-target-world-up-yaw45.png) | Roll, 45 degrees </br>![](images/cm-binding-mode-lock-target-world-up-roll45.png) |


</br></br>


### World Space



The offset is interpreted in world space relative to the origin of the Follow target. The camera will not change position when the target rotates.



|                                            |                                   |
| ------------------------------------------------- | --------------------------------------------------- |
|Start </br> ![](images/cm-binding-mode-world-space-start.png) | Pitch, 45 degrees </br>![](images/cm-binding-mode-world-space-pitch45.png) |
| Yaw, 45 degrees </br> ![](images/cm-binding-mode-world-space-yaw45.png) | Roll, 45 degrees </br>![](images/cm-binding-mode-world-space-roll45.png) |



</br></br>

### Simple Follow With World Up



Simple follow with world up interprets the offset and damping values in camera-local space. This mode emulates the action a human camera operator would take when instructed to follow a target.

The camera attempts to move as little as possible to maintain the same distance from the target; the direction of the camera with regard to the target does not matter. Regardless of the orientation of the target, the camera tries to preserve the same distance and height from it.



|                                                         |                                            |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| Start </br>![](images/cm-binding-mode-simple-follow-world-up-start.png) | Pitch, 45 degrees </br>![](images/cm-binding-mode-simple-follow-world-up-pitch45.png) |
| Yaw, 45 degrees </br>![](images/cm-binding-mode-simple-follow-world-up-yaw45.png) | Roll, 45 degrees </br>![](images/cm-binding-mode-simple-follow-world-up-roll45.png) |

