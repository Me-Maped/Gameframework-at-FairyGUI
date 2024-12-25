//
//  UnityBridgeManager.h
//  TaoliDance
//
//  Created by maped on 2024/3/1.
//

#import <Foundation/Foundation.h>
#import "UnityBridgeDefine.h"

NS_ASSUME_NONNULL_BEGIN

@class BaseSdkProxy;

__attribute__ ((visibility("default")))
@interface UnityBridge : NSObject

+ (instancetype)new  NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;
- (id)copy           NS_UNAVAILABLE;
- (id)mutableCopy    NS_UNAVAILABLE;

@property (copy,   nonatomic) CommonCallbackHandle callbackHandle;
@property (strong, nonatomic) NSMutableDictionary *waitBlockDic;
@property (copy,   nonatomic) DHResultHandler loadResultHandler;
@property (copy,   nonatomic) DHResultHandler unloadResultHandler;

+ (instancetype)instance;

// 调用Unity函数
+ (void)callUnity:(int)callType strData:(const char*)strData callback:(UnityCallbackFunc)callback;
+ (char*)callUnitySync:(int)callType strData:(const char*)strData;
// 设置通用调用模式回调
+ (void)setCommonCallHandle:(CommonCallbackHandle) handle;

/// 设置 Mach-O 头部信息
/// 注意：
///  （1）若要设置，则必须在第一次加载引擎之前进行设置
/// @param header 头部信息
+ (void)setExecuteHeader:(const MachHeader*)header;
/// 加载Unity引擎
/// 注意：
/// （1）DHResult.code == DHResultCodeOK 表示开始加载或加载成功，否则开始加载失败或加载失败
/// （2）有两个加载状态：
///       - 状态1：引擎开始加载回调状态 DHResult.data == @(0）
///       - 状态2：引擎加载成功回调状态 DHResult.data == @(1）
/// @param resultHandler 引擎加载回调
+ (void)load:(DHResultHandler)resultHandler;
/// 卸载引擎
+ (void)unload;
/// 获取引擎容器视图
/// 注意：
///  （1）调用时机为在引擎加载成功之后调用
/// @return 容器视图
+ (UIView *)getContainerView;

@end

NS_ASSUME_NONNULL_END
