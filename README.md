## This fork changes

- Add folder and item button;
- VS Code Editor instead of ScintillaNET;
- Exporting progress bar. No more "App is not responding";
- .NET 8 instead of .NET Framework.
- Drag and drop files
- Dark theme by [@Dready](https://github.com/Dreary)
- Convert USM files to MP4
- VLC Player for MP4 files

# Orion2 Repacker

Orion2 Repacker - A HaRepacker for MapleStory2!

[![image](https://github.com/AngeloTadeucci/Orion2-Repacker/assets/15664821/6943f397-17c0-46ec-b098-fd9397947f9a)](https://github.com/AngeloTadeucci/Orion2-Repacker)

---

## General Features

- Supports all MS2 file formats (MS2F, NS2F, OS2F, and PS2F)
- Loads the file list into a expandable tree view for easy access
- Selecting a node will render it, as well as display actions for modifying it
- Double-clicking a node will expand the directory and add the new entries to tree
- Ability to add, remove, copy, paste, and modify any of the data
- Ability to export the selected node's file data to disk
- Full saving support for all file formats

## Rendering

Currently, the Orion2 Repacker is able to display the following formats:

- Initialization Files (.ini)
- N-Triple Files (.nt)
- LUA Files (.lua)
- XML Files (.xml)
- FLAT Files (.flat)
- XBlock Files (.xblock)
- Database Diagram Files (.diagram)
- Preset Files (.preset)
- PNG Image Files (.png)
- DDS Image Files (.dds)
- USM Movie Files (.usm)

The following are currently unable to be rendered, but these external applications can:

- Gamebryo NIF/KF Files (.nif, .kf, .kfm) - [Noesis](https://richwhitehouse.com/index.php?content=inc_projects.php&showproject=91)

## Dependencies

The Orion2 Repacker utilizes these great libraries:

- [Monaco Editor](https://github.com/microsoft/monaco-editor) The Monaco Editor is the code editor that powers VS Code.
- [Pfim](https://github.com/nickbabcock/Pfim) DDS decoder
- [VGMToolbox](https://sourceforge.net/projects/vgmtoolbox/) USM decoder
  - Thanks [https://github.com/Rikux3/UsmToolkit](https://github.com/Rikux3/UsmToolkit) for the examples
- [vgmstream](https://vgmstream.org/) .adx audio decoder
- [libVLC](https://www.videolan.org/vlc/libvlc.html) VLC Player