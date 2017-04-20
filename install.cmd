@echo off
rem Provides a simple means of installing the app.
rem You just need to get the executable and .dlls in one directory.
rem If you have Sandcastle Help File Builder installed, you can use Panama.shfbproj
rem to build a .chm file and it will be copied to your destination directory as well.
setlocal
set ROOT=%~dp0
set BIN=%ROOT%Source\Panama\bin\x86\release
rem Change the destination path as needed and use this file to install.
set DEST=D:\Writing\Panama
if exist "%BIN%\Panama.exe" (
  copy "%BIN%\Panama.exe" %DEST%
  copy "%BIN%\*.dll" %DEST%
  rem This doesn't get copied to Panama\bin
  copy "%ROOT%..\Restless.Tools.Library\Reference Assemblies\DocumentFormat.OpenXml.dll" %DEST%
)

rem Copy CHM file if it's there
if exist "%ROOT%\Help\Panama.Reference.chm" (
  copy "%ROOT%\Help\Panama.Reference.chm" %DEST%
)
dir %DEST%
endlocal
pause
