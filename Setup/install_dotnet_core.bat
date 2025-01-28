@echo off
SetLocal EnableDelayedExpansion

rem set DOTNETCORENAME=dotnet-sdk-6.0.202-win-x64
set DOTNETCORENAME=dotnet-sdk-8.0.308-win-x64
set PACKPATH=%TEMP%\%DOTNETCORENAME%.exe 
set LOGPATH=%TEMP%\%DOTNETCORENAME%.log

echo.
echo =================================================================
echo =================================================================
echo Downloading .NET package ...
echo =================================================================
echo =================================================================
rem C:\Windows\System32\curl.exe -o %PACKPATH% https://download.visualstudio.microsoft.com/download/pr/e4f4bbac-5660-45a9-8316-0ffc10765179/8ade57de09ce7f12d6411ed664f74eca/%DOTNETCORENAME%.exe
C:\Windows\System32\curl.exe -o %PACKPATH% https://download.visualstudio.microsoft.com/download/pr/9d35afb6-e1ae-439b-8673-ae0496623649/6d7b61c8c110d14cc9a8a95c8d9e63ea/%DOTNETCORENAME%.exe
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
