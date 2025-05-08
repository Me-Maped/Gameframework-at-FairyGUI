set WORKSPACE=..

set LUBAN_DLL=.\Tools\Luban\Luban.dll
set CONF_ROOT=.\Config

dotnet %LUBAN_DLL% ^
    --conf %CONF_ROOT%\luban.conf ^
    -t client ^
    -c cs-bin ^
    -d bin ^
    -x outputCodeDir=%WORKSPACE%\Client\Assets\GameMain\Scripts\HotFix\GameProto\GameConfig ^
    -x outputDataDir=%WORKSPACE%\Client\Assets\AssetRaw\Configs ^
    -x pathValidator.rootDir=%WORKSPACE%\Client ^
    -x l10n.provider=default ^
    -x l10n.textFile.path=%CONF_ROOT%\Datas\l10n\Localization.xlsx ^
    -x l10n.textFile.keyFieldName=key

pause