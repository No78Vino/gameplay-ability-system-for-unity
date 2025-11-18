# About Cinemachine

![Unity Cinemachine](images/CinemachineSplash.png)

Cinemachine is a suite of modules for operating the Unity camera. Cinemachine solves the complex mathematics and logic of tracking targets, composing, blending, and cutting between shots. It is designed to significantly reduce the number of time-consuming manual manipulations and script revisions that take place during development.

The procedural nature of these modules makes Cinemachine bug-resistant. When you make adjustments—for example, change an animation, vehicle speed, terrain, or other GameObjects in your Scene—Cinemachine dynamically adjusts its behavior to make the best shot. There is no need, for example, to re-write camera scripts just because a character turns left instead of right.

Cinemachine works in real time across all genres including FPS, third person, 2D, side-scroller, top down, and RTS. It supports as many shots in your Scene as you need. Its modular system lets you compose sophisticated behaviors.

Cinemachine works well with other Unity tools, acting as a powerful complement to Timeline, animation, and post-processing assets. Create your own [extensions](CinemachineVirtualCameraExtensions.md) or integrate it with your custom camera scripts.

## Installing Cinemachine

Cinemachine is a free package, available for any project. You install Cinemachine like [any other package](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html).

After you install Cinemachine, a new *Cinemachine* folder appears in the Gizmos folder of your Project window, and a new __GameObject > Cinemachine__ menu is available. You can also access this menu when you right click inside the Hierarchy view.

![Cinemachine menu in the Unity Editor](images/CinemachineMenu.png)

## Requirements

Cinemachine has no external dependencies. Just install it and start using it. If you are also using the Post Processing Stack (version 2), then adapter modules are provided - protected by `ifdef` directives which auto-define if the presence of the Post Processing Stack is detected.  

There are similar `ifdef`-protected behaviours for other packages, such as HDRP Volumes, Timeline, and Pixel-Perfect.

This version of Cinemachine is supported by the following versions of the Unity Editor:

* 2019.4+ and later

### Upgrading from the Cinemachine Asset Package

If you already installed Cinemachine from the Unity Asset Store, you can upgrade to the Cinemachine Package.

To upgrade to the Cinemachine Package:

1. In Unity Editor, 2019.4+ or later, open your project.

2. Save the current Scene you are working on.

3. Create a new, empty Scene.

4. In the [Project window](https://docs.unity3d.com/Manual/ProjectView.html), delete the Cinemachine Asset and any CinemachinePostProcessing adaptor assets you may have installed.

5. Install the Cinemachine package.