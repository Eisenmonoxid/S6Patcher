# S6Patcher
A simple application that fixes some bugs and adds new features in the various editions of "The Settlers 6 - Rise of an Empire". 
<p align="center">
	<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Header.jpg?raw=true" width="80%" height="80%" alt="Header"/>
</p>

---
## Features
#### Find a comprehensive list of all features in the [Features](https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Features.md) file.

---
## Download
Find the latest release of the application [here](https://github.com/Eisenmonoxid/S6Patcher/releases/latest).  

---
## Usage
1. Make sure that your game has been launched successfully `at least once` before using the S6Patcher.
2. Check that the latest official patch `1.71.4289` is installed (main menu bottom right corner).
3. Download the [latest release](https://github.com/Eisenmonoxid/S6Patcher/releases/latest) and launch the `S6Patcher.exe`.
4. Click the button `"Choose File ..."` and select the game executable that you wish to patch (`Settlers6.exe`/`Settlers6R.exe` or `S6MapEditor.exe`/`S6MapEditorR.exe`). These should be located in the installation folder of the game.  
5. Select the features that you wish to apply.
6. Click on the button `Patch File` and wait for the process to finish.
7. Close the application and start the game/editor as usual.

In case your game is from `Steam`, you will have to use the tool `Steamless` before patching.

When patching, the application will create a backup of the original file. To restore this backup, simply choose the 
previously patched .exe file and use the button `Restore Backup`.

**Should there be any questions or errors: [Discord](https://discord.gg/7SGkQtAAET).**

---
## Modloader
The S6Patcher features a simple [Modloader](https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Modloader.md), which can be used to create mods without having to modify the existing game files.

---
## Tech
- The S6Patcher modifies bytes in the game/editor executable. A backup of the executable is created beforehand and can be restored from the application.
- A new folder `Script` in `<Documents>\THE SETTLERS - Rise of an Empire\` and three lua script files `"UserScriptGlobal.lua"`, `"UserScriptLocal.lua"` and `"EMXBinData.s6patcher"` in said folder are created.  
- The configuration file `<Documents>\THE SETTLERS - Rise of an Empire\Config\Options.ini` is extended with a new section `[S6Patcher]`, where some necessary configuration values are stored.
- When the option "Activate Modloader" is checked when patching, the application creates a new folder `modloader` in the game installation path where modded files can be stored.  
- The option "Download and Install Bugfix Mod" will download the zip compressed folder `Modfiles.zip` from the repository directory `/Source/Resources/Gamefiles` and extract
it into the `modloader` folder. 

---
## Contribution
In case you want to contribute your own features and/or fixes to the project, feel free to fork the repository and create a pull request.   
**Important:** Everything contributed to the project must either be created by yourself or approved by the original author(s).