# Framing Transposer

This Virtual Camera __Body__ algorithm moves the camera in a fixed screen-space relationship to the __Follow__ target. You can also specify offsets, damping, and composition rules. __Framing Transposer__ only changes the camera’s position in space. It does not re-orient or otherwise aim the camera.

__Framing Transposer__ is designed for 2D and orthographic cameras. But it works also with perspective cameras and 3D environments.

This algorithm first moves the camera along the camera Z axis until the __Follow__ target is at the desired distance from the camera’s X-Y plane. It then moves the camera in its X-Y plane until the __Follow__ target is at the desired point on the camera’s screen.

**Note**: __Framing Transposer__ ignores the LookAt target - only the Follow target is used.

If the __Follow__ target is a [Target Group](CinemachineTargetGroup.md), then additional properties are available to frame the entire group.

## Properties

| **Property:** || **Function:** |
|:---|:---|:---|
| __Lookahead Time__ || Adjusts the offset of the Virtual Camera from the Follow target based on the motion of the target. Cinemachine estimates the point where the target will be this many seconds into the future. This feature is sensitive to noisy animation and can amplify the noise, resulting in undesirable camera jitter. If the camera jitters unacceptably when the target is in motion, turn down this property, or animate the target more smoothly. |
| __Lookahead Smoothing__ || The smoothness of the lookahead algorithm. Larger values smooth out jittery predictions and increase prediction lag. |
| __Lookahead Ignore Y__ || If checked, ignore movement along the Y axis for lookahead calculations. |
| __X Damping__ || How responsively the camera tries to maintain the offset in the x-axis. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.  Using different settings per axis can yield a wide range of camera behaviors. |
| __Y Damping__ || How responsively the camera tries to maintain the offset in the y-axis. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.   |
| __Z Damping__ || How responsively the camera tries to maintain the offset in the z-axis. Small numbers make the camera more responsive. Larger numbers make the camera respond more slowly.   |
| __Target Movement Only__ || If this is enabled, then damping only applies to the motion of the target.  Camera rotation changes will bypass damping. |
| __Screen X__ || Horizontal screen position for target. The camera moves to position the tracked object here. |
| __Screen Y__ || Vertical screen position for target, The camera moves to position the tracked object here. |
| __Camera Distance__ || The distance to maintain along the camera axis from the Follow target. |
| __Dead Zone Width__ || Do not move the camera horizontally when the target is within this range of the position. |
| __Dead Zone Height__ || Do not move the camera vertically if the target is within this range of the position. |
| __Dead Zone Depth__ || Do not move the camera along its z-axis if the Follow target is within this distance of the specified camera distance. |
| __Unlimited Soft Zone__ || If checked, then the soft zone is unlimited in size. |
| __Soft Zone Width__ || When the target is in this range, move the camera horizontally to frame the target in the dead zone. The Damping properties affect the rate of the camera movement.  |
| __Soft Zone Height__ || When the target is in this range, move the camera vertically to frame the target in the dead zone. The Damping properties affect the rate of the camera movement.  |
| __Bias X__ || Moves the target position horizontally away from the center of the soft zone. |
| __Bias Y__ || Moves the target position vertically away from the center of the soft zone. |
| __Group Framing Mode__ || Available when Follow specifies a [Target Group](CinemachineTargetGroup.md). Specifies the screen dimensions to consider when framing.  |
| | _Horizontal_ | Consider only the horizontal dimension. Ignore vertical framing. |
| | _Vertical_ | Consider only the vertical dimension. Ignore horizontal framing. |
| | _Horizontal And Vertical_ | Use the larger of the horizontal and vertical dimensions to get the best fit. |
| | _None_ | Don’t do any framing adjustment. |
| __Adjustment Mode__ || How to adjust the camera to get the desired framing. You can zoom, dolly in or out, or do both. Available when Follow specifies a Target Group.  |
| | _Zoom Only_ | Don’t move the camera, only adjust the FOV. |
| | _Dolly Only_ | Move the camera, don’t change the FOV. |
| | _Dolly Then Zoom_ | Move the camera as much as permitted by the ranges, then adjust the FOV if necessary to make the shot. |
| __Group Framing Size__ || The bounding box that the targets should occupy. Use 1 to fill the whole screen, 0.5 to fill half the screen, and so on. Available when Follow specifies a Target Group.  |
| __Max Dolly In__ || The maximum distance toward the target to move the camera. Available when Follow specifies a Target Group.  |
| __Max Dolly Out__ || The maximum distance away from the target to move the camera. Available when Follow specifies a Target Group.  |
| __Minimum Distance__ || Set this to limit how close to the target the camera can get. Available when Follow specifies a Target Group.  |
| __Maximum Distance__ || Set this to limit how far from the target the camera can get. Available when Follow specifies a Target Group.  |
| __Minimum FOV__ || If adjusting FOV, do not set the FOV lower than this. Available when Follow specifies a Target Group.  |
| __Maximum FOV__ || If adjusting FOV, do not set the FOV higher than this. Available when Follow specifies a Target Group.  |
| __Minimum Ortho Size__ || If adjusting Orthographic Size, do not set it lower than this. Available when Follow specifies a Target Group.  |
| __Maximum Ortho Size__ || If adjusting Orthographic Size, do not set it higher than this. Available when Follow specifies a Target Group. |



