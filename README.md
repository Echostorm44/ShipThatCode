# ShipThatCode
 A simple utility to create an installer and updater for .net projects without code changes using Github to host updates.

![image](https://github.com/Echostorm44/ShipThatCode/assets/107306362/949b09ca-1f17-4148-9a36-bb9766e8290a)

ShipThatCode is my version of ClickOnce.  It is crazy to me that all the options for creating installers for and keeping windows programs up to date are such a convoluted time consuming mess that are sometimes more work than the product itself.

Here is the use case. I have an application that runs on Windows that I want to create a simple but attractive installer for and to have it check for updates each time it launches without having to add any code to my application.

ShipThatCode builds 2 files.  An installer.exe and a zip that the launcher uses to update users on older versions.  

installer.exe is a sort of self extrtracting archive that will 
1. Extract your program to a temp folder
2. Run the setup.exe which is included
3. Show the user your EULA
4. Ask them where they want to install
5. Copy the files
6. Create a proper shortcut on the desktop that points to an updater program that will look in your Github releases for the most recent version of the zip file which it will download and unzip over the local version.

This is as simple for you the developer and the end user as I can make it.
All you have to do is fill out a couple fields and click "Generate Installer / Update Zip" and it will spit out 2 files 

![image](https://github.com/Echostorm44/ShipThatCode/assets/107306362/c0996fe1-4706-4b83-8046-adf49a011dcf)

for you to upload to a Github Release

![image](https://github.com/Echostorm44/ShipThatCode/assets/107306362/e157eb54-7573-434b-b6ed-af39e192c44b)


The install process looks like this:

![explorer_rw6dIxvlyC](https://github.com/Echostorm44/ShipThatCode/assets/107306362/9cbc45e1-5eb7-4a85-9c9d-1168a91c1574)

It uses the icon you pointed out during setup to create a nice branded shortcut on the desktop that points to the launcher file that checks for updates before launching your exe
The launcher will delete all the files previously installed except any in the config.json in the semicolon delmited string property DoNotDeleteTheseFiles

![image](https://github.com/Echostorm44/ShipThatCode/assets/107306362/3e9b6c48-2f99-4241-8e14-308b1c0509ce)

The EULA.txt file it uses is in the SetupFiles folder, you can just paste your custom EULA in there or use the default.

I hope that this helps other devs with their projects and allows them to focus on writing cool apps instad of spending ages making installers and updaters. 





