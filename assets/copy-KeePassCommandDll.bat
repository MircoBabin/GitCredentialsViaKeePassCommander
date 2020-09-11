@echo off
    cd /d "%~dp0"
    del /q KeePassCommandDll.dll >nul 2>&1
    copy /y ..\..\KeePassCommander\bin\release\KeePassCommandDll.dll