# Cinemachine Confiner 2D

![](images/CinemachineConfiner2Dc.png)

Use the Cinemachine Confiner 2D [extension](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.6/manual/CinemachineVirtualCameraExtensions.html) to confine the camera’s position so that the screen edges stay within a shape defined by a 2D polygon. This works for orthographic or perspective cameras, provided that the camera's forward vector remains parallel to the bounding shape’s normal (that is, that the camera is looking straight at the polygon, and not obliquely at it).

When confining the camera, the Cinemachine Confiner 2D considers the camera’s view size at the polygon plane, and its aspect ratio. Based on this information and the input polygon, it computes a second (smaller) polygon, and constrains the camera’s transform to it. Computation of this secondary polygon is resource-intensive, so you should only do this when absolutely necessary. 

Necessary use cases in which you need to recompute the cached secondary polygon include:

- when the input polygon’s points change,
- when the input polygon is non-uniformly scaled.

In these cases, for efficiency reasons, Cinemachine does not automatically regenerate the inner polygon. The client needs to call the InvalidateCache() method to trigger the recalculation. You can do this from; 

- the script by calling InvalidateCache, or 
- the component inspector; to do so, press the **Invalidate Cache** button.

If the input polygon scales uniformly or translates or rotates, the cache remains valid. 

## Oversize Windows
If sections of the confining polygon are too small to fully contain the camera window, Cinemachine calculates a polygon skeleton for those regions. This is a shape with no area, that serves as a place to put the camera when it is confined to this region of the shape.

Skeleton computation is the most resource-heavy part of the cache calculation, so it is a good idea to tune this with some care:

- To optimize the skeleton calculation, set the **Max Window Size** property to the largest size you expect the camera window to have. Cinemachine does not spend time calculating the skeleton for window sizes larger than that.


# Properties:

|**Property:**|**Function:**|
|:---|:---|
|Bounding Shape 2D|Set the 2D shape you want to confine the camera viewport to.|
|Damping|Damping Is applied around corners to avoid jumps. Higher numbers are more gradual.|
|Max Window Size|To optimize computation and memory performance, set this to the largest view size that the camera is expected to have. The Confiner 2D does not compute a polygon cache for frustum sizes larger than this. This refers to the size in world units of the frustum at the confiner plane (for orthographic cameras, this is just the orthographic size). If set to 0, then Cinemachine ignores this parameter and calculates a polygon cache for all potential window sizes.|
