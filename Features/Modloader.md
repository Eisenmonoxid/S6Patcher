# Modloader
The S6Patcher features a simple modloader, which makes it possible to load additional game files without having to overwrite existing files. The files are placed in a special folder, which is then 
loaded by the game on startup.

## Tech
When the "Activate Modloader" - option was checked when patching the game, it will try to load the file "modloader\bba\mod.bba" as first archive file on startup.
