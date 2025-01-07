#!/bin/zsh
WORKSPACE=..
LUBAN_DLL=${WORKSPACE}/Tools/Luban/Luban.dll
CONF_ROOT=${WORKSPACE}/Luban/Config

dotnet $LUBAN_DLL \
    --conf ${CONF_ROOT}/luban.conf \
    -t all \
    -c cs-bin \
    -d bin \
    -x outputDataDir=${WORKSPACE}/Client/Assets/AssetRaw/Configs \
    -x outputCodeDir=${WORKSPACE}/Client/Assets/GameMain/Scripts/HotFix/GameProto/GameConfig \
    -x l10n.provider=default \
    -x l10n.textFile.path=${CONF_ROOT}/Datas/l10n/Localization.xlsx \
    -x l10n.textFile.keyFieldName=key