@echo off
SetLocal EnableDelayedExpansion

rem set DOTNETCORENAME=dotnet-sdk-6.0.202-win-x64
set DOTNETCORENAME=dotnet-sdk-8.0.405-win-x64
set PACKPATH=%TEMP%\%DOTNETCORENAME%.exe 
set LOGPATH=%TEMP%\%DOTNETCORENAME%.log

echo.
echo =================================================================
echo =================================================================
echo Downloading .NET package ...
echo =================================================================
echo =================================================================
rem C:\Windows\System32\curl.exe -o %PACKPATH% https://download.visualstudio.microsoft.com/download/pr/e4f4bbac-5660-45a9-8316-0ffc10765179/8ade57de09ce7f12d6411ed664f74eca/%DOTNETCORENAME%.exe
C:\Windows\System32\curl.exe -o %PACKPATH% https://download.visualstudio.microsoft.com/download/pr/4b3b488c-9e69-4d60-bba2-79412b68d15d/b55f49a270c3413a6ea4b208f820515d/%DOTNETCORENAME%.exe
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
