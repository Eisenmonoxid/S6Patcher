# S6Patcher
A simple application that fixes some bugs and adds new features in the various editions of "Settlers 6 - Rise of an Empire". 
<p align="center">
	<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/53acfce6ab42f9d2ee6f38f1699d6a8742013e4c/Features/Header.jpg" width="35%" height="35%" alt="Header"/>
</p>>

---
## Download
Find the latest release of the application [here](https://github.com/Eisenmonoxid/S6Patcher/releases/latest).  
Other download sources could potentially offer outdated versions!

---
## Usage
Select the game executable(s) that you wish to patch (`Settlers6.exe`/`Settlers6R.exe` or `S6MapEditor.exe`/`S6MapEditorR.exe`). These should be located in the installation folder of the game.  
**Should there be any questions, errors or feature requests: [Discord](https://discord.gg/7SGkQtAAET).**

---
## Modloader
The S6Patcher features a simple Modloader, which can be used to create mods without having to modify the existing game files. Look [here](https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Modloader.md).

---
## Features
For illustrated descriptions of some of the key features of the application, take a look at the [Features](https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Features.md) file.

### Original Release
- For the original release (OV) with latest Patch 1.71.
```
- Can restore the texture slider in the graphics menu, so high quality textures are available.
- Can set the quality of ground textures to a custom value.
- Can set the maximum zoom level in the game.
- Can activate the Development Mode.
- Can set the Large Address Aware Flag for more usable virtual memory.
- Can fix Bugs in Script&Code (e.g. "Meldungsstau", "Entertainercrash", "2K & 4K - Resolution").
- Can enable all base game knights in the "Eastern Realm" expansion pack.
- Can enable the Limited/Special Edition (e.g. decorative objects in the base game).
-> Original releases from Steam will have to be extracted with the tool "Steamless" first!
```
### History Edition
- For all History Editions.
```
- Can set the quality of ground textures to a custom value.
- Can activate the Development Mode. (Necessary for e.g. the LuaDebugger to work).
- Can disable the autosave or set a custom timer interval.
- Can set the maximum zoom level in the game.
- Can set the Large Address Aware Flag for more usable memory.
- Can fix Bugs in Script&Code (e.g. "Meldungsstau", "Entertainercrash").
- Can enable all base game knights in the "Eastern Realm" expansion pack.
- Can enable the Limited/Special Edition (e.g. decorative objects in the base game).
-> History Editions from Steam will have to be extracted with the tool "Steamless" first!
```
### Mapeditor
- For all Mapeditor releases.
```
- High resolution textures for Buildings/Entities/Ground.
- Unlimited scaling of the entity size.
- Free placing and moving of entities, walls and wall gates (blocking is ignored).
- Higher general entity limit and move more entities at the same time (100 -> 1055).
- Black map border area can be used and is not deleted at map save.
- Activates the Development Mode.
- Enables walls and wall gates from all climate zones in all climate zones.
- Can place, select and move all possible entity types (Careful, some may crash).
- Can apply all texture types from all climate zones (and some hidden ones too).
- Can open map files that have been protected with the "S6Tools".
- Can set the Large Address Aware Flag for more usable memory.
```
---
## Tech
- The S6Patcher modifies bytes in the game/editor executable. A backup of the executable is created beforehand and can be restored from the application.
- A new folder `Script` in `<Documents>\THE SETTLERS - Rise of an Empire\` and three precompiled lua script files `"UserScriptGlobal.lua"`, `"UserScriptLocal.lua"` and `"EMXBinData.s6patcher"` in said folder are created.  
- The configuration file `<Documents>\THE SETTLERS - Rise of an Empire\Config\Options.ini` is extended with a new section `[S6Patcher]`, where some necessary configuration values are stored.
- When the option "Activate Modloader" is checked when patching, the application creates a new folder `modloader` in the game installation path where modded files can be stored.  