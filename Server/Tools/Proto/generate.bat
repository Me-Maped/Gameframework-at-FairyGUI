@echo off
setlocal enabledelayedexpansion

rem ===get absolute path===
for %%i in ("%~dp0.") do set script_dir=%%~fi

rem ===get .proto files===
set proto_relative_path=..\..\Geek.Server.Proto\Proto

rem ===set protoc.exe path===
set protoc_relative_path=.\protoc.exe

rem ===set csharp output path===
set output_relative_dir=..\..\Geek.Server.Generate\Proto

rem ===make full path===
set protoc_path="%script_dir%\%protoc_relative_path%"
set output_dir="%script_dir%\%output_relative_dir%"
set proto_path="%script_dir%\%proto_relative_path%"

rem ===find .proto file===
for %%i in (%proto_path%\*.proto) do (
    echo Generating C# code for %%i...
    rem ===generating code===
    %protoc_path% --proto_path=%proto_path% --csharp_out=%output_dir% "%%i"
)

echo All .proto files have been processed.
pause

