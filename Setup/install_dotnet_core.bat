@echo off
SetLocal EnableDelayedExpansion

set DOTNETCORENAME=dotnet-sdk-6.0.202-win-x64
set PACKPATH=%TEMP%\%DOTNETCORENAME%.exe 
set LOGPATH=%TEMP%\%DOTNETCORENAME%.log

echo.
echo =================================================================
echo =================================================================
echo Downloading .NET package ...
echo =================================================================
echo =================================================================
C:\Windows\System32\curl.exe -o %PACKPATH% https://download.visualstudio.microsoft.com/download/pr/e4f4bbac-5660-45a9-8316-0ffc10765179/8ade57de09ce7f12d6411ed664f74eca/%DOTNETCORENAME%.exe
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
