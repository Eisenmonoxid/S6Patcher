# Key Features
This excerpt should highlight some of the key features of the S6Patcher. For a full feature list, take a look at the [ReadME](https://github.com/Eisenmonoxid/S6Patcher/blob/master/README.md).

## All Versions: Higher Zoomlevel
- The default zoom limit value of the game is 7200. When patching, the angle of the RTS camera is maintained and other camera behaviours (Cutscene, ThroneRoom, FreeView) are not impacted. The pictures below show a comparison of standard 7200 and 14000 zoom levels. It is recommended to not go higher than 20000.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Zoom_High_Final.png" width="40%" height="40%" alt="Zoom_High"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Zoom_Normal_Final.png" width="40%" height="40%" alt="Zoom_Normal"/>

## All Versions: Windowed Checkbox
- A new checkbox in the options menu allows to start the game either in windowed or in fullscreen mode, depending on the state. 
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Windowed_Final.png" width="35%" height="35%" alt="Windowed"/>

## All Versions: Base game knights in the expansion pack
- All seven knights can be selected in the "Eastern Realm" expansion pack. **Caution:** Some audio feedback lines regarding the "Eastern Realm" features (Tradepost, Well, Geologist) are missing and the stories of some maps might not really make sense (e.g. "Kestrals Wedding" when playing as Kestral).
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Knights_Final.png" width="35%" height="35%" alt="Knights"/>

## All Versions: Single Stop, Downgrade & Military Release
- These new buttons (originally introduced in the NEP by Netsurfer in 2009) were rewritten and are now available in campaign and singleplayer. 
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/726e73400338b8228e0ca2b5aa93ec96d26a963b/Features/Downgrade_Final.png" width="60%" height="60%" alt="Downgrade"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/726e73400338b8228e0ca2b5aa93ec96d26a963b/Features/Military_Release_Final.png" width="60%" height="60%" alt="Military_Release"/>

## All Versions: Day/Night Cycle
- A day/night cycle similiar to the one in Settlers 7. This is entirely cosmetic and has no impact on the gameplay. 
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/53acfce6ab42f9d2ee6f38f1699d6a8742013e4c/Features/Night_Cycle_Final.png" width="60%" height="60%" alt="Night"/>

## Original Release: Texture Resolution
- The S6Patcher can restore the texture slider for entities and set a custom texture resolution for ground textures. The pictures show a comparison between unpatched and highest texture resolution. It is recommended to stay in the interval of 128 - 4096 for ground texture resolution.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Textures_Low_Final.png" width="35%" height="35%" alt="Textures_Low"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Textures_High_Final.png" width="35%" height="35%" alt="Textures_High"/>

## Original Release: Higher Resolutions
- Two new resolutions are added in the ingame video options menu: 2K (2560x1440) and 4K (3840x2160).
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Resolution_Final.png" width="40%" height="40%" alt="Resolution"/>

## Mapeditor: Unlimited Scaling
- Entities in the Mapeditor can now be freely scaled in their size.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Scaling_Final.png" width="40%" height="40%" alt="Scaling"/>

## Mapeditor: All Entities & Textures available and freely placeable
- In the "Place Entities" dialog within the map editor, all possible entities are available. They can be freely placed and selected. **Caution:** Some entities may cause a crash, so it's recommended to save the map before placing an unknown entity. Same goes for ground textures.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/f93c4140ad422dd49f9109154c4fa031f4f3ace5/Features/Menu_Final.png" width="20%" height="20%" alt="Menu"/>

## Mapeditor: Usable black border area
- The black border area of the map can now be utilized and will no longer be deleted when saving the map. If the map area needs to be restricted, the "Map Boundary" menu option can still be used manually.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/f93c4140ad422dd49f9109154c4fa031f4f3ace5/Features/Black_Border_Final.png" width="40%" height="40%" alt="Black_Border"/>

# Recommendation: DXVK
Download the latest DXVK release from the [GitHub](https://github.com/doitsujin/dxvk/releases/latest) and unpack the **d3d9.dll** and **dxgi.dll** files into the game directory, where the "Settlers6.exe" or "Settlers6R.exe" can be found. 
This wrapper will translate the Direct3D 9.0c commands to Vulkan, which is a much more modern graphics API. In the [configuration file](https://github.com/doitsujin/dxvk/blob/master/dxvk.conf), i recommend to use the following flags:
```
dxgi.syncInterval = 1
d3d9.presentInterval = 1
d3d9.samplerAnisotropy = 16
d3d9.forceSwapchainMSAA = 8
```
The first two set VSync (without it, the game runs with an unlimited framerate), the second one sets the anisotropic filtering and the last one sets Multi-Sample Anti-Aliasing. This requires a strong PC, of course.
