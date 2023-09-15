#!/bin/bash
#protoc版本3.19.4

# 自定义csharp代码生成路径
CSHARP_OUTPUT="../../Client/Assets/GameMain/Scripts/HotFix/GameProto/GamePb/"
# 自定义csharp扩展代码生成路径
CSHARP_MSG_OUTPUT=$CSHARP_OUTPUT"/Msg/"

# pb文件路径
PROTO_DIR="./Proto"
# 模板文件
TMP_DIR="./Template/"
CSHARP_TMP=$TMP_DIR"CSharp.txt"
CSHARP_ENUM_TMP=$TMP_DIR"CSharpEnum.txt"
CSHARP_MSG_TMP=$TMP_DIR"CSharpMsg.txt"


while getopts ":a:b:h:" options;
do
    case "${options}" in
        a)
            CSHARP_OUTPUT=${OPTARG}
            ;;
        b)
            CSHARP_MSG_OUTPUT=${OPTARG}
            ;;
        h)
            echo "Usage: -a [csharp output path] -b [csharp msg output path]"
            ;;
        :)
            echo "Error: -"${OPTARG}
            exit 1
            ;;
        *)
            exit 1
            ;;
    esac
done


function getproto(){
    for element in `ls $1`
    do  
        dir_or_file=$1"/"$element
        if [ -d $dir_or_file ]
        then 
            getproto $dir_or_file
        else
            echo "Proto路径 $dir_or_file"

            echo "生成C#文件"
            protoc --csharp_out $CSHARP_OUTPUT $dir_or_file
        fi  
    done
}

# 清空生成目录下所有文件
rm -rf $CSHARP_OUTPUT
mkdir $CSHARP_OUTPUT
getproto $PROTO_DIR

#生成协议号和枚举
go run main.go -protoDir $PROTO_DIR -csharpOutput $CSHARP_OUTPUT -csharpMsgOutput $CSHARP_MSG_OUTPUT -csharpTmp $CSHARP_TMP -csharpEnumTmp $CSHARP_ENUM_TMP -csharpMsgTmp $CSHARP_MSG_TMP