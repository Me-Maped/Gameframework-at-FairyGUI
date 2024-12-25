//
//  UnityBridgeDefine.h
//  UnityFramework
//
//  Created by maped on 2024/3/4.
//

#ifndef UnityBridgeDefine_h
#define UnityBridgeDefine_h

// BundleID 待修改
#define FRAMEWORK_BUNDLE_ID "com.mengineframework.player"


typedef void (^UnityCallbackFunc)(NSString *);
typedef void (^CommonCallbackHandle)(int callType,NSString * jsonContent);


typedef NS_ENUM(int, DHResultCode) {
    DHResultCodeOK                 = 0,             ///< 正常
    DHResultCodeInternalError      = -1,            ///< 内部错误，具体详见错误回调中 message 参数
    DHResultCodeCheckLicenseFailed = -2,            ///< SDK 鉴权失败
    DHResultCodeUnauthorized       = -3,            ///< SDK 尚未鉴权
    DHResultCodeInvalidParam       = -4,            ///< 参数初始化错误
    DHResultCodeServerError        = -5             ///< TTS 相关错误，具体详见错误回调中 message 参数
};

struct DHResult {
    DHResultCode code;                              ///< 结果码
    NSString *message;                                ///< 描述信息
    id data;                                          ///< 结果数据
};
typedef struct DHResult DHResult;

typedef void(^DHResultHandler)(DHResult result);

typedef struct
#ifdef __LP64__
    mach_header_64
#else
    mach_header
#endif
    MachHeader;

#endif /* UnityBridgeDefine_h */
