@echo off
rem Provides a simple means of installing the app.
rem You just need to get the executable and .dlls in one directory.
rem If you have Sandcastle Help File Builder installed, you can use Panama.shfbproj
rem to build a .chm file and it will be copied to your destination directory as well.
setlocal
set ROOT=%~dp0
set BIN=%ROOT%src\Panama\bin\x64\release
rem Change the destination path as needed and use this file to install.
set DEST=D:\Writing\Panama
if exist "%BIN%\Panama.exe" (
  xcopy "%BIN%\Panama.exe" %DEST% /Y
  xcopy "%BIN%\*.dll" %DEST% /E /Y
)

rem Copy CHM file if it's there
if exist "%ROOT%\Help\Panama.Reference.chm" (
  xcopy "%ROOT%\Help\Panama.Reference.chm" %DEST% /Y
)
dir %DEST%
endlocal
pause
