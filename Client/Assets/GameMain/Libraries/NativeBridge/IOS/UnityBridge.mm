//
//  UnityBridgeManager.m
//  TaoliDance
//
//  Created by maped on 2024/3/1.
//

#import "UnityBridge.h"
#import "UnityInterface.h"
#import "unity_bridge.h"
#import <UnityFramework/UnityFramework.h>

static int _argc = 0;
static char **_argv = nullptr;
static const MachHeader* _header = nullptr;
static NSDictionary *_appLaunchOpts = nil;

@interface UnityBridge () <LifeCycleListener>
@property (strong, nonatomic) UnityFramework *unityFramework;
@end

@implementation UnityBridge

+ (instancetype)instance {
    static dispatch_once_t onceToken;
    static UnityBridge *_instance = nil;
    dispatch_once(&onceToken, ^{
        _instance = [[self alloc] init];
        _instance.waitBlockDic = [NSMutableDictionary dictionary];
    });
    return _instance;
}

+ (void)callUnity:(int)callType strData:(const char*)strData callback:(UnityCallbackFunc)callback {
    NSTimeInterval currentTimeStamp = [[NSDate date] timeIntervalSince1970] * 1000;
    NSString *currentTimeStampString = [NSString stringWithFormat:@"%.0f", currentTimeStamp];
    [[self instance].waitBlockDic setObject:callback forKey:currentTimeStampString];
    call_unity(callType,[currentTimeStampString UTF8String], strData);
}

+ (char*)callUnitySync:(int)callType strData:(const char*)strData{
    return call_unity_sync(callType, strData);
}

+ (void)setCommonCallHandle:(CommonCallbackHandle)handle {
    [UnityBridge instance].callbackHandle = handle;
}

+ (void)setExecuteHeader:(const MachHeader*)header {
    _header = header;
    [[UnityBridge instance].unityFramework setExecuteHeader:header];
}
+ (void)load:(DHResultHandler)resultHandler {
    UnityBridge *manager = [UnityBridge instance];
    if (_header == nullptr) {
        NSLog(@"[UnityBridgeManager]: you should call +setExecuteHeader and set &_mh_execute_header");
    }
    manager.loadResultHandler = resultHandler;
    /* Register the listener of application lifecycle and unity lifecycle . */
    UnityUnregisterLifeCycleListener(manager);
    UnityRegisterLifeCycleListener(manager);
    /* Run the unity enginer with embed . */
    NSArray<NSString *> *arguments = [[NSProcessInfo processInfo] arguments];
    _argc = (int)arguments.count;
    char** argv = new char*[_argc];
    for (int index = 0; index < _argc; index++)
        argv[index] = (char *)[arguments[index] UTF8String];
    _argv = argv;
    [manager.unityFramework runEmbeddedWithArgc:_argc argv:_argv appLaunchOpts:_appLaunchOpts];
    manager.unityFramework.appController.quitHandler = ^(){
        NSLog(@"[NativeBridgeManager] AppController.quitHandler called");
        delete [] _argv;
    };
}
+ (void)unload{
    UnityUnloadApplication();
}
+ (UIView *)getContainerView {
    UnityBridge *manager = [UnityBridge instance];
    if (manager.unityFramework
        && manager.unityFramework.appController
        && manager.unityFramework.appController.rootView) {
        return manager.unityFramework.appController.rootView;
    }
    return nil;
}


#pragma mark - ### ------
#pragma mark - Getter
- (UnityFramework *)unityFramework {
    if (!_unityFramework) {
        NSBundle *bundle = [NSBundle bundleWithIdentifier:@FRAMEWORK_BUNDLE_ID];
        if ([bundle isLoaded] == false) [bundle load];
        _unityFramework = [UnityFramework getInstance];
        [_unityFramework setDataBundleId:@FRAMEWORK_BUNDLE_ID.UTF8String];
        if (![_unityFramework appController]) [_unityFramework setExecuteHeader:_header];
    }
    return _unityFramework;
}

#pragma mark - ------ Nofity ------
#pragma mark - ### ------
#pragma mark - UnityFrameworkListener
- (void)unityDidUnload:(NSNotification*)notification {
    UIView *rootView = self.unityFramework.appController.rootView;
    if (rootView) {
        [rootView removeFromSuperview];
    }
    UnityUnregisterLifeCycleListener(self);
    if(self.unloadResultHandler){
        DHResult result;
        result.code = DHResultCodeOK;
        result.message = @"unityEnterGame";
        result.data = @(0);
        self.unloadResultHandler(result);
    }
}

- (void)unityDidQuit:(NSNotification *)notification {
}



#pragma mark - ### ------
#pragma mark - ApplicationListener
- (void)willResignActive:(NSNotification *)notification {
    id app = [UIApplication sharedApplication];
    if ([self.unityFramework appController]) [[self.unityFramework appController] applicationWillResignActive:app];
}

- (void)didBecomeActive:(NSNotification*)notification {
    id app = [UIApplication sharedApplication];
    if ([self.unityFramework appController]) [[self.unityFramework appController] applicationDidBecomeActive:app];
}

- (void)willEnterForeground:(NSNotification *)notification {
    id app = [UIApplication sharedApplication];
    if ([self.unityFramework appController]) [[self.unityFramework appController] applicationWillEnterForeground:app];
}

- (void)didEnterBackground:(NSNotification *)notification {
    id app = [UIApplication sharedApplication];
    if ([self.unityFramework appController]) [[self.unityFramework appController] applicationDidEnterBackground:app];
}



#pragma mark - ### ------
#pragma mark - U3D call native of iOS
DHPLAYER_API void Call_Native(int callType,const char* strData)
{
    NSString* t_strData = nil;
    if(strData!=NULL && strlen(strData)>0)
    {
        t_strData=[NSString stringWithUTF8String:strData];
    }
    else
    {
        t_strData=@"";
    }
    [UnityBridge instance].callbackHandle(callType,t_strData);
}

DHPLAYER_API void Call_Native_Async(const char* timeStamp,const char* strData)
{
    NSString* t_strData = nil;
    if(strData!=NULL && strlen(strData)>0)
    {
        t_strData=[NSString stringWithUTF8String:strData];
    }
    else
    {
        t_strData=@"";
    }
    
    NSString* stampKey = [[NSString alloc] initWithUTF8String:timeStamp];
    UnityBridge *singleton = [UnityBridge instance];
    UnityCallbackFunc block = singleton.waitBlockDic[stampKey];
    if(block)
    {
        block(t_strData);
        [singleton.waitBlockDic removeObjectForKey:stampKey];
    }
    else
    {
        NSLog(@"[UnityBridgeManager] not find UnityCallbackFunc block");
    }
}

@end
