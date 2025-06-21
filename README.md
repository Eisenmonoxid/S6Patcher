# S6Patcher
A simple application that fixes some bugs and adds new features in the various editions of "Settlers 6 - Rise of an Empire". 
<p align="center">
	<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/53acfce6ab42f9d2ee6f38f1699d6a8742013e4c/Features/Header.jpg" width="80%" height="80%" alt="Header"/>
</p>

---
## Download
Find the latest release of the application [here](https://github.com/Eisenmonoxid/S6Patcher/releases/latest).  
Other download sources could potentially offer outdated versions!

---
## Usage
1. Download the latest release and launch the `S6Patcher.exe`.
2. Click the button `"Choose File ..."` and select the game executable that you wish to patch (`Settlers6.exe`/`Settlers6R.exe` or `S6MapEditor.exe`/`S6MapEditorR.exe`). These should be located in the installation folder of the game.  
3. Select the features that you wish to apply (or choose the recommended preset).
4. Click on the button `Patch` and wait for the process to finish.
5. Close the application and start the game/editor as usual.

In case your game is from `Steam`, you will have to use the tool `Steamless` before patching.

**Should there be any questions or errors: [Discord](https://discord.gg/7SGkQtAAET).**

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
- Can set the quality of ground textures to a custom resolution.
- Can set the maximum zoom level in the game.
- Can activate the Development Mode. (Necessary for e.g. the LuaDebugger to work).
- Can set the Large Address Aware Flag for more usable virtual memory.
- Can fix multiple bugs and errors in Script & Code (e.g. "Meldungsstau", "Entertainercrash", "2K & 4K - Resolution").
- Can enable all base game knights in the "Eastern Realm" expansion pack.
- Can enable the Limited/Special Edition (e.g. decorative objects in the base game).
```
### History Edition
- For all History Editions.
```
- Can set the quality of ground textures to a custom resolution.
- Can activate the Development Mode. (Necessary for e.g. the LuaDebugger to work).
- Can disable the autosave or set a custom timer interval.
- Can set the maximum zoom level in the game.
- Can set the Large Address Aware Flag for more usable virtual memory.
- Can fix multiple bugs and errors in Script & Code (e.g. "Meldungsstau", "Entertainercrash").
- Can enable all base game knights in the "Eastern Realm" expansion pack.
- Can enable the Limited/Special Edition (e.g. decorative objects in the base game).
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
- Can set the Large Address Aware Flag for more usable virtual memory.
```
---
## Tech
- The S6Patcher modifies bytes in the game/editor executable. A backup of the executable is created beforehand and can be restored from the application.
- A new folder `Script` in `<Documents>\THE SETTLERS - Rise of an Empire\` and three lua script files `"UserScriptGlobal.lua"`, `"UserScriptLocal.lua"` and `"EMXBinData.s6patcher"` in said folder are created.  
- The configuration file `<Documents>\THE SETTLERS - Rise of an Empire\Config\Options.ini` is extended with a new section `[S6Patcher]`, where some necessary configuration values are stored.
- When the option "Activate Modloader" is checked when patching, the application creates a new folder `modloader` in the game installation path where modded files can be stored.  

## Linux Support
The S6Patcher is developed using .NET Framework 4.8 with C# 7.3 and WinForms. Linux support has been tested using [Mono](https://www.mono-project.com/download/stable/). 
[libgdiplus](https://www.mono-project.com/docs/gui/libgdiplus/) may also be needed to run the application on your Linux distribution.