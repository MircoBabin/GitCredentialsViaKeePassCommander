@echo off
setlocal
cls

cd /D "%~dp0"

set sz_exe=C:\Program Files\7-Zip\7z.exe
if exist "%sz_exe%" goto build

set sz_exe=C:\Program Files (x86)\7-Zip\7z.exe
if exist "%sz_exe%" goto build

echo !!! 7-Zip 18.06 - 7z.exe not found
pause
goto:eof

:build 
echo 7-Zip 18.06: %sz_exe%

FOR /F "tokens=* USEBACKQ" %%g IN (`"%~dp0..\bin\Release\git-credential-keepasscommand.exe" --version`) do (SET GitCredentialsViaKeePassCommanderReleaseVersion=%%g)
FOR /F "tokens=* USEBACKQ" %%g IN (`"%~dp0..\bin\Debug\git-credential-keepasscommand.exe" --version`) do (SET GitCredentialsViaKeePassCommanderDebugVersion=%%g)

if "%GitCredentialsViaKeePassCommanderReleaseVersion%" == "%GitCredentialsViaKeePassCommanderDebugVersion%" goto build_zip
echo.
echo Release version: %GitCredentialsViaKeePassCommanderReleaseVersion%
echo Debug version..: %GitCredentialsViaKeePassCommanderDebugVersion%
echo.
echo !!! Versions do not match.
pause
goto :eof


:build_zip
echo.
echo Release version: %GitCredentialsViaKeePassCommanderReleaseVersion%
echo.
echo.


del /q "Release\*" >nul 2>&1

set files="%~dp0..\bin\Release\git-credential-keepasscommand.exe"
set files=%files% "%~dp0..\bin\Release\git-credential-keepasscommand.exe.config"

"%sz_exe%" a -tzip -mx7 "Release\GitCredentialsViaKeePassCommander-%GitCredentialsViaKeePassCommanderReleaseVersion%.zip" %files%
"%sz_exe%" a -tzip -mx7 "Release\GitCredentialsViaKeePassCommander-%GitCredentialsViaKeePassCommanderReleaseVersion%-debugpack.zip" "%~dp0..\bin"

echo.
echo.
echo Created "Release\GitCredentialsViaKeePassCommander-%GitCredentialsViaKeePassCommanderReleaseVersion%.zip"
echo Created "Release\GitCredentialsViaKeePassCommander-%GitCredentialsViaKeePassCommanderReleaseVersion%-debugpack.zip" 

rem https://github.com/MircoBabin/GitCredentialsViaKeePassCommander/releases/latest/download/release.download.zip.url-location
rem Don't output trailing newline (CRLF)
<NUL >"Release\release.download.zip.url-location" set /p="https://github.com/MircoBabin/GitCredentialsViaKeePassCommander/releases/download/%GitCredentialsViaKeePassCommanderReleaseVersion%/GitCredentialsViaKeePassCommander-%GitCredentialsViaKeePassCommanderReleaseVersion%.zip"

echo.
echo Created "Release\release.download.zip.url-location" 
echo.

pause
