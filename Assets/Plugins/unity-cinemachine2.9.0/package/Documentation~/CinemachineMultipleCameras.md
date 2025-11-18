# Multiple Unity cameras

Split-screen and picture-in-picture effects require the use of more than one Unity camera. Each Unity camera presents its own view on the player’s screen.

To use a multi-camera split-screen for two players:

1. For each player, [create a layer](https://docs.unity3d.com/Manual/Layers.html). For example, for two players, create layers named P1 and P2.

2. Add two Unity cameras to your Scene, set up their viewports, and give each one its own Cinemachine Brain component.

3. For each Unity camera, set the __Culling Mask__ to the appropriate layer while excluding the other layer. For example, set the first Unity camera to include layer P1 while excluding P2.

4. Add 2 Virtual Cameras, one to follow each player to follow the players. Assign each Virtual Camera to a player layer.

