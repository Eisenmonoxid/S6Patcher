# Modloader
The S6Patcher features a simple modloader, which makes it possible to load additional game files without having to overwrite/replace any existing files. 
The files are placed in a special folder or packed into an archive file, which is then loaded by the game on startup.

## Tech
### Original Release
When the `"Activate Modloader"` - option was checked when patching the game, it will try to load the file `"modloader\bba\mod.bba"` as first archive file on startup. If the file does not
exist, the game will display an error message and abort the boot process.

### History Edition
Here, the process is a bit different: Since the game does not use archive files, it will add the folder path `"modloader\shr"` to its internal directory manager. The files in this
paths are loaded by the game first before loading anything else. If the path does not exist, the game will launch normally without notification.

## Creating a mod
Use the [bba6 - Tool](https://github.com/mcb5637/bba6tool) (written by yoq and mcb) to unpack your game files (.bba) and to create a `"mod.bba"` containing your modified game files and additional files. For the History Editions,
it is not necessary to use the bba6 - Tool, you can directly put any modified files in the folder mentioned above.  
**Note:** The folder structure of the game has to be retained in the `mod.bba` / the `modloader/shr` folder.

```
The full path in the Original Release should look something like this: <Program Files>\THE SETTLERS - Rise of an Empire\modloader\bba\mod.bba
In the History Edition, the path to the modified files looks like this: <Program Files>\Ubisoft\thesettlers6\modloader\shr\<Your modded files>
```

**In case there are any questions: [Discord](https://discord.gg/7SGkQtAAET).**