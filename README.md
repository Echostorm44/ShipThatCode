# ShipThatCode
 A simple utility to create an installer and updater for .net projects without code changes using Github to host updates.

![image](https://github.com/Echostorm44/ShipThatCode/assets/107306362/67cfa31c-a18e-4547-a881-12265585bd45)

ShipThatCode is my version of ClickOnce.  It is crazy to me that all the options for creating installers for and keeping windows programs up to date are such a convoluted time consuming mess that are sometimes more work that the product itself.

Here is the use case. I have an application that runs on Windows that I want to create a simple but attractive installer for and for it to check for updates each time it launches without having to add any code to my application.

ShipThatCode builds 2 files.  An installer.exe and a zip that the launcher uses to update users on older versions.

installer.exe is a sort of self extrtracting archive that will 
1. Extract your program to a temp folder
2. Run the setup.exe which is included
3. Show the user your EULA
4. Ask them where they want to install
5. Copy the files
6. Create a proper shortcut on the desktop that points to an updater program that will look in your projects releases for the most recent version of the zip file and download and overwrite the app files as needed.

This is as simple for you the developer and the end user as I can make it.
All you have to do is fill out a couple fields and click "Generate Installer / Update Zip" and it will spit out 2 files 

![image](https://github.com/Echostorm44/ShipThatCode/assets/107306362/c0996fe1-4706-4b83-8046-adf49a011dcf)

for you to upload to a Github Release

![image](https://github.com/Echostorm44/ShipThatCode/assets/107306362/e157eb54-7573-434b-b6ed-af39e192c44b)


The install process looks like this:

![devenv_PG1DVL7Cwm](https://github.com/Echostorm44/ShipThatCode/assets/107306362/5fa66ddd-8071-4081-a2a6-59fa70262c78)



