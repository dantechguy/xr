# xr

Unity version: `2022.3.18f1 LTS`

## Collaboration

We should avoid editing the same scene at once. 

Each of us work/experiment on a separate scene, marked with our names. (*e.g. `Lei_AR`*)

Example workflow:
1. Lei works on the debug console feature, then saves the `DebugConsole` gameobject as a prefab
2. Dan and Josh drags the prefab into their scenes, and now the feature exists in their scene as well
3. If Lei updates the debug console, he can update the prefab and the changes will show up in all scenes

I (Lei) have set up 2 scenes: `Dan_Track` and `Josh_Car`, and put some basic 3D models in it.

## Handling Input

I (Lei) suggest using Unity's Input System, it is better than the default `Unity.Input`. (Can be event-based rather than polling, and can map both mouse and touch to the same action)

`Assets -> AR Input Actions` is the action asset we are using. 

I am working on writing an input manager script so we can abstract it away, and you don't need to know the details of how the input system work.

## Logging

I (Lei) suggest using my custom static logging class `XLogger`. 

Usage is very simple: `XLogger.Log("Hello")` ,`XLogger.Log(Category.UI, "Hello")`  
- there are also `LogWarning` and `LogError`
- feel free to add categories to the `Category` enum
- the benefits is first there is a category classification, and we can optionally hook the output to a debug console, so we can see the logs in the phone builds.

## Important Unity Concepts

Here are some key concepts I (Lei) suggest looking up
- Component system and the `MonoBehaviour` class
- Physics: Colliders and RigidBodies
- Materials 
- the `ScriptableObject` class
- Revise OOP and design patterns

## UI

Unity UI can be a bit tricky at times, especially the layouts. Feel free to contack me (Lei) if you need help with UI. The good news is that we don't need to write html and css. 
