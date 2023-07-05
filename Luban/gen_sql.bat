set WORKSPACE=..

set GEN_CLIENT=%WORKSPACE%\Luban\Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\Luban\Config

%GEN_CLIENT% --template_search_path %CONF_ROOT%\Templates -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_data_dir ..\GenerateDatas\sql\mysql ^
 --gen_types data_template --template:data:file lua ^
 -s server

@echo off
cd ..\GenerateDatas\sql\mysql
for %%f in (*.lua) do ren "%%f" "%%~nf.sql"

echo ======== 生成配置文件结束 ========

pause