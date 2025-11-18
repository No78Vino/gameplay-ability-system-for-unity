# Noise properties

Use Noise properties in a Virtual Camera to simulate camera shake. Cinemachine includes a __Basic Multi Channel Perlin__ component, which adds Perlin noise to the movement of the Virtual Camera. __Perlin noise__ is a technique to compute random movement with a natural behavior.

![Choosing the Basic Multi Channel Perlin component to add camera noise](images/CinemachineBasicMultiChannelPerlin.png)

The Basic Multi Channel Perlin component applies a noise profile. A noise profile is an Asset that defines the behavior of noise over time. Cinemachine includes a few noise profile assets. You can [edit these and create your own](CinemachineNoiseProfiles.md).

To apply noise:

1. Select your Virtual Camera in the [Scene](https://docs.unity3d.com/Manual/UsingTheSceneView.html) view or [Hierarchy](https://docs.unity3d.com/Manual/Hierarchy.html) window.

2. In the [Inspector](https://docs.unity3d.com/Manual/UsingTheInspector.html), use the  __Noise__ drop-down menu to choose __Basic Multi Channel Perlin__.

3. In __Noise Profile__, choose an existing profile asset or [create your own profile](CinemachineNoiseProfiles.md).

4. Use __Amplitude Gain__ and __Frequency Gain__ to fine-tune the noise.

## Properties

| **Property:** | **Function:** |
|:---|:---|
| __Noise Profile__ | The noise profile asset to use.|
| __Amplitude Gain__ | Gain to apply to the amplitudes defined in the noise profile. Use 1 to use the amplitudes defined in the noise profile. Setting this to 0 mutes the noise. Tip: Animate this property to ramp the noise effect up and down.|
| __Frequency Gain__ | Factor to apply to the frequencies defined in the noise profile. Use 1 to use the frequencies defined in the noise profile. Use larger values to shake the camera more rapidly. Tip: Animate this property to ramp the noise effect up and down. |
| __Pivot Offset__ | When rotating the camera, offset the camera's pivot by the indicated x, y, and z distance when applying rotational noise.  This generates some positional variation that corresponds to the rotation noise. |



