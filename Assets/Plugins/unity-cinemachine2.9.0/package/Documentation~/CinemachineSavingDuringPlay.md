# Saving in Play Mode

It’s often most convenient to adjust camera settings while the game is playing. But normally, Unity does not save your changes to the Scene when you exit Play Mode. Cinemachine has a special feature to preserve the tweaks you make during Play Mode.  It doesn’t save structural changes, like adding or removing a behavior. With the exception of certain properties, Cinemachine preserves most of the settings in your Virtual Cameras when you exit Play Mode.

When you exit Play Mode, Cinemachine scans the Scene to collect any changed properties in the Virtual Cameras.  Cinemachine saves these changes a second or so after exiting. Use the __Edit > Undo__ command to revert these changes.

Check __Save During Play__ on any Virtual Camera in the [Inspector](https://docs.unity3d.com/Manual/UsingTheInspector.html) to enable this feature.  This is a global property, not per-camera, so you only need to check or uncheck it once.

Cinemachine components have the special attribute `[SaveDuringPlay]` to enable this functionality. Feel free to use it on classes within your own custom scripts too if you need it. To exclude a field in a class with the `[SaveDuringPlay]` attribute, add the `[NoSaveDuringPlay]` attribute to the field.

