# Key Features
This excerpt should highlight some of the key features of the S6Patcher. For a full feature list, take a look at the [ReadME](https://github.com/Eisenmonoxid/S6Patcher/blob/master/README.md).

## All Versions: Higher Zoomlevel
- Standard value is 7200. Compared to "solutions" purely utilising the lua script, the angle of the RTS camera is maintained. Other camera behaviours (Cutscene, ThroneRoom, FreeView) are not impacted. The pictures show a comparison of standard (7200) and 14000 zoom levels. Every value is possible, but it is recommended to not go higher than 20000.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Zoom_High_Final.png" width="40%" height="40%" alt="Zoom_High"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Zoom_Normal_Final.png" width="40%" height="40%" alt="Zoom_Normal"/>

## All Versions: Windowed Checkbox
- A new checkbox in the options menu allows to start the game either in windowed or in fullscreen mode, depending on the state. 
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Windowed_Final.png" width="35%" height="35%" alt="Windowed"/>

## All Versions: Base game knights in the expansion pack
- All seven knights can be selected in the "Eastern Realm" expansion pack. **Caution:** Some audio feedback lines regarding the "Eastern Realm" features (Tradepost, Well, Geologist) are missing and the stories of some maps might not really make sense (e.g. "Kestrals Wedding" when playing as Kestral).
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Knights_Final.png" width="35%" height="35%" alt="Knights"/>

## Original Release: Texture Resolution
- The S6Patcher can restore the texture slider for entities and set a custom texture resolution for ground textures. The pictures show a comparison between unpatched and highest texture resolution. It is recommended to stay in the interval of 128 - 4096 for ground texture resolution.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Textures_Low_Final.png" width="35%" height="35%" alt="Textures_Low"/>
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Textures_High_Final.png" width="35%" height="35%" alt="Textures_High"/>

## Original Release: Higher Resolutions
- Two new resolutions are added in the ingame video options menu: 2K (2560x1440) and 4K (3840x2160).
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Resolution_Final.png" width="40%" height="40%" alt="Resolution"/>

## Mapeditor: Unlimited Scaling
- Entities in the Mapeditor can now be freely scaled in their size.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Scaling_Final.png" width="40%" height="40%" alt="Scaling"/>

## Mapeditor: All Entities & Textures available and freely placeable
- In the "Place Entities" dialog within the map editor, all possible entities are available. They can be freely placed and selected. **Caution:** Some entities may cause a crash, so it's recommended to save the map before placing an unknown entity. Same goes for ground textures.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Menu_Final.png" width="20%" height="20%" alt="Menu"/>

## Mapeditor: Usable black border area
- The black border area of the map can now be utilized and will no longer be deleted when saving the map. If the map area needs to be restricted, the "Map Boundary" menu option can still be used manually.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/76c046da963664caf6438e687e7e700008d92962/Images/Black_Border_Final.png" width="40%" height="40%" alt="Black_Border"/>

# Recommendation: DXVK
Download the latest DXVK release from the [GitHub](https://github.com/doitsujin/dxvk/releases/latest) and unpack the **d3d9.dll** and **dxgi.dll** files into the game directory, where the "Settlers6.exe" or "Settlers6R.exe" can be found. 
This wrapper will translate the Direct3D 9.0c commands to Vulkan, which is much more modern graphics API. In the [configuration file](https://github.com/doitsujin/dxvk/blob/master/dxvk.conf), i personally use the following flags:
```
dxgi.syncInterval = 1
d3d9.presentInterval = 1
d3d9.samplerAnisotropy = 16
d3d9.forceSwapchainMSAA = 8
```
The first two set VSync, second the anisotropic filtering and the last one Multi-Sample Anti-Aliasing. This requires a strong PC, of course.
