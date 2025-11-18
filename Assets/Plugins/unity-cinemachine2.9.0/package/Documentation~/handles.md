# Cinemachine Handle toolbar

The Cinemachine Handle toolbar is a group of 3D controls that allow you to manipulate virtual camera parameters visually in the Scene view. You can use the handle tools to interactively adjust the selected object's parameters quickly and efficiently rather than controlling them via the inspector.

## Activating and deactivating the toolbar

To activate the Handle toolbar:

* Right-click on the **Scene** tab in the Scene view.
* Select **Overlays** and then **Cinemachine** from the pop-up menu.

![overlays-menu](images/overlays-menu.png)

The Cinemachine Handle toolbar appears as shown above.
* Select any Handle tool from the toolbar.
* The corresponding Handle then appears in the Scene view.
* You can then select the Handle again to deactivate it from the Scene view.

## Handle tools

The following four Handle tools are available in the toolbar:

![handle-toolbar](images/handle-toolbar.png)

**1. Field of View (FOV)**

The FOV tool can adjust Vertical FOV, Horizontal FOV, Orthographic Size, or Focal Length depending on what's selected by the user. It can control:

* Vertical or Horizontal FOV (depending on the selection in the Main Camera) when the camera is in Perspective mode.
* Orthographic Size when the camera is in Orthographic mode.
* Focal length when the camera is in Physical mode.

![FOV](images/FOV.png)

For more information on the Field of View (FOV) property see, [Setting Virtual Camera properties](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/CinemachineVirtualCamera.html).

**2. Far/Near clip planes**

* You can drag the points to increase the far clip plane and near clip plane.

![clip-plane](images/clip-plane.png)

For more information on the Far and Near clip plane properties see, [Setting Virtual Camera properties](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/CinemachineVirtualCamera.html).

**3. Follow offset**

The Follow offset is an offset from the Follow Target. You can drag the points to increase or decrease the Follow offset position.

![follow-offset](images/follow-offset.png)

For more information on the Follow offset property see, [Orbital Transposer properties](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/CinemachineBodyOrbitalTransposer.html).

**4. Tracked object offset**

This starts from where the camera is placed. You can drag the points to increase or decrease the tracking target position when the desired area isn't the tracked object’s center.

![tracked-object-offset](images/tracked-object-offset.png)

For more information on the Tracked object offset property see, [Composer properties](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/CinemachineAimComposer.html).
