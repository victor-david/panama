@echo off
rem Change paths as needed and use this file to install.
setlocal
set ROOT=%~dp0
set BIN=%ROOT%Source\Panama\bin\x86\release
set DEST=D:\Writing\Panama
copy "%BIN%\Panama.exe" %DEST%
copy "%BIN%\*.dll" %DEST%
REM This doesn't get copied to Panama\bin
copy "%ROOT%..\Restless.Tools.Library\Reference Assemblies\DocumentFormat.OpenXml.dll" %DEST%
REM Copy CHM file
if exist "%ROOT%\Help\Panama.Reference.chm" (
  copy "%ROOT%\Help\Panama.Reference.chm" %DEST%
)
dir %DEST%
endlocal
pause
