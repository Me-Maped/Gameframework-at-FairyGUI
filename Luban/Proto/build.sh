#!/bin/bash
#protoc版本3.19.4

proto_dir="./Proto"
csharp_dir="./CSharp"
cpp_dir="./Cpp"
go_dir="./Go"
pb_dir="Pb"
tmp_dir="./Template/"

PROTO_OUTPUT=$proto_dir

CSHARP_PB_NS="Pb"
CSHARP_NS="Generate.Pb"
CSHARP_ENUM_NS="Generate.Enum"
CSHARP_OUTPUT=$csharp_dir"/"
CSHARP_MSG_OUTPUT=$csharp_dir"/Msg/"

CPP_OUTPUT=$cpp_dir"/"
TYPE_FILE_NAME="ProtoType"
ENUM_FILE_NAME="ProtoEnum"
HELPER_FILE_NAME="ProtoHelper"

GO_OUTPUT=$go_dir"/"

CPP_ENUM_TMP=$tmp_dir"CppEnum.txt"
CPP_HELPER_H_TMP=$tmp_dir"CppHelperH.txt"
CPP_TMP=$tmp_dir"Cpp.txt"
CPP_HELPER_TMP=$tmp_dir"CppHelper.txt"
CSHARP_TMP=$tmp_dir"CSharp.txt"
CSHARP_ENUM_TMP=$tmp_dir"CSharpEnum.txt"
CSHARP_MSG_TMP=$tmp_dir"CSharpMsg.txt"
GO_TMP=$tmp_dir"Go.txt"


while getopts ":a:b:c:d:e:f:g:h:i:j:k:l:m:n:o:" options;
do
    case "${options}" in
        a)
            CSHARP_PB_NS=${OPTARG}
            ;;
        b)
            CSHARP_NS=${OPTARG}
            ;;
        c)
            CSHARP_ENUM_NS=${OPTARG}
            ;;
        d)
            CSHARP_MSG_OUTPUT=${OPTARG}
            ;;
        e)
            TYPE_FILE_NAME=${OPTARG}
            ;;
        f)
            ENUM_FILE_NAME=${OPTARG}
            ;;
        g)
            HELPER_FILE_NAME=${OPTARG}
            ;;
        h)
            CPP_ENUM_TMP=${OPTARG}
            ;;
        i)
            CPP_HELPER_H_TMP=${OPTARG}
            ;;
        j)
            CPP_TMP=${OPTARG}
            ;;
        k)
            CPP_HELPER_TMP=${OPTARG}
            ;;
        l)
            CSHARP_TMP=${OPTARG}
            ;;
        m)
            CSHARP_ENUM_TMP=${OPTARG}
            ;;
        n)
            CSHARP_MSG_TMP=${OPTARG}
            ;;
        o)
            GO_TMP=${OPTARG}
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
            cd $1

            echo "生成C#文件"
            protoc --csharp_out "./../"$pb_dir"/CSharp" $element

            echo "生成Go文件"
            protoc --go_out "./../"$pb_dir"/Go" $element

            echo "生成C++文件"
            #需要进入到Proto目录下执行protoc，否则生成出的Pb会带有文件夹，导致头文件引用有问题
            protoc --cpp_out "./../"$pb_dir"/Cpp" $element
            cd ..
        fi  
    done
}

rm -rf $csharp_dir
rm -rf $cpp_dir
rm -rf $go_dir
rm -rf $pb_dir
mkdir $pb_dir
mkdir $pb_dir"/Cpp"
mkdir $pb_dir"/CSharp"
mkdir $pb_dir"/Go"
getproto $proto_dir

#生成协议号和枚举
go run main.go -csharpPbNs $CSHARP_PB_NS -csharpNs $CSHARP_NS -csharpEnumNs $CSHARP_ENUM_NS -protoDir $PROTO_OUTPUT -csharpOutput $CSHARP_OUTPUT -cppOutput $CPP_OUTPUT -csharpMsgOutput $CSHARP_MSG_OUTPUT -typeFileName $TYPE_FILE_NAME -enumFileName $ENUM_FILE_NAME -helperFileName $HELPER_FILE_NAME -cppEnumTmp $CPP_ENUM_TMP -cppHelperHTmp $CPP_HELPER_H_TMP -cppTmp $CPP_TMP -cppHelperTmp $CPP_HELPER_TMP -csharpTmp $CSHARP_TMP -csharpEnumTmp $CSHARP_ENUM_TMP -csharpMsgTmp $CSHARP_MSG_TMP -goTmp $GO_TMP