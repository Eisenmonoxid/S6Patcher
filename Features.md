# Key Features
This excerpt should explain the key features of the S6Patcher.

## All Versions: Higher Zoomlevel
- Standard value is 7200. Compared to "solutions" purely utilising the lua script, the angle of the RTS camera is maintained. Other camera behaviours (Cutscene, ThroneRoom, FreeView) are not impacted. The pictures show a comparison of standard (7200) and 14000 zoom levels. Every value is possible, but it is recommended to not go higher than 20000.
<img src="https://github.com/user-attachments/assets/6ea2d8cf-ed35-47a2-8439-14e02f768f6a" width="40%" height="40%" alt="Zoom_High"/>
<img src="https://github.com/user-attachments/assets/a9cd4dd8-d0de-4f5f-b9ec-55eb776d166d" width="40%" height="40%" alt="Zoom_Normal"/>

## All Versions: Windowed Checkbox
- A new checkbox in the options menu allows to start the game either in windowed or in fullscreen mode, depending on the state. 
<img src="https://github.com/user-attachments/assets/0f43051f-9bb6-49d6-a48c-fce7e299702a" width="35%" height="35%" alt="Windowed"/>

## All Versions: Base game knights in the expansion pack
- All seven knights can be selected in the "Eastern Realm" expansion pack. **Caution:** Some audio feedback lines regarding the "Eastern Realm" features (Tradepost, Well, Geologist) are missing and the stories of some maps might not really make sense (e.g. "Kestrals Wedding" when playing as Kestral).
<img src="https://github.com/user-attachments/assets/c51b161b-d8b5-4de0-aefd-ca383ae52067" width="35%" height="35%" alt="Knights"/>

## Original Release: Texture Resolution
- The S6Patcher can restore the texture slider for entities and set a custom texture resolution for ground textures. The pictures show a comparison between unpatched and highest texture resolution. It is recommended to stay in the interval of 128 - 4096 for ground texture resolution.
<img src="https://github.com/user-attachments/assets/916b09a8-1b25-419e-a1d1-ecd3bf8b8cc6" width="35%" height="35%" alt="Textures_Low"/>
<img src="https://github.com/user-attachments/assets/aeb55f99-254f-41a9-9daf-d97817810db3" width="35%" height="35%" alt="Textures_High"/>

## Original Release: Higher Resolutions
- Two new resolutions are added in the ingame video options menu: 2K (2560x1440) and 4K (3840x2160).
<img src="https://github.com/user-attachments/assets/aefa8ae8-fd8f-41b5-903f-964ae46dca0c" width="40%" height="40%" alt="Resolution"/>

## Mapeditor: Unlimited Scaling
- Entities in the Mapeditor can now be freely scaled in their size.
<img src="https://github.com/user-attachments/assets/81ff17ce-6c77-4ace-ba51-77b21bc66666" width="40%" height="40%" alt="Scaling"/>

## Mapeditor: All Entities & Textures available and freely placeable
- In the "Place Entities" dialog within the map editor, all possible entities are available. They can be freely placed and selected. **Caution:** Some entities may cause a crash, so it's recommended to save the map before placing an unknown entity. Same goes for ground textures.
<img src="https://github.com/user-attachments/assets/1b143e2b-fde0-46aa-8fe3-2324dc88aa1b" width="20%" height="20%" alt="Menu"/>

## Mapeditor: Usable black border area
- The black border area of the map can now be utilized and will no longer be deleted when saving the map. If the map area needs to be restricted, the "Map Boundary" menu option can still be used manually.
<img src="https://github.com/user-attachments/assets/b7d41826-e551-4c19-bef4-88f4227d54f0" width="40%" height="40%" alt="Black_Border"/>

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
