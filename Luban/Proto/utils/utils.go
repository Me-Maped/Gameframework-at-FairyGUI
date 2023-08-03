package utils

import (
	"bytes"
	"fmt"
	"os"
	"path/filepath"
	"strconv"
	"strings"
)

func FilterFile(dir string) []string {
	files, _ := filepath.Glob(dir + "/*.proto")
	return files
}

func CheckErr(err error) {
	if err != nil {
		panic(err)
	}
}

func CheckPath(path string) (bool, error) {
	_, err := os.Stat(path)
	if err == nil {
		return true, err
	}
	if os.IsNotExist(err) {
		err = os.Mkdir(path, os.ModePerm)
		if err == nil {
			return true, err
		}
		return false, err
	}
	return false, err
}

func ParseString(tempFile string) string {
	content, err := os.ReadFile(tempFile)
	CheckErr(err)
	buffer := bytes.NewBuffer(content)
	lineCount := 0
	var result strings.Builder

	for {
		line, err := buffer.ReadString('\n')
		if err != nil {
			//最后一行
			if len(line) > 0 {
				_, writeErr := result.WriteString(line)
				if writeErr != nil {
					panic(writeErr)
				}
			}
			break
		}
		lineCount++
		_, writeErr := result.WriteString(line)
		if writeErr != nil {
			panic(writeErr)
		}
	}
	return result.String()
}

func ParseProto(proto string) []ProtocolInfo {
	content, err := os.ReadFile(proto)
	CheckErr(err)

	buffer := bytes.NewBuffer(content)
	infos := make([]ProtocolInfo, 0)
	lineCount := 0
	protocolStartIndex := -1

	for {
		line, err := buffer.ReadString('\n')
		if err != nil {
			break
		}
		lineCount++

		if lineCount == 1 {
			if !strings.Contains(line, "@proto_id") {
				if !strings.Contains(line, "@proto") {
					fmt.Printf("文件 %s 第一行未定义@proto_id 跳过生成!\n", proto)
				}
				break
			}
			protocolStartIndex = parseHead(line)
			continue
		}

		idx := strings.Index(line, "message")
		if idx < 0 {
			continue
		}

		info := ProtocolInfo{}
		info.ProtocolName = line[idx+7:]
		idx = strings.Index(info.ProtocolName, "{")
		if idx >= 0 {
			info.ProtocolName = info.ProtocolName[:idx]
		}
		info.ProtocolName = strings.Replace(info.ProtocolName, "\r", "", -1)
		info.ProtocolName = strings.Replace(info.ProtocolName, "\n", "", -1)
		info.ProtocolName = strings.Replace(info.ProtocolName, " ", "", -1)

		if len(info.ProtocolName) == 0 {
			continue
		}

		protocolStartIndex++
		info.ProtocolID = protocolStartIndex

		infos = append(infos, info)
	}
	return infos
}

func ParseEnum(proto string) []EnumInfo {
	content, err := os.ReadFile(proto)
	CheckErr(err)

	buffer := bytes.NewBuffer(content)
	infos := make([]EnumInfo, 0)
	info := EnumInfo{}
	oneStart := false
	enums := make([]EnumStruct, 0)
	lineCount := 0

	for {
		line, err := buffer.ReadString('\n')
		if err != nil {
			//最后一行
			idx := strings.Index(line, "}")
			if idx >= 0 && oneStart {
				info.Enums = enums
				infos = append(infos, info)
				oneStart = false
			}
			break
		}
		lineCount++

		if lineCount == 1 {
			if !strings.Contains(line, "@proto_enum") {
				if !strings.Contains(line, "@proto") {
					fmt.Printf("文件 %s 第一行未定义@proto_enum 跳过生成!\n", proto)
				}
				break
			}
			continue
		}

		idx := strings.Index(line, "enum")
		if idx >= 0 {
			info = EnumInfo{}
			info.EnumType = line[idx+4:]
			idx = strings.Index(info.EnumType, "{")
			if idx >= 0 {
				info.EnumType = info.EnumType[:idx]
			}
			info.EnumType = strings.Replace(info.EnumType, "\r", "", -1)
			info.EnumType = strings.Replace(info.EnumType, "\n", "", -1)
			info.EnumType = strings.Replace(info.EnumType, " ", "", -1)
			oneStart = true
			enums = make([]EnumStruct, 0)
			continue
		}

		idx = strings.Index(line, "=")
		if idx >= 0 && oneStart {
			oneStruct := EnumStruct{}
			oneStruct.EnumName = line[:idx]
			oneStruct.EnumId = line[idx+1:]
			oneStruct.EnumName = strings.Replace(oneStruct.EnumName, "\r", "", -1)
			oneStruct.EnumName = strings.Replace(oneStruct.EnumName, "\n", "", -1)
			oneStruct.EnumName = strings.Replace(oneStruct.EnumName, " ", "", -1)
			oneStruct.EnumId = strings.Replace(oneStruct.EnumId, "\r", "", -1)
			oneStruct.EnumId = strings.Replace(oneStruct.EnumId, "\n", "", -1)
			oneStruct.EnumId = strings.Replace(oneStruct.EnumId, " ", "", -1)
			oneStruct.EnumId = strings.Replace(oneStruct.EnumId, ";", "", -1)
			enums = append(enums, oneStruct)
			continue
		}

		idx = strings.Index(line, "}")
		if idx >= 0 && oneStart {
			info.Enums = enums
			infos = append(infos, info)
			oneStart = false
		}
	}

	return infos
}

func parseHead(line string) int {
	bret := strings.Contains(line, "@proto_id")
	if !bret {
		return -1
	}

	idx := strings.LastIndex(line, "=")
	sub := line[idx+1:]
	sub = strings.Replace(sub, " ", "", -1)
	sub = strings.Replace(sub, "\r", "", -1)
	sub = strings.Replace(sub, "\n", "", -1)
	startIndex, err := strconv.Atoi(sub)
	CheckErr(err)
	return startIndex
}
