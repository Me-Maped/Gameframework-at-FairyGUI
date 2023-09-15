@echo off

:: ===get absolute path===
for %%i in ("%~dp0.") do set SCRIPT_DIR=%%~fi

:: protoc
set PROTOC="%SCRIPT_DIR%\protoc.exe"
:: go
set TMP_GEN="%SCRIPT_DIR%\main.exe"

:: 自定义客户端C#代码生成路径
set CLIENT_RELATIVE_PATH=..\..\Client\Assets\GameMain\Scripts\HotFix\GameProto\GamePb
set CLIENT_CSHARP_OUTPUT="%SCRIPT_DIR%\%CLIENT_RELATIVE_PATH%"
set CLIENT_NS="Pb"
:: 自定义服务端C#代码生成路径
set SERVER_RELATIVE_PATH=..\..\Server\Geek.Server.Generate\Proto
set SERVER_CSHARP_OUTPUT="%SCRIPT_DIR%\%SERVER_RELATIVE_PATH%"
set SERVER_NS="Geek.Server.Proto"

:: pb文件路径
set PROTO_RELATIVE_DIR=.\Proto
set PROTO_DIR="%SCRIPT_DIR%\%PROTO_RELATIVE_DIR%"
:: 模板文件
set TMP_DIR=".\Template"
set CSHARP_TMP="%TMP_DIR%\CSharp.txt"

:: 参数处理
:parse_args
if "%~1"=="" goto execute

if "%~1"=="-a" (
    set CLIENT_CSHARP_OUTPUT=%~2
    shift
    shift
    goto parse_args
)

if "%~1"=="-b" (
    set SERVER_CSHARP_OUTPUT=%~2
    shift
    shift
    goto parse_args
)

if "%~1"=="-h" (
    echo "Usage: -a [client csharp output path] -b [server csharp output path]"
)

:execute
:: 清空生成目录下所有文件
rd /s /q %CLIENT_CSHARP_OUTPUT%
rd /s /q %SERVER_CSHARP_OUTPUT%
mkdir %CLIENT_CSHARP_OUTPUT%
mkdir %SERVER_CSHARP_OUTPUT%

:: 生成客户端协议号和枚举
echo Generating Template...
%TMP_GEN% -protoDir %PROTO_RELATIVE_DIR% -csharpOutput %CLIENT_RELATIVE_PATH% -csharpTmp %CSHARP_TMP% -csharpPbNs %CLIENT_NS%

:: 函数：获取proto文件并生成代码
for %%i in (%PROTO_DIR%\*.proto) do (
    echo Generating C# code for %%i ...
    :: ===generating client code===
    %PROTOC% --proto_path=%PROTO_DIR% --csharp_out=%CLIENT_CSHARP_OUTPUT% "%%i"
)

:: 生成服务端协议号和枚举
echo Generating Template...
%TMP_GEN% -protoDir %PROTO_RELATIVE_DIR% -csharpOutput %SERVER_RELATIVE_PATH% -csharpTmp %CSHARP_TMP% -csharpPbNs %SERVER_NS%

:: 函数：获取proto文件并生成代码
for %%i in (%PROTO_DIR%\*.proto) do (
    echo Generating C# code for %%i ...
    :: ===generating server code===
    %PROTOC% --proto_path=%PROTO_DIR% --csharp_out=%SERVER_CSHARP_OUTPUT% "%%i"
)

echo All .proto files have been processed.
pause


