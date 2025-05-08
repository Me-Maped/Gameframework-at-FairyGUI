# GameFramework-at-FairyGUI

GameFramework + FairyGUI +Luban + HybridCLR + YooAsset + UniTask + GeekServer

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
#### AssetRaw: 资源目录
注：如需删除目录，需将YooAssetsCollection中对应的目录收集信息删除。以下均为受资源管理的目录，如需新增，需要在YooAssets中同步添加。
* Actor，角色目录
* Atlas，图集目录
* Audios，音频目录
* Configs，表配置生成目录
* DLL，热更新程序集目录
* Effects，特效目录
* Fonts，字体文件目录
* Materials，材质目录
* Prefabs，预制体目录
* Scenes，场景目录
* Shaders，Shader及变体收集目录
* UI，FairyGUI文件生成目录
* Version，版本信息文件目录

#### GameMain: 主程序代码目录
* AssetSetting，YooAsset资源配置文件目录
* Libraries，主程序引用的三方库目录
* Resources，首包资源目录：
    + Settings，系统设置，首包多语言设置目录
    + UI，首包UI资源
* Scripts，主程序代码
    + Editor， 编辑器扩展代码
    + HotFix， 主逻辑代码（可热更新代码）
        - GameLogic，逻辑代码（注意，其中Generate为FairyGUI生成的组件代码，为方便主逻辑中调用UI组件将其划分为同一程序集，如需自定义生成位置，需要将UIProject/Game工程里生成路径配置为指定目录）。GameApp.cs为主逻辑代码入口。
        - GameProto，表结构与网络协议等生成目录
    + Runtime，首包逻辑代码（不可热更新）。GameEntry.cs为模块启动主入口
        - Extention，扩展方法
        - Loading，首包UI逻辑
        - NativeBridge，APP桥接逻辑
        - Network，Protobuf长链接实现
        - Procedure，启动流程逻辑
        - Util，工具类

#### UnityGameFramework: 框架代码目录
* Libraries，框架依赖的三方库
* ResRaw，框架配置与必须资源
* Scrips （略，与GameFramework相似）

### UI及功能制作流程
UI使用FairyGUI编辑器制作，项目分为Game(主游戏工程)和Launch(启动器工程，即首包UI)。Game中除Fairy生成的目录外还有一个plugins，用于自定义组件partial代码。生成资源与代码的路径在项目设置中，注意Game和Launch生成路径不同，且生成代码不同。Launch部分的管理器见[UILoadMgr](Client/Assets/GameMain/Scripts/Runtime/Loading/UILoadMgr.cs)，Game部分则由[UIComponent](Client/Assets/UnityGameFramework/Scripts/Runtime/UI/UIComponent.cs)管理。UI制作请自行前往官网查看教程。
#### UI模块功能创建
1. 在FairyEditor中制作一个UI界面，在其导出设置中选择为其发布代码，生成的代码会在GameLogic/Generate中。
2. 如果导出的是一个全新的包（Fairy中Package的概念），则发布后需要点击引擎工具栏中`GameFramework/FairyGUI/导出UIBinder`选项，为新生成的包初始化一个组件绑定。
3. 新建一个界面类继承自UIForm，泛型类型为界面组件类型，如示例[LoginForm](Client/Assets/GameMain/Scripts/HotFix/GameLogic/Login/UI/LoginForm.cs)中`UI_login_main`为对应UI组件生成类型。若为复杂界面，UIForm还提供带Controller和Model的泛型。
4. 重写`CreateConfig`方法，为其返回一个界面参数。参数详情见其对应的返回类型中的描述。
5. 调用`GameModule.UI.OpenForm<T>()`即可打开对应界面
6. 使用MVC时其生命周期见基类`UIFormBase`，Model>Controller>View。


### 代码热更新
~~TODO~~

### 资源打包
~~TODO~~


### 其他功能
GeekServer服务启动测试：见Server下README启动流程。已支持Protobuf下TCP和Websocket协议连接。


## <strong>特别鸣谢
#### <a href="https://github.com/tuyoogame/YooAsset"><strong>YooAsset</strong></a> - YooAsset是一套商业级经历百万DAU游戏验证的资源管理系统。
#### <a href="https://fairygui.com"><strong>FairyGUI</strong></a> - FairyGUI是一个重视设计，摈弃脚本与配置的专业游戏UI解决方案。
#### <a href="https://hybridclr.doc.code-philosophy.com/"><strong>HybridCLR</strong></a> - HybridCLR是一个特性完整、零成本、高性能、低内存的近乎完美的Unity全平台原生c#热更方案。
#### <a href="https://github.com/Alex-Rachel/GameFramework-Next"><strong>GameFramework-at-YooAsset</strong></a> - GameFramework-at-YooAsset 是ALEXTANGXIAO大佬针对GameFramework资源系统二次开发的Unity游戏框架
#### <a href="https://github.com/leeveel/GeekServer"><strong>GeekServer</strong></a> - GeekServer GeekServer 是一个开源的分区分服的游戏服务器框架，采用 C# .Netcore 开发，开发效率高，性能强，跨平台，并内置不停服热更新机制。可以满足绝大部分游戏类型的需求，特别是和 Unity3D 协同开发更佳。
