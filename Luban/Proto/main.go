package main

import (
	"flag"
	"fmt"
	"html/template"
	"os"
	"src/utils"
	"strings"
	"time"
)

var (
	ProtoDir = flag.String("protoDir", "./Proto", "proto存放目录")

	CSharpPbNs      = flag.String("csharpPbNs", "Pb", "协议号命名空间")
	CSharpNs        = flag.String("csharpNs", "Generate.Pb", "msg命名空间")
	CSharpEnumNs    = flag.String("csharpEnumNs", "Generate.Enum", "enum命名空间")
	CSharpOutput    = flag.String("csharpOutput", "./CSharp/", "csharp协议号输出目录")
	CSharpMsgOutput = flag.String("csharpMsgOutput", "./CSharp/Msg/", "csharp自定义协议类输出目录")

	CppOutput      = flag.String("cppOutput", "./Cpp/", "cpp输出目录")
	TypeFileName   = flag.String("typeFileName", "ProtoType", "ProtoType文件名")
	EnumFileName   = flag.String("enumFileName", "ProtoEnum", "ProtoEnum文件名")
	HelperFileName = flag.String("helperFileName", "ProtoHelper", "ProtoHelper文件名")

	GoOutput = flag.String("goOutput", "./Go/", "go文件输出目录")
	GoTypeFileName  = flag.String("goTypeFileName", "consts", "goProtoType文件名")

	CppEnumTmp    = flag.String("cppEnumTmp", "./Template/CppEnum.txt", "CppEnum模版")
	CppHelperHTmp = flag.String("cppHelperHTmp", "./Template/CppHelperH.txt", "CppHelperH模版")
	CppTmp        = flag.String("cppTmp", "./Template/Cpp.txt", "Cpp模版")
	CppHelperTmp  = flag.String("cppHelperTmp", "./Template/CppHelper.txt", "CppHelper模版")
	CSharpTmp     = flag.String("csharpTmp", "./Template/CSharp.txt", "CSharp模版")
	CSharpEnumTmp = flag.String("csharpEnumTmp", "./Template/CSharpEnum.txt", "CSharpEnum模版")
	CSharpMsgTmp  = flag.String("csharpMsgTmp", "./Template/CSharpMsg.txt", "CSharpMsg模版")
	GoTmp         = flag.String("goTmp", "./Template/Go.txt", "Go模版")
)

func main() {
	flag.Parse()

	allProtoFiles := utils.FilterFile(*ProtoDir)
	fmt.Println("搜索到的proto文件:")
	fmt.Println(allProtoFiles)

	parseInfo := utils.ParseInfo{}
	parseInfo.CSharpPbNs = *CSharpPbNs
	parseInfo.CSharpNs = *CSharpNs
	parseInfo.GenerateTime = time.Now().String()
	parseInfo.Infos = make([]utils.ProtocolInfo, 0)

	parseEnumInfo := utils.ParseEnumInfo{}
	parseEnumInfo.CSharpEnumNs = *CSharpEnumNs
	parseEnumInfo.GenerateTime = time.Now().String()
	parseEnumInfo.Infos = make([]utils.EnumInfo, 0)

	for i := 0; i < len(allProtoFiles); i++ {
		filePath := allProtoFiles[i]
		eInfos := utils.ParseEnum(filePath)
		pInfos := utils.ParseProto(filePath)
		if len(eInfos) > 0 {
			parseEnumInfo.Infos = append(parseEnumInfo.Infos, eInfos...)
			fmt.Println("解析Enum文件成功:" + filePath)
		}
		if len(pInfos) > 0 {
			parseInfo.Infos = append(parseInfo.Infos, pInfos...)
			fisrtIndex := strings.Index(filePath, "/")
			secondIndex := strings.Index(filePath, ".")
			parseInfo.PbNames = append(parseInfo.PbNames, filePath[fisrtIndex+1:secondIndex])
			fmt.Println("解析Proto文件成功:" + filePath)
		}
	}

	saveProtoFile(&parseInfo, *CSharpOutput, "cs", *TypeFileName, utils.ParseString(*CSharpTmp))
	saveProtoFile(&parseInfo, *CppOutput, "h", *TypeFileName, utils.ParseString(*CppTmp))
	saveProtoFile(&parseInfo, *CppOutput, "h", *HelperFileName, utils.ParseString(*CppHelperHTmp))
	saveProtoFile(&parseInfo, *CppOutput, "cpp", *HelperFileName, utils.ParseString(*CppHelperTmp))
	saveProtoFile(&parseInfo, *GoOutput, "go", *GoTypeFileName, utils.ParseString(*GoTmp))
	saveEnumFile(&parseEnumInfo, *CSharpOutput, "cs", *EnumFileName, utils.ParseString(*CSharpEnumTmp))
	saveEnumFile(&parseEnumInfo, *CppOutput, "h", *EnumFileName, utils.ParseString(*CppEnumTmp))
	// saveCsharMsgFile(&parseInfo, *CSharpMsgOutput,utils.ParseString(*CSharpMsgTmp))
}

func saveProtoFile(info *utils.ParseInfo, outputPath string, fileType string, fileName string, fileTemplate string) {
	_, err := utils.CheckPath(outputPath)
	utils.CheckErr(err)

	tmpl, err := template.New(fileType).Parse(fileTemplate)
	utils.CheckErr(err)

	filePath := outputPath + fileName + "." + fileType
	os.Remove(filePath)
	file, err := os.OpenFile(filePath, os.O_CREATE|os.O_WRONLY, 0755)
	utils.CheckErr(err)

	info.FileName = fileName
	err = tmpl.Execute(file, info)
	utils.CheckErr(err)
}

func saveEnumFile(info *utils.ParseEnumInfo, outputPath string, fileType string, fileName string, fileTemplate string) {
	_, err := utils.CheckPath(outputPath)
	utils.CheckErr(err)

	tmpl, err := template.New(fileType).Parse(fileTemplate)
	utils.CheckErr(err)

	filePath := outputPath + fileName + "." + fileType
	os.Remove(filePath)
	file, err := os.OpenFile(filePath, os.O_CREATE|os.O_WRONLY, 0755)
	utils.CheckErr(err)

	info.FileName = fileName
	err = tmpl.Execute(file, info)
	utils.CheckErr(err)
}

func saveCsharMsgFile(info *utils.ParseInfo, outputPath string, fileTemplate string) {
	for i := 0; i < len(info.Infos); i++ {
		_, err := utils.CheckPath(outputPath)
		utils.CheckErr(err)

		infoStruct := info.Infos[i]
		tmpl, err := template.New("cs").Parse(fileTemplate)
		utils.CheckErr(err)

		msgPath := outputPath + infoStruct.ProtocolName + "Msg.cs"
		os.Remove(msgPath)
		file, err := os.OpenFile(msgPath, os.O_CREATE|os.O_WRONLY, 0755)
		utils.CheckErr(err)

		data := utils.CharpMsgTempData{}
		data.Info = infoStruct
		data.CSharpNs = *CSharpNs
		err = tmpl.Execute(file, data)
		utils.CheckErr(err)
	}
}
