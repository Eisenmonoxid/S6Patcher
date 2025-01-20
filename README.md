# S6Patcher
---
## Download
- Find the latest release of the application [here](https://github.com/Eisenmonoxid/S6Patcher/releases/latest). Other download sources could potentially offer outdated versions.
- _Die neueste Version des Programmes findet sich [hier](https://github.com/Eisenmonoxid/S6Patcher/releases/latest) zum Download. Andere Downloadquellen könnten unter Umständen veraltete Versionen anbieten._
---
## Usage
- Select the game executable(s) that you wish to patch (Settlers6.exe/Settlers6R.exe or S6MapEditor.exe/S6MapEditorR.exe). Look in your installation folder.  
**Should you have any questions, errors or feature requests: [Discord](https://discord.gg/7SGkQtAAET).**

- _Wähle die ausführbare(n) Datei(en), die gepatcht werden soll(en) (Settlers6.exe/Settlers6R.exe oder S6MapEditor.exe/S6MapEditorR.exe) aus, welche sich im Installationsordner befindet/n.  
**Sollte es Fragen, Fehler oder Featurewünsche geben: [Discord](https://discord.gg/7SGkQtAAET).**_
---
## Features
- For illustrated descriptions of some of the key features of the application, take a look at the [Features](https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Features.md) file.
- _Eine bebilderte Beschreibung einiger der Hauptfeatures des Programmes findet sich in der [Features](https://github.com/Eisenmonoxid/S6Patcher/blob/master/Features/Features.md) Datei._

### Original Release
- For the original release (OV) with latest Patch 1.71.
- _Für die Originalversion (OV) mit neuestem Patch 1.71._

German:
```
- Kann den Textur-Slider im Grafikmenü reaktivieren, um hohe Texturqualität auswählen zu können.
- Kann die Qualität von Bodentexturen auf einen benutzerdefinierten Wert setzen.
- Kann das maximale Zoomlevel setzen.
- Kann das Large Address Aware Flag setzen, um mehr adressierbaren Speicher zu haben.
- Kann den Development-Mode ohne Registry-Key dauerhaft aktivieren.
- Kann Script&Code-Bugs fixen (z.B. "Meldungsstau", "Entertainercrash", "2K & 4K - Auflösung").
- Kann die Auswahl aller Grundspielritter im "Reich des Ostens" - Addon ermöglichen.
- Kann die Limited/Special Edition aktivieren (zB Zierobjekte im Grundspiel).
-> Originalversionen von Steam müssen zuerst mit dem Tool "Steamless" entpackt werden!
```
English:
```
- Can restore the texture slider in the graphics menu, so high texture quality can be choosen.
- Can set the quality of ground textures to a custom value.
- Can set the maximum zoom level in the game.
- Can activate the Development-Mode permanently without Registry-Key.
- Can set the Large Address Aware Flag for more usable memory.
- Can fix Bugs in Script&Code (e.g. "Meldungsstau", "Entertainercrash", "2K & 4K - Resolution").
- Can enable all base game knights in the "Eastern Realm" expansion pack.
- Can enable the Limited/Special Edition (e.g. decorative objects in the base game).
-> Original releases from Steam will have to be extracted with the tool "Steamless" first!
```
### History Edition
- For all History Editions.
- _Für alle History Editionen._

German:
```
- Kann die Qualität von Bodentexturen auf einen benutzerdefinierten Wert setzen.
- Kann den Development-Mode dauerhaft aktivieren (Notwendig für die Funktion des LuaDebuggers).
- Kann den Autosave deaktivieren oder ein benutzerdefiniertes Intervall setzen.
- Kann das maximale Zoomlevel setzen.
- Kann das Large Address Aware Flag aktivieren, um mehr adressierbaren Speicher zu haben.
- Kann Script&Code-Bugs fixen (z.B. "Meldungsstau", "Entertainercrash").
- Kann die Auswahl aller Grundspielritter im "Reich des Ostens" - Addon ermöglichen.
- Kann die Limited/Special Edition aktivieren (zB Zierobjekte im Grundspiel).
-> History Editionen von Steam müssen zuerst mit dem Tool "Steamless" entpackt werden!
```
English:
```
- Can set the quality of ground textures to a custom value.
- Can activate the Development-Mode permanently (Necessary for e.g. the LuaDebugger to work).
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
- _Für alle Mapeditorversionen._

German:
```
- Hohe Texturen für Gebäude/Entitäten/Boden.
- Unbegrenztes Skalieren der Größe von Entitäten.
- Freies Platzieren und Ineinander-Verschieben von Entitäten, Mauern und Mauertoren (Blocking wird ignoriert).
- Erhöhen des allgemeinen Entitätenlimits und mehr Entitäten gleichzeitig Bewegen (100 -> 1055).
- Schwarzer Kartenrand wird beim Speichervorgang nicht gelöscht und kann verwendet werden.
- Dauerhaftes Aktivieren des Development-Modes ohne Registry-Key.
- Mauern und Mauertore aus allen Klimazonen können gesetzt werden.
- Alle möglichen Entitäten können platziert, selektiert und verschoben werden (Achtung, einige führen zum Absturz).
- Alle Texturtypen von allen Klimazonen können aufgetragen werden (Auch einige Versteckte).
- Kann Mapdateien öffnen, welche zuvor mit den "S6Tools" geschützt wurden.
- Kann das Large Address Aware Flag aktivieren, um mehr adressierbaren Speicher zu haben.
```
English:
```
- High resolution textures for Buildings/Entities/Ground.
- Unlimited scaling of the entity size.
- Free placing and moving of entities, walls and wall gates (blocking is ignored).
- Higher general entity limit and move more entities at the same time (100 -> 1055).
- Black map border area can be used and is not deleted at map save.
- Activates the Development-Mode without Registry-Key.
- Enables walls and wall gates from all climate zones in all climate zones.
- Can place, select and move all possible entity types (Careful, some may crash).
- Can apply all texture types from all climate zones (and some hidden ones too).
- Can open map files that have been protected with the "S6Tools".
- Can set the Large Address Aware Flag for more usable memory.
```
---
## Tech
- The S6Patcher modifies bytes in the game/editor executable. A backup of the executable is created beforehand and can be restored from the application.
- A new folder `"Script"` in `<Documents>\THE SETTLERS - Rise of an Empire\` and two lua script files `"UserScriptLocal.lua"` and `"EMXBinData.s6patcher"` in said folder are created.  
While UserScriptLocal is in plain text and can be modified, EMXBinData is the minified and precompiled `"MainMenuUserScript.lua"`. 
- The configuration file `<Documents>\THE SETTLERS - Rise of an Empire\Config\Options.ini` is extended with a new section `[S6Patcher]`, where some values are stored.

The Patcher is developed with the .NET Framework 4.8 and is therefore compatible with Windows 7 SP1 up to the latest Windows OS (Windows 11 at the time of writing).  
The application uses WinForms, so Linux support via e.g. [Mono](https://en.wikipedia.org/wiki/Mono_(software)) could be possible, but is currently untested.