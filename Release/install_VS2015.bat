@echo off
echo Copy zip file to Visual Studio 2015 template folder
set "net35Source=%~dp0ConsoleToolProject_Net35.zip"
set "net45Source=%~dp0ConsoleToolProject_Net45.zip"
set "net46Source=%~dp0ConsoleToolProject_Net46.zip"
set "o365Source=%~dp0ConsoleToolProject_Office365.zip"

set "destPath=%USERPROFILE%\Documents\Visual Studio 2015\Templates\ProjectTemplates\Visual C#\Custom Project"
if exist "%destPath%" rd /s /q "%destPath%"
md "%destPath%"

Copy "%net35Source%" "%destPath%" /l
Copy "%net45Source%" "%destPath%" /l
Copy "%net46Source%" "%destPath%" /l
Copy "%o365Source%" "%destPath%" /l

echo Install finished, please restart your Visual Studio.
pause