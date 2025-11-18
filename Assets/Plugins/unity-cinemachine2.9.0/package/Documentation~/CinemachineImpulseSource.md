# Cinemachine Impulse Source

Use the **Cinemachine Impulse Source** component to generate impulses on events that are not collisions or Collider triggers. This is a generic Impulse Source that exposes a family of `GenerateImpulse()` API methods. These methods generate impulses at the specified locations and with the specified velocities and strengths. Call these methods directly from your game logic, or use them with [UnityEvents](https://docs.unity3d.com/Manual/UnityEvents.html). 

>**TIP:** You can use the script for this component as an example to reference when creating your own custom impulse-generating classes.

To add a Cinemachine Impulse Source to your Scene:

1. Select the GameObject that you want to trigger camera shake, navigate to its Inspector, and click the **Add Component** button.

2. Go to **Scripts > Cinemachine**, and select **Cinemachine Impulse Source**.

By default, an Impulse Source affects every [Impulse Listener](CinemachineImpulseListener.md) in range, but you can apply [channel filtering](CinemachineImpulseFiltering.md#ChannelFiltering) to make Impulse Sources affect some Impulse Listeners and not others. 

##Properties:

The properties in the Cinemachine Impulse Source Inspector window are divided into the following sections.

- [Impulse Channel](#ImpulseChannel)
- [Signal Shape](#SignalShape)
- [Time Envelope](#TimeEnvelope)
- [Spatial Range](#SpatialRange)



<a name="ImpulseChannel"></a>

### Impulse Channel

Impulse Listeners filter impulses based on channels to control which Impulse Sources they react to. Channels work like Camera Layers, but are distinct from them. These properties control the channels that the Impulse Source broadcasts impulse signals on. For details, see documentation on [Filtering](CinemachineImpulseFiltering.md).

![](images/InspectorImpulseSourceChannel.png)

| **Property:**       | **Function:**                                                |
| ------------------- | ------------------------------------------------------------ |
| **Impulse Channel** | Choose one or more channels from the drop-down.<br /><br />Click **Edit** to modify existing channels or add new ones. |

<a name="SignalShape"></a>

### Signal Shape

These properties control the basic characteristics of the raw signal when it is emitted by the Impulse Source.

![](images/InspectorImpulseSourceSignal.png)

| **Property:**  | **Function:**                                                |
| -------------- | ------------------------------------------------------------ |
| **Raw Signal** | The raw signal form. <br /><br />Drag a signal Asset onto the signal name to connect the signal to the Impulse Source.<br /><br />Click the “gear” icon for additional signal options, including options for creating new signals.<br /><br />See [Raw Vibration Signals](CinemachineImpulseRawSignal.md) for details. |
|**Amplitude Gain** | Set a value by which to multiply the amplitude of **Raw Signal**. This controls the strength of the vibrations. This is set to 1 by default.<br /><br />Use values greater than 1 to amplify the signal, 1 for the original signal, values less than 1 and greater than 0 to attenuate the signal, and 0 to mute the signal. |
|**Frequency Gain**|Set a value by which to multiply the frequency of **Raw Signal**. This controls the speed of the vibrations. This is set to 1 by default.<br /><br />Use values greater than one to increase the frequency, 1 for the original signal, values less than 1 and greater than 0 to reduce the frequency of the signal. A frequency of 0 holds a single noise value indefinitely, as though time were frozen.|
|**Randomize**|Enable this to randomize the **Raw Signal’s** start time.<br /><br />This property is available when the **Raw Signal** is a noise profile Asset. See [Raw Vibration Signals](CinemachineImpulseRawSignal.md) for details.|
|**Repeat Mode**|Specify whether to loop the **Raw Signal** over the duration of the time envelope, or stretch it to fit the time envelope.<br /><br />This property is available when the **Raw Signal** is a fixed signal. See [Raw Vibration Signals](CinemachineImpulseRawSignal.md) for details.|

<a name="TimeEnvelope"></a>
### Time Envelope

These properties control the duration of the impulse and the intensity of the **Raw Signal** over that duration.

![](images/InspectorImpulseTimeEnvelope.png)

| Property:             | Function:                                                    |
| --------------------- | ------------------------------------------------------------ |
| **Attack**            | Define how the signal reaches full amplitude at the beginning of the waveform. Click the preview pane (grey bar) to set the shape of the curve, and use the numerical field to set the time (in seconds) to reach the full amplitude. Leave the preview pane blank to use a default curve that is suitable for most purposes. |
| **Sustain Time**      | Set the time to maintain the full amplitude of the waveform after the attack. |
| **Decay**             | Define how the signal goes from full amplitude to zero the end of the waveform. Click the preview pane (grey bar) to set the shape of the curve, and use the numerical field to set the time (in seconds) to mute the signal. Leave the preview pane blank to use a default curve that is suitable for most purposes. |
| **Scale With Impact** | Enable this to apply signal amplitude scaling to **Time Envelope**. If checked, the Cinemachine Impulse Source component scales the time envelope according to the strength of the impact: stronger impacts last longer, and milder impacts have a shorter duration. |


<a name="SpatialRange"></a>
### Spatial Range

These properties define a zone in the Scene. The impulses from this Impulse Source only affect Impulse Listeners in this zone.

![](images/InspectorImpulseSourceSpatialRange.png)

| Property:|| Function:|
|--|--|--|
|**Impact Radius**||Set the radius of the space in the Scene around the impact point where the signal stays at full amplitude. Beyond that the signal strength fades out over the dissipation distance. After that, the strength is zero. In other words, the total effect radius of the signal is **Impact Radius** + **Dissipation Distance**.|
|**Direction Mode**||Define how the Cinemachine Impulse Source component should apply the direction of the signal as the Impulse Listener moves away from the Impulse Source.|
||Fixed|Use **Fixed** to use a constant signal direction.|
||Rotate Toward Source|Use **Rotate Toward Source** to rotate the signal in the direction of the Impulse Source, giving a subtle visual clue about the source’s location. **Rotate Toward Source** has no effect on radially-symmetric signals.|
|**Dissipation Mode**||Define how the signal dissipates when the listener is outside the **Impact Radius**.|
||Linear Decay|The signal dissipates evenly over the **Dissipation Distance**.|
||Soft Decay|The signal dissipates slowly at the beginning of the **Dissipation Distance**, more quickly in the middle, and slowly again as it reaches the end.|
|| Exponential Decay|The signal dissipates very quickly at the beginning of the **Dissipation Distance**, then more and more slowly as it reaches the end.|
|**Dissipation Distance**||Set the distance beyond the impact radius over which the signal decays from full strength to nothing.|
|**Propagation Speed**||The speed (m/s) at which the impulse propagates through space.  High speeds allow listeners to react instantaneously, while slower speeds allow listeners in the scene to react as if to a wave spreading from the source. |