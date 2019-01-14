@echo off
echo Copy zip file to Visual Studio 2012 template folder
set "commonSourceFile=%~dp0Console Tool Project.zip"
set "net35SourceFile=%~dp0Console Tool Project for Net 3.5.zip"

set "destPath=%USERPROFILE%\Documents\Visual Studio 2012\Templates\ProjectTemplates\Visual C#"

Copy "%commonSourceFile%" "%destPath%" /l
Copy "%net35SourceFile%" "%destPath%" /l

echo Install finished, please restart your Visual Studio.
pause