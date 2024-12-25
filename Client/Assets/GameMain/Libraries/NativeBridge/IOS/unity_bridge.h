//
//  unity_bridge.h
//  Unity-iPhone
//
//  Created by maped on 2024/3/6.
//

#ifndef unity_bridge_h
#define unity_bridge_h

#include <stdio.h>

#ifdef _MSC_VER
    #ifdef __cplusplus
        #ifdef DHPLAYER_EXPORTS
            #define DHPLAYER_API extern "C" __declspec(dllexport)
        #else
            #define DHPLAYER_API extern "C" __declspec(dllimport)
        #endif
    #else
        #ifdef DHPLAYER_EXPORTS
            #define DHPLAYER_API __declspec(dllexport)
        #else
            #define DHPLAYER_API __declspec(dllimport)
        #endif
    #endif
#else
    #ifdef __cplusplus
        #ifdef DHPLAYER_EXPORTS
            #define DHPLAYER_API extern "C" __attribute__((visibility("default")))
        #else
            #define DHPLAYER_API extern "C"
        #endif
    #else
        #ifdef DHPLAYER_EXPORTS
            #define DHPLAYER_API __attribute__((visibility("default")))
        #else
            #define DHPLAYER_API
        #endif
    #endif
#endif

char* call_unity_sync(int callType,const char* jsonContent);
void call_unity(int callType,const char* timeStamp,const char* jsonContent);

#endif /* unity_bridge_h */
