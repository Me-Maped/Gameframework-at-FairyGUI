package utils

type ProtocolInfo struct {
	ProtocolID   int
	ProtocolName string
}

type CharpMsgTempData struct {
	CSharpNs string
	Info     ProtocolInfo
}

type EnumStruct struct {
	EnumId   string
	EnumName string
}

type EnumInfo struct {
	EnumType string
	Enums    []EnumStruct
}

type ParseInfo struct {
	CSharpPbNs   string
	CSharpNs     string
	GenerateTime string
	FileName     string
	Infos        []ProtocolInfo
	PbNames      []string
}

type ParseEnumInfo struct {
	CSharpEnumNs string
	GenerateTime string
	FileName     string
	Infos        []EnumInfo
}
