# TSEspionage - A Twilight Struggle Mod

TSEspionage is a mod for the Playdek Twilight Struggle client. This mod adds various improvements and addresses bugs.

## Installation (Windows)

First you will need to find your Twilight Struggle install folder. Open Steam and right click on "Twilight Struggle", then click on "Properties...", next click on "Installed Files", finally click "Browse...". This will open the folder that Twilight Struggle is installed into.

Next download the latest version of the mod for Windows. Open the ZIP file. Extract the contents of the ZIP file directly into the Twilight Struggle install directory. You should now have a directory called "TSEspionage" in the same place as the "TwlightStruggle" executable.

![Twilight Struggle Folder](https://github.com/xelrach/TSEspionage/blob/main/docs/Twilight_Struggle_Folder.png?raw=true)

## Uninstall (Windows)

Remove doorstop_config.ini, .doorstop_version, winhttp.dll, and TSEspionage from the Twilight Struggle directory

## Development

If you would like to develop TSEspionage there are a few steps to get set-up:
* TSEspionage is a C# project, so you will likely want to install an IDE that supports C#.
* Use git to clone the repository.
* Open the git directory in your IDE
* NuGet will need to run (probably will be run automatically by your IDE)
* Open TSEspionage/TSEspionage.csproj. This file contains a list of "Reference"s. These are DLLs that we need to copy from the Twilight Struggle client.
* Open Twilight Struggle's install directory and then the "TwilightStruggle_Data/Managed" directory.
* Copy all the files from the list into your repository's "TSEspionage/resources/Third Party" directory.
