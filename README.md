# DiscordRichPresence for Beat Saber

A Beat Saber mod that provides rich presence status to Discord using the `DiscordCore` library.

## Features
- Shows when you are in the Menu, Settings, or Level Selection.
- During gameplay, displays the song name, author, difficulty, energy %, and current combo.
- Native Discord progress bar for track duration.
- Displays match results at the end of the song (Rank, Combo, Notes Cut, Score).

## Requirements
- **BSIPA** (Beat Saber IPA)
- **SiraUtil**
- **DiscordCore** (https://github.com/WentTheFox/DiscordCore)

## Build Instructions
1. Clone this repository into a folder on your PC.
2. Open `DiscordRichPresence.csproj` in an editor or Visual Studio.
3. Make sure the `<BeatSaberDir>` property inside the `.csproj` correctly points to your Beat Saber installation folder.
4. Replace `AppId` in `Services/DiscordPresenceManager.cs` with your actual Discord application ID from the Discord Developer Portal.
5. Compile using Visual Studio or `dotnet build`. The output DLL needs to be placed into `Beat Saber/Plugins`.

## Limitations & Fragility
This mod relies on specific Beat Saber classes (`GameEnergyCounter`, `ComboController`, `AudioTimeSyncController`, and `ResultsViewController`). Since Beat Saber updates frequently, class/method names may change. 
- **Harmony Patches:** The mod hooks into `ResultsViewController.Init` and `HMUI.ViewController.DidActivate` via Harmony. If these methods change their signature in future game updates, the mod will need to be updated.
- **Images:** Due to Discord constraints, custom cover art requires an internet URL (e.g. from BeatSaver API) or pre-uploaded Developer Portal assets. The mod uses a `default_icon` key for now.
