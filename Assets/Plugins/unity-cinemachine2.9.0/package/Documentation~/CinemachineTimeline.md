# Cinemachine and Timeline

Use [Timeline](https://docs.unity3d.com/Packages/com.unity.timeline@latest) to activate, deactivate, and blend between Virtual Cameras. In Timeline, combine Cinemachine with other GameObjects and assets to interactively implement and tune rich cutscenes, even interactive ones.

**Tip**: For simple shot sequences, use a [Cinemachine Blend List Camera](CinemachineBlendListCamera.md) instead of Timeline.

Timeline overrides the priority-based decisions made by [Cinemachine Brain](CinemachineBrainProperties.md). When the timeline finishes, control returns to the Cinemachine Brain, which chooses the Virtual Camera with the highest Priority setting.

You control Virtual Cameras in Timeline with a __Cinemachine Shot Clip__. Each shot clip points to a Virtual Camera to activate then deactivate. Use a sequence of shot clips to specify the order and duration of each shot.

To cut between two Virtual Cameras, place the clips next to each other. To blend between two Virtual Cameras, overlap the clips.

![Cinemachine Shot Clips in Timeline, with a cut (red) and a blend (blue)](images/CinemachineTimelineShotClips.png)

To create a Timeline for Cinemachine:

1. Create an empty GameObject in your Scene by choosing the __GameObject > Create Empty __menu item.

2. Give the empty GameObject a descriptive name. For example, `IntroTimeline`.

3. In your Scene, select your empty Timeline object as the focus to create a Timeline Asset and instance.

4. Click the padlock button to lock the TImeline window to make it easier to add and adjust tracks.

5. Drag a Unity camera with a CinemachineBrain component onto the Timeline Editor, then choose __Create Cinemachine Track__ from the drop-down menu.

6. Add other tracks to the Timeline for controlling the subjects of your Scene.  For example, add an Animation track to animate your main character.

**Tip**: Delete the default track that refers to your Timeline object. This track isn’t necessary for Timeline. For example, in the Timeline editor, right-click the track for IntroTimeline and choose __Delete__.

To add Cinemachine Shot Clips to a Cinemachine Track:

1. In the Cinemachine Track, right-click and choose __Add Cinemachine Shot Clip__.

2. Do one of the following:

    * To add an existing Virtual Camera to the shot clip, drag and drop it onto the Virtual Camera property in the Cinemachine Shot component.

    * To create a new Virtual Camera and add it to the shot clip, click Create in the Cinemachine Shot component.

3. In the Timeline editor, adjust the order, duration, cutting, and blending of the shot clip.

4. [Adjust the properties of the Virtual Camera](CinemachineVirtualCamera.md) to place it in the Scene and specify what to aim at or follow.

5. To animate properties of the Virtual Camera, create an Animation Track for it and animate as you would any other GameObject.

6. Organize your Timeline tracks to fine-tune your Scene.

