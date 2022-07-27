@echo off
SetLocal EnableDelayedExpansion

set DOTNETCORENAME=dotnet-sdk-6.0.302-win-x64
set PACKPATH=%TEMP%\%DOTNETCORENAME%.exe 
set LOGPATH=%TEMP%\%DOTNETCORENAME%.log

echo.
echo =================================================================
echo =================================================================
echo Downloading .NET package ...
echo =================================================================
echo =================================================================
C:\Windows\System32\curl.exe -o %PACKPATH% https://download.visualstudio.microsoft.com/download/pr/c246f2b8-da39-4b12-b87d-bf89b6b51298/2d43d4ded4b6a0c4d1a0b52f0b9a3b30/%DOTNETCORENAME%.exe
if not exist %PACKPATH% (
    echo.
    echo ERROR during download of .NET package
    echo.
    exit /B 1
)
echo.
echo =================================================================
echo =================================================================
echo Installing .NET package ...
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
    echo ERROR during installation of .NET package
    echo.
    exit /B 1
)
exit /b 0
