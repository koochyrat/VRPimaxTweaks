# VRPimaxTweaks
Subnautica: Below Zero mod to fix culling at edges and double UI for Pimax without having to turn on parallel projections mode. Use together with the main VRTweaks mod. 

Also shrinks the UI and move it to the bottom. Minimal UI for maximum FOV.

Install the main VRTweaks mod first. Then unzip this mod into your Subnautica folder.

If you want to build from source:
1) Install VRTweaks and VRPimaxTweaks mod
2) Run MrPurple's https://github.com/MrPurple6411/AssemblyPublicizer on Assembly-CSharp.dll in your Subnautica folder. Reference this in the next step
3) Open the project in Visual Studio, and add reference to all the missing assemblies with exclamation mark in your Subnautica folder
4) (You can also edit the .csproj file by hand to point to the correct assemblies)
5) Change the post-build event paths to point to your Subnautica mods folder
6) Build the project
