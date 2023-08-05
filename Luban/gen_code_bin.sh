#!/bin/zsh
WORKSPACE=..
GEN_CLIENT=${WORKSPACE}/Luban/Luban.ClientServer/Luban.ClientServer.dll
CONF_ROOT=${WORKSPACE}/Luban/Config

dotnet ${GEN_CLIENT} -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${WORKSPACE}/Client/Assets/AssetRaw/Configs \
 --output_code_dir ${WORKSPACE}/Client/Assets/GameMain/Scripts/HotFix/GameProto/GameConfig \
 --gen_types code_cs_unity_bin,data_bin \
 -s all 