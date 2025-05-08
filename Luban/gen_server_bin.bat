set WORKSPACE=..

set LUBAN_DLL=.\Tools\Luban\Luban.dll
set CONF_ROOT=.\Config

dotnet %LUBAN_DLL% ^
    --conf %CONF_ROOT%\luban.conf ^
    -t server ^
    -c cs-bin ^
    -d bin ^
    -x outputCodeDir=%WORKSPACE%\Server\Geek.Server.Generate\Configs\Data ^
    -x outputDataDir=%WORKSPACE%\Server\Bytes ^
    -x pathValidator.rootDir=%WORKSPACE%\Server ^
    -x l10n.provider=default ^
    -x l10n.textFile.path=%CONF_ROOT%\Datas\l10n\Localization.xlsx ^
    -x l10n.textFile.keyFieldName=key

pause