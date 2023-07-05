# GameFramework-at-FairyGUI

GameFramework + FairyGUI +Luban + HybridCLR + YooAsset + UniTask

实现初衷：在工作中接触到了FairyGUI与HybridCLR，真的十分好用，但git上很难找到使用FairyGUI设计的纯C#UI管理模块。在看到ALEXTANGXIAO开发的Gameframework-at-YooAsset后，决定在此基础上进行扩展（说白了就是缝合怪），以满足个人开发需求。经验有限，望指正。

除UI模块外相较于Gameframework-at-YooAsset的修改：
* 反射注册Handler，实现NetworkChannelHelper
* Network中抽象出Socket，并新增UnityWebSocket
* 支持WebRequest自定义请求头
* 流程优化，修复bug
* Luban模板修改，生成异步表加载结构


``` json
//程序集划分设计
Assets/GameMain/Scripts
├── Editor              // 编辑器程序集
├── HotFix              // 游戏热更程序集目录 [Folder]
|   ├── GameProto       // 游戏配置协议程序集 [Dll]  
|   ├── BattleCore      // 游戏核心战斗程序集 [Dll] 
|   └── GameLogic       // 游戏业务逻辑程序集 [Dll]
|           ├── GameApp.cs                  // 热更主入口
|           └── GameApp_RegisterSystem.cs   // 热更主入口注册系统
└── Runtime             // Runtime程序集
```

### TODO
- [ ] Protobuf自定义代码模板工具
- [ ] 优化UI框架，让写UI更简单
- [ ] 支持MacOS上工具链的使用
- [ ] 对Network模块扩展，使其支持 var ack = await Send<T>(req) 的形式
- [ ] FiaryGUI自定义包管理
- [ ] 修改并使用GeekServer作为配套后端
- [ ] 基于此框架完成一个小Demo
- [ ] 尝试在此基础上使用ECS架构


## <strong>特别鸣谢
#### <a href="https://github.com/tuyoogame/YooAsset"><strong>YooAsset</strong></a> - YooAsset是一套商业级经历百万DAU游戏验证的资源管理系统。

#### <a href="https://fairygui.com"><strong>FairyGUI</strong></a> - FairyGUI是一个重视设计，摈弃脚本与配置的专业游戏UI解决方案。

#### <a href=""><strong>HybridCLR</strong></a> - HybridCLR是一个特性完整、零成本、高性能、低内存的近乎完美的Unity全平台原生c#热更方案。

#### <a href=""><strong>GameFramework-at-YooAsset</strong></a> - GameFramework-at-YooAsset 是ALEXTANGXIAO大佬针对GameFramework资源系统二次开发的Unity游戏框架
