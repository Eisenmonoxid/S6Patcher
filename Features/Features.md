# Key Features
This excerpt should highlight some of the key features of the S6Patcher. Find a full feature list below.

---
## All Versions: Higher Zoom Level
- The default zoom limit value of the game is `7200`. When patching, the angle of the RTS camera is maintained and other camera behaviours (Cutscene, ThroneRoom, FreeView) are not impacted. The pictures below show a comparison of default `7200` and `14000` zoom levels. It is recommended to not go higher than `20000`.

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Zoom_High_Final.jpg?raw=true" width="40%" height="40%" alt="Zoom_High"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Zoom_Normal_Final.jpg?raw=true" width="40%" height="40%" alt="Zoom_Normal"/>

## All Versions: Ingame Options Menu
- A new menu in the options menu allows to toggle some of the features of the S6Patcher ingame.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Ingame_Menu_Final.jpg?raw=true" width="60%" height="60%" alt="Windowed"/>

## All Versions: Base Game Knights in the Expansion Pack
- All seven knights can be selected in the "Eastern Realm" expansion pack. 
**Caution:** Some audio feedback lines regarding the "Eastern Realm" features (Tradepost, Well, Geologist) are missing and the stories of some maps might not really make sense (e.g. "Kestrals Wedding" when playing as Kestral).

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Knights_Final.jpg?raw=true" width="35%" height="35%" alt="Knights"/>

## All Versions: Special Knights (Crimson Sabatt & Red Prince)
- The antagonistic knights "Crimson Sabatt" and the "Red Prince" can be selected in singleplayer maps in the base game and additionally in the "Eastern Realm" expansion pack.
**Caution:** The stories of some maps might not really make sense when playing as these knights.

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/SpecialKnights_Final.jpg?raw=true" width="45%" height="45%" alt="SpecialKnights"/>

## All Versions: Single Stop, Downgrade & Military Release
- These new buttons (originally introduced in the NEP by Netsurfer in 2009) were rewritten and are now available in campaign and singleplayer. 

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Downgrade_Final.jpg?raw=true" width="40%" height="40%" alt="Downgrade"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Military_Release_Final.jpg?raw=true" width="40%" height="40%" alt="Military_Release"/>

## All Versions: Day/Night Cycle
- A day/night cycle similar to the one in Settlers 7. This is entirely cosmetic and has no impact on the gameplay. 

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Night_Cycle_Final.jpg?raw=true" width="60%" height="60%" alt="Night"/>

## Original Release: Texture Resolution
- The S6Patcher can restore the texture slider for entities and set a custom texture resolution for ground textures. The pictures show a comparison between unpatched and highest texture resolution. It is recommended to stay in the interval of 128 - 4096 for ground texture resolution.

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Textures_Low_Final.jpg?raw=true" width="35%" height="35%" alt="Textures_Low"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Textures_High_Final.jpg?raw=true" width="35%" height="35%" alt="Textures_High"/>

## Original Release: Higher Resolutions
- Two new resolutions are added in the ingame video options menu: 2K (2560x1440) and 4K (3840x2160).

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Resolution_Final.jpg?raw=true" width="40%" height="40%" alt="Resolution"/>

## Mapeditor: Unlimited Scaling
- Entities in the Mapeditor can now be freely scaled in their size.

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Scaling_Final.jpg?raw=true" width="40%" height="40%" alt="Scaling"/>

## Mapeditor: All Entities & Textures available and freely placeable
- In the "Place Entities" dialog within the map editor, all possible entities are available. They can be freely placed and selected. **Caution:** Some entities may cause a crash, so it's recommended to save the map before placing an unknown entity. Same goes for ground textures.

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Menu_Final.jpg?raw=true" width="20%" height="20%" alt="Menu"/>

## Mapeditor: Usable black border area
- The black border area of the map can now be utilized and will no longer be deleted when saving the map. If the map area needs to be restricted, the "Map Boundary" menu option can still be used manually.

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Black_Border_Final.jpg?raw=true" width="40%" height="40%" alt="Black_Border"/>

---
## Additional Features and Bugfixes
### All Versions
- "Meldungsstau": Caused the feedback message queue to become stuck, resulting in no further messages being displayed.
- "Entertainercrash": Caused the game to crash when an entertainer was selected and send back.
- Middle Europe NPC barracks will now correctly spawn soldiers as intended.
- Salt and Dye slots are added to city storehouses so they can be traded.
- The big cathedral displays the correct name `Cathedral` instead of `B_Cathedral_Big` when selected.
- When selecting wooden fences, the `Build Stone Wall` button is no longer displayed.
- The Limited/Special edition is activated, which e.g. enables decorative objects in the base game.
- `Mouse Scroll Speed` and `Keyboard Scroll Speed` sliders in the game options menu are now correctly set from the configuration file.
- The Large Address Aware flag can be set, allowing the game to use more virtual memory.
- The Development Mode can be activated. (Necessary for e.g. the LuaDebugger to work).
- Makes it possible to mod the game without having to modify the existing game files (Sideloading).
- The PE Header file CheckSum is now recalculated correctly. This should fix the issue with the game not starting on some systems.
- Added an Easy Debug mode, which halts the main thread on boot up and allows to attach a debugger easily.
- When pressing `SHIFT` when selecting all units, ammunition carts will now also be selected.
- When pressing `CTRL` when selecting all units, Thiefs will now no longer be selected.
- Campfires from bandit camps can now no longer crash the game under certain circumstances.
- Base Game campaign fixes:
  - Mission 16: The allied knights now have their correct names and entity types instead of Marcus.
  - Mission 3: The harbour now has the correct name.
  - Mission 11: The victory message by the Red Prince now has the correct portrait and voice message.
  - Mission 15: The reinforcements are now correctly spawned.
  - Mission 13: The Red Prince and the Harbor are now actually enemies and attacks are therefore possible.

### Original Release
- The parental control check is disabled. This caused the game to fail to launch when a certain Win32 DLL file was missing on the system.
- Multiple instances of the game can be launched at the same time, even when the Development Mode is not active.
- Remote session game start is possible.

### History Edition
- The autosave can be disabled or set to a custom timer interval.
- `GUI.SendScriptCommand` is now fully working in the Multiplayer.

### Mapeditor
- Free placing and moving of entities, walls and wall gates (all blocking is ignored).
- Higher general entity limit and move more entities at the same time (100 -> 1055).
- Enables walls and wall gates from all climate zones in all climate zones.
- Can open map files that have been protected with the "S6Tools".

### Bugfix Mod
- Two Pass Alpha Blending (Reflections) and Shadows are enabled for the following ship models:
  - `D_X_Kogge`
  - `D_X_Dragonboat01`
  - `D_X_Dragonboat02`
  - `D_X_VikingBoat`   

- This will also fix the problem with `D_X_VikingBoat` not being rendered correctly in the game.   

<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Reflection_Final.jpg?raw=true" width="40%" height="40%" alt="Reflection"/>

- An unused festival music track will now play when the player starts a festival (50% chance for either the original or the new track to play).
- Middle Europe NPC barracks will now correctly spawn soldiers as intended (This is now also directly fixed in the configuration file).
- Polar bears are now using the correct "DIE" animation instead of "WALK" when killed.

---
# Recommendation: DXVK
Download the latest DXVK release from the [GitHub](https://github.com/doitsujin/dxvk/releases/latest) repository and unpack the **d3d9.dll** and **dxgi.dll** files into the game directory, where the "Settlers6.exe" or "Settlers6R.exe" can be found. 
This wrapper will translate the Direct3D 9.0c commands to Vulkan, which is a much more modern graphics API. In the [configuration file](https://github.com/doitsujin/dxvk/blob/master/dxvk.conf), i recommend to use the following flags:
```
d3d9.presentInterval = 1
d3d9.samplerAnisotropy = 16
```
The first option enables vertical synchronisation (without it, the game runs at an unlimited framerate), the second one sets the anisotropic filtering level. It will also provide a higher framerate in CPU - bound situations.
