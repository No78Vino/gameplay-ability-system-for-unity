# Blending between Virtual Cameras

Use blending properties to specify how the Cinemachine Brain component performs a blend between virtual cameras.

A Cinemachine blend is not a fade, wipe, or dissolve. Rather, Cinemachine Brain performs a smooth animation of the position, rotation, and other settings of the Unity camera from one Virtual Camera to the next.

For blends between specific Virtual Cameras, use the __Custom Blends__ list in the Cinemachine Brain component. Use the __Default Blend__ property in Cinemachine Brain to specify blends between Virtual Cameras that do not have custom blends.

![Custom Blends list in Cinemachine Brain](images/CinemachineCustomBlends.png)

The __From__ and __To__ settings are name-based, not references. This means that Cinemachine finds cameras by matching their names to the settings. They are not linked to specific GameObjects.  The built-in dropdowns can be used to select a virtual camera from the current scene, or the name can be typed directly into the text boxes.  If a name does not match any virtual camera in the current scene, the field will be highlighted in yellow.

Use the reserved name **\*\*ANY CAMERA\*\*** to blend from or to any Virtual Camera.

When Cinemachine begins a transition from one virtual camera to another, it will look in this asset for an entry that matches the upcoming transition, and apply that blend definition.  

- If none is found, then the CinemachineBrain's DefaultBlend setting will apply.  
- If multiple entries in the Custom Blends asset match the upcoming transition, Cinemachine will choose the one with the strongest specificity.  For example, if blending from vcam1 to vcam2, and the custom blends asset contains an entry for _vcam1-to-AnyCamera_, and another entry for _vcam1-to-vcam2_, then the _vcam1-to-vcam2_ entry will apply.
- If multiple entries in the Custom Blends asset match the upcoming transition with equally-strong specificity, then the first one found will apply.

## Properties:

| **Property:** || **Function:** |
|:---|:---|:---|
| __From__ || The name of the Virtual Camera to blend from. Use the name \*\*ANY CAMERA\*\* to blend from any Virtual Camera. This property is available only for custom blends. |
| __To__ || The name of the Virtual Camera to blend to. Use the name \*\*ANY CAMERA\*\* to blend to any Virtual Camera. This property is available only for custom blends. |
| __Style Default Blend__ || Shape of the blend curve. |
| | _Cut_ | Zero-length blend. |
| | _Ease In Out_ | S-shaped curve, giving a gentle and smooth transition. |
| | _Ease In_ | Linear out of the outgoing Virtual Camera, and ease into the incoming Virtual Camera. |
| | _Ease Out_ | Ease out of the outgoing Virtual Camera, and blend linearly into the incoming Virtual Camera. |
| | _Hard In_ | Ease out of the outgoing Virtual Camera, and accelerate into the incoming Virtual Camera. |
| | _Hard Out_ | Accelerate out of the outgoing Virtual Camera, and ease into the incoming Virtual Camera. |
| | _Linear_ | Linear blend. mechanical-looking. |
| | _Custom_ | Custom blend curve. Allows you to draw a custom blend curve. |
| __Time__ || Duration (in seconds) of the blend. |


