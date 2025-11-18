# Managing and grouping Virtual Cameras

A __manager__ camera oversees many Virtual Cameras but acts as a single Virtual Camera from the point of view of Cinemachine Brain and Timeline.

Cinemachine includes these manager cameras:

* [Free Look Camera](CinemachineFreeLook.md): an enhanced [Orbital Transposer](CinemachineBodyOrbitalTransposer.md). It manages three horizontal orbits, arranged vertically to surround an avatar.

* [Mixing Camera](CinemachineMixingCamera.md): uses the weighted average of up to eight child Virtual Cameras.

* [Blend List Camera](CinemachineBlendListCamera.md): executes a sequence of blends or cuts of its child Virtual Cameras.

* [Clear Shot Camera](CinemachineClearShot.md): picks the child Virtual Camera with the best view of the target.

* [State-Driven Camera](CinemachineStateDrivenCamera.md): picks a child Virtual Camera in reaction to changes in animation state.

Because manager cameras act like normal Virtual Cameras, you can nest them. In other words, create arbitrarily complex camera rigs that combine regular Virtual Cameras and manager cameras.

