# Cinemachine 3rd Person Aim Extension

This extension is conceived to be a part of a 3rd-person camera rig.  

To preserve aiming accuracy, this extension deliberately cancels out all rotational noise, and forces a hard look at the target point. However, it is still possible to use camera noise with this extension, provided that the noise affects the camera position, instead of the rotation.

See the __AimingRig__ sample scene for an example of this.

Additionally, if _Aim Target Reticle_ is non-null, this extension will project a ray from the Follow target's position and find the first object that collides with that ray. The Aim Target Reticle object will then be placed on that point in the game view, to indicate what the player would hit if a shot were to be fired. This point may be different from what the camera is looking at, if the found object is close enough to be affected by parallax as a result of the offset between player and camera.


## Properties:

| **Property:** | **Function:** |
|:---|:---|
| __Aim Collision Filter__ | Objects on these layers will be detected. |
| __Ignore Tag__ | Objects with this tag will be ignored.  It is a good idea to set this field to the target's tag.  |
| __Aim Distance__ | How far to project the object detection ray.  |
| __Aim Target Reticle__ | This 2D object will be positioned in the game view over the raycast hit point, if any, or will remain in the center of the screen if no hit point is detected. May be null, in which case no on-screen indicator will appear.  |
