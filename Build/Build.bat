@echo off
echo Build project template
cd %~dp0
cd ..

set "buildTool=C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

"%buildTool%" ConsoleToolProjectTemplate\BuildProjectTemplate.sln /t:rebuild /p:Configuration=Release

xcopy /y /e ConsoleToolProjectTemplate\BuildProjectTemplate\ConsoleToolProject_Net35\bin\Release\ProjectTemplates\CSharp\1033\ConsoleToolProject_Net35.zip Release
xcopy /y /e ConsoleToolProjectTemplate\BuildProjectTemplate\ConsoleToolProject_Net45\bin\Release\ProjectTemplates\CSharp\1033\ConsoleToolProject_Net45.zip Release
xcopy /y /e ConsoleToolProjectTemplate\BuildProjectTemplate\ConsoleToolProject_Net46\bin\Release\ProjectTemplates\CSharp\1033\ConsoleToolProject_Net46.zip Release
xcopy /y /e ConsoleToolProjectTemplate\BuildProjectTemplate\ConsoleToolProject_Office365\bin\Release\ProjectTemplates\CSharp\1033\ConsoleToolProject_Office365.zip Release

pause