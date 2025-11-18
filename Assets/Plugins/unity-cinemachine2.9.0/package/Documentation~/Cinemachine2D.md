# 2D graphics

Cinemachine supports orthographic cameras. When you set the Unity camera's projection to Orthographic, Cinemachine adjusts to accommodate it. In Virtual Camera properties for __Lens__, __FOV__ is replaced by __Orthographic Size__. Note that settings related to FOV and certain FOV-oriented behaviors such as [Follow Zoom](CinemachineFollowZoom.md) have no effect if the camera is orthographic.

In orthographic environments, it doesn’t usually make sense to rotate the camera. Accordingly, Cinemachine offers the [Framing Transposer](CinemachineBodyFramingTransposer.md) to handle framing and composition without rotating the camera.

