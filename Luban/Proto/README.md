## Protobuf生成工具

这是一个使用go语言对protobuf进行解析和代码模版生成的工具。自己在做游戏前后端交互过程中不想手动编辑协议模版代码和协议号，以及想通过配置jenkins一键生成pb而偷懒开发的。因为只花了一个星期学go和shell，所以工具代码质量不咋滴，但功能实现还是没什么问题，目前支持C#，C++以及Go语言模版生成，需要进行扩展的可以在Template文件夹中添加文本文件，同时在main.go中cv对应的代码。

### 环境配置

安装 go1.19版本、protobuf、protoc-gen-go（proto生成工具没测过版本问题，应该问题不大）

### 运行

命令行输入 sh build.sh 即可使用默认参数进行构建，也可以在后面追加需要修改的参数，具体见build.sh代码

### 工具组织结构

- Cpp：根据模版生成C++代码
- CSharp：根据模版生成C#代码
- Go：根据模版生成Go代码
- Pb：Protobuf生成文件夹
- Template：代码模版
- utils：工具代码
  - define.go：结构定义
  - utils.go：文件查找与转换
- build.sh：构建脚本
- main.go：工具代码入口

### 存在的问题
- 生成的代码模版与自定义的结构相关，扩展起来不是很方便，还需要修改代码
- build.sh中的参数传递感觉有点多余

### TODO
- build.sh使用长参数传入
