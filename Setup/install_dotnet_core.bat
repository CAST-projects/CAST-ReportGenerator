@echo off
SetLocal EnableDelayedExpansion

set DOTNETCORENAME=dotnet-sdk-3.1.102-win-x64
set PACKPATH=%TEMP%\%DOTNETCORENAME%.exe 
set LOGPATH=%TEMP%\%DOTNETCORENAME%.log

echo.
echo =================================================================
echo =================================================================
echo Downloading .NET core package ...
echo =================================================================
echo =================================================================
C:\Windows\System32\curl.exe -o %PACKPATH% https://download.visualstudio.microsoft.com/download/pr/5aad9c2c-7bb6-45b1-97e7-98f12cb5b63b/6f6d7944c81b043bdb9a7241529a5504/%DOTNETCORENAME%.exe
if not exist %PACKPATH% (
    echo.
    echo ERROR during download of .NET core package
    echo.
    exit /B 1
)
echo.
echo =================================================================
echo =================================================================
echo Installing .NET core package ...
echo =================================================================
echo =================================================================
echo.
echo Installation logs will be found into file:
echo %LOGPATH%
echo.
echo.
set /p ok=Enter return to continue ....

call %PACKPATH% /install /log %LOGPATH%
if not exist %PACKPATH% (
    echo.
    echo ERROR during installation of .NET core package
    echo.
    exit /B 1
)
exit /b 0
