# Modloader
The S6Patcher features a simple modloader, which makes it possible to load additional game files without having to overwrite/replace any existing files. 
The files are, depending on the game version, placed in a special folder or packed into an archive file, which is then loaded by the game on startup.

---

## Tech
### Original Release
When the `"Activate Modloader"` - option was checked when patching the game, it will try to load the file `"modloader\bba\mod.bba"` as its first archive file on startup. If the file does not
exist, the game will display an error message and abort the boot process.

### History Edition
Here, the process is a bit different: Since the game does not use archive files, it will add the folder path `"modloader\shr"` to its internal directory manager. The files in this
path are loaded by the game first before loading anything else. If the path does not exist, the game will launch normally without notification.

---

## Creating a mod
When creating a mod for the OV, use the [bba6-Tool](https://github.com/mcb5637/bba6tool) (written by yoq) to unpack your game files (.bba) and to create a `"mod.bba"` containing your modified game files. For the History Editions,
it is not necessary to use the bba6-Tool, you can directly put any modified files in the folder mentioned above.  
**Note:** The folder structure of the game has to be retained in the `mod.bba` file / the `modloader/shr` folder.

```
The full path in the Original Release should look something like this: <Program Files>\THE SETTLERS - Rise of an Empire\modloader\bba\mod.bba
In the History Edition, the path to the modified files should look like this: <Program Files>\Ubisoft\thesettlers6\modloader\shr\<Your modded files>
```

---

## Small example mod (History Edition)
We want to change the default player color to yellow. So we locate the file that contains the player color mapping `<Settlers>\Data\base\shr\config\playercolor.xml`. We copy this file to to the modloader path
(the Patcher should have created the folders, if not, simply create them yourself). We have to rebuild the folder structure of the game in the `modloader\shr\` folder, so the game recognizes the modified file.
This should look like this: `<Settlers>\modloader\shr\config\playercolor.xml`.
After that, we can open the .xml file in any editor and replace the second entry with our own custom RGB colors:
```xml
		<!-- 4 City colors / player colors-->		
		<Color>
			<Red>255</Red>
			<Green>255</Green>
			<Blue>0</Blue>
		</Color>
```
Save the file, start the game, and the changes should have been applied.
<img src="https://github.com/Eisenmonoxid/S6Patcher/blob/28b561a3ac6f39ad59e13dd84a3cb77610fad7fd/Features/Playercolor_Final.png" width="50%" height="50%" alt="Player_Color"/>

For the Original Release, the process is basically the same, only difference is that the folders and the file must be packed into a .bba - archive with the bba6-Tool. The resulting .bba file must be named `mod.bba` and
must be located in the following path: `<Settlers>\modloader\bba\mod.bba`.

---

## Questions
**In case there are any questions: [Discord](https://discord.gg/7SGkQtAAET).**