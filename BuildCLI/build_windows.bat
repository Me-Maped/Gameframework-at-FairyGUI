cd /d %~dp0

call path_define.bat

%UNITYEDITOR_PATH%/Unity.exe %WORKSPACE% -logFile %BUILD_LOGFILE% -executeMethod UnityGameFramework.Editor.BuildTool.AutomationBuildWindows -quit -batchmode -CustomArgs:Language=en_US; %WORKSPACE%

@REM for /f "delims=[" %%i in (%BUILD_LOGFILE%) do echo %%i

pause