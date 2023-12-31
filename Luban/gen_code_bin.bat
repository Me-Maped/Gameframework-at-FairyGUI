set WORKSPACE=..

set GEN_CLIENT=%WORKSPACE%\Luban\Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\Luban\Config

%GEN_CLIENT% -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %WORKSPACE%\Client\Assets\GameMain\Scripts\HotFix\GameProto\GameConfig ^
 --output_data_dir %WORKSPACE%\Client\Assets\AssetRaw\Configs ^
 --gen_types code_cs_unity_bin,data_bin ^
 -s all 

echo ======== 生成配置文件结束 ========

pause