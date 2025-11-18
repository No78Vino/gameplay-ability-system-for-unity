# Setting up Virtual Cameras

In your project, organize your Scene Hierarchy to have a single Unity camera with a CinemachineBrain component and many Virtual Cameras.

To add a Virtual Camera to a Scene:

1. In the Unity menu, choose __GameObject > Cinemachine > Virtual Camera__. <br/>Unity adds a new GameObject with a Cinemachine Virtual Camera component. If necessary, Unity also adds a [Cinemachine Brain](CinemachineBrainProperties.md) component to the Unity camera GameObject for you.

2. Use the __Follow__ property to specify a GameObject to follow. <br/>The Virtual Camera automatically positions the Unity camera relative to this GameObject at all times, even as you move it in the Scene.

3. Use the __Look At__ property to specify the GameObject that the Virtual Camera should aim at. <br/>The Virtual Camera automatically rotates the Unity camera to face this GameObject at all times, even as you move it in the Scene.

4. [Customize the Virtual Camera](CinemachineVirtualCamera.md) as needed. <br/>Choose the algorithm for following and looking at, and adjust settings such as the follow offset, the follow damping, the screen composition, and the damping used when re-aiming the camera.



