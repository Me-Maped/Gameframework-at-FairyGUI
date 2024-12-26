# GameFramework-at-FairyGUI

GameFramework + FairyGUI +Luban + HybridCLR + YooAsset + UniTask

实现初衷：在工作中接触到了FairyGUI与HybridCLR，真的十分好用，但git上很难找到使用FairyGUI设计的纯C#UI管理模块。在看到ALEXTANGXIAO开发的Gameframework-at-YooAsset后，决定在此基础上进行扩展（说白了就是缝合怪），以满足个人开发需求。经验有限，望指正。

```
// 项目结构设计
BuildCLI                // 构建工具
Client                  // 客户端代码
└── UIProject           // FairyUI工程
Luban                   // 配置工具集
└── Config              // 表配置与模板文件
Server                  // GeekServer服务端(可剥离)
```

```
// 程序集划分设计
Assets/GameMain/Scripts
├── Editor              // 编辑器程序集
├── HotFix              // 游戏热更程序集目录
|   ├── GameProto       // 游戏配置协议程序集
|   ├── BattleCore      // 游戏核心战斗程序集
|   └── GameLogic       // 游戏业务逻辑程序集
└── Runtime             // Runtime程序集
Asset/UnityGameFramework/Scripts
├── Editor              // 框架编辑器程序集
└── Runtime             // 框架运行时程序集
```

## WorkFlow
### 客户端结构介绍
~~TODO~~
### UI及功能制作流程
~~TODO~~
### 代码热更新
~~TODO~~
### 资源打包
~~TODO~~
### 其他功能
~~TODO~~


## <strong>特别鸣谢
#### <a href="https://github.com/tuyoogame/YooAsset"><strong>YooAsset</strong></a> - YooAsset是一套商业级经历百万DAU游戏验证的资源管理系统。
#### <a href="https://fairygui.com"><strong>FairyGUI</strong></a> - FairyGUI是一个重视设计，摈弃脚本与配置的专业游戏UI解决方案。
#### <a href=""><strong>HybridCLR</strong></a> - HybridCLR是一个特性完整、零成本、高性能、低内存的近乎完美的Unity全平台原生c#热更方案。
#### <a href=""><strong>GameFramework-at-YooAsset</strong></a> - GameFramework-at-YooAsset 是ALEXTANGXIAO大佬针对GameFramework资源系统二次开发的Unity游戏框架
