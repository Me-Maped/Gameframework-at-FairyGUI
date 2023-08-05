#!/bin/bash
#protoc版本3.19.4

# 自定义csharp代码生成路径
CSHARP_OUTPUT="../../Client/Assets/GameMain/Scripts/HotFix/GameProto/GamePb/"
# 自定义csharp自定义扩展代码生成路径
CSHARP_MSG_OUTPUT=$CSHARP_OUTPUT"/Msg/"
# 自定义go代码生成路径
GO_OUTPUT="../../Server/demo_server/pb/"

# pb文件路径
PROTO_DIR="./Proto"
# 模板文件
TMP_DIR="./Template/"
CSHARP_TMP=$TMP_DIR"CSharp.txt"
CSHARP_ENUM_TMP=$TMP_DIR"CSharpEnum.txt"
CSHARP_MSG_TMP=$TMP_DIR"CSharpMsg.txt"
GO_TMP=$TMP_DIR"Go.txt"


while getopts ":a:b:c:" options;
do
    case "${options}" in
        a)
            CSHARP_OUTPUT=${OPTARG}
            ;;
        b)
            CSHARP_MSG_OUTPUT=${OPTARG}
            ;;
        c)
            GO_OUTPUT=${OPTARG}
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

            echo "生成Go文件"
            protoc --go_out $GO_OUTPUT $dir_or_file

            # echo "生成C++文件"
            #需要进入到Proto目录下执行protoc，否则生成出的Pb会带有文件夹，导致头文件引用有问题
            # protoc --cpp_out "./../"$pb_dir"/Cpp" $element
        fi  
    done
}

# 清空生成目录下所有文件
rm -rf $CSHARP_OUTPUT
rm -rf $GO_OUTPUT
mkdir $CSHARP_OUTPUT
mkdir $GO_OUTPUT
getproto $PROTO_DIR

#生成协议号和枚举
go run main.go -protoDir $PROTO_DIR -csharpOutput $CSHARP_OUTPUT -csharpMsgOutput $CSHARP_MSG_OUTPUT -csharpTmp $CSHARP_TMP -csharpEnumTmp $CSHARP_ENUM_TMP -csharpMsgTmp $CSHARP_MSG_TMP -goTmp $GO_TMP -goOutput $GO_OUTPUT