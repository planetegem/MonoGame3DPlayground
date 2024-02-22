This is a small tool that I will use when developing my next game, Frikandisland.

The tool has 3 purposes:

1) Load in models and check them for errors in rotation and/or scaling. Models will most likely be made in Blender. The floor consists of tiles that are 1m x1m in Blender (for comparison). Models should be exported with Z-Axis upwards and point of origin at Z = 0.
2) Test animations and see how they interact with controls. For animations, I'll most likely be using the Aether.Extras library.
3) Check collision detection and determine size of bounding circle. Frikandisland will be a 3D game, but all movement will take place on a 2D plain. As such, I only need to test collision detection on that 2D plain. For collision detection, entities will be defined by one or more bounding circle. The bounding circles are scaled and positioned to match the model as closesly/logically as possible. The tool gives you a minimap that shows the 2D plain (where the actual collision detection is taking place) and a preview of the bounding circles on the 3D model.

To inspect the scene, you can drag the mouse to rotate the camera; mouse wheel gives you a zoom function and the arrow keys are used to interact with the player model.
As I start working on Frikandisland, I will be adding extensions of the BasicEntity class for every entity I add to the game (player models, enemies, structures, etc).
