package args

import (
	"os"
	"path"
	"path/filepath"
	"zinx/lib/utils/commandline/uflag"
)

type args struct {
	ExeAbsDir  string
	ExeName    string
	ConfigFile string
}

var (
	Args   = args{}
	isInit = false
)

func init() {
	exe := os.Args[0]

	pwd, err := os.Getwd()
	if err != nil {
		panic(any(err))
	}

	Args.ExeAbsDir = pwd
	Args.ExeName = path.Base(exe)
}

func InitConfigFlag(defaultValue string, tips string) {
	if isInit {
		return
	}
	isInit = true
	uflag.StringVar(&Args.ConfigFile, "c", defaultValue, tips)
}

func FlagHandle() {
	if !filepath.IsAbs(Args.ConfigFile) {
		Args.ConfigFile = path.Join(Args.ExeAbsDir, Args.ConfigFile)
	}
}
