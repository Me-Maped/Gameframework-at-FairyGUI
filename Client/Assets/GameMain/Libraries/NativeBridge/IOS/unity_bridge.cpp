//
//  unity_bridge.cpp
//  Unity-iPhone
//
//  Created by maped on 2024/3/6.
//

#include "unity_bridge.h"

#define FUNC_DEF(FUNC_TYPE_NAME,FUNC_NAME,RETURN_TYPE,PARAMS...)\
typedef RETURN_TYPE (*FUNC_TYPE_NAME)(PARAMS);\
FUNC_TYPE_NAME FUNC_NAME = nullptr;\
DHPLAYER_API void reg_##FUNC_NAME(FUNC_TYPE_NAME func){FUNC_NAME = func;}

FUNC_DEF(NativeCallSyncDelegate, call_unity_sync_func, char*, int callType,const char* jsonContent)
FUNC_DEF(NativeCallDelegate, call_unity_wait_func, void, int callType,const char* timeStamp,const char* jsonContent)

char* call_unity_sync(int type,const char* json){
    if(call_unity_sync_func == nullptr){
        return "";
    }
    return call_unity_sync_func(type,json);
}

void call_unity(int type,const char* timeStamp,const char* json){
    if(call_unity_wait_func == nullptr){
        return;
    }
    call_unity_wait_func(type,timeStamp,json);
}


