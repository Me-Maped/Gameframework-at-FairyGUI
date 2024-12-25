//
// Source code recreated from a .class file by IntelliJ IDEA
// (powered by FernFlower decompiler)
//

package com.unity.bridge;

import java.util.HashMap;

public class UnityBridge {
    private static String TAG = "UnityBridge";
    private static HashMap<Integer, INativeCallback> nativeCallbackMap = new HashMap<>();
    private static HashMap<String,IUnityCallback> unityCallbackMap = new HashMap<>();
    private static UnityBridge instance;
    private static IUnityCaller unityCaller;

    private UnityBridge(){}

    public static UnityBridge getInstance(){
        if(instance == null){
            instance = new UnityBridge();
        }
        return instance;
    }

    // Unity使用，不要手动调用
    public static void setBridgeCallback(IUnityCaller caller){
        unityCaller = caller;
    }
    // Unity使用，不要手动调用
    public static void onUnityCall(int callType,String strData){
        INativeCallback receiver = nativeCallbackMap.get(callType);
        if (receiver != null) {
            receiver.invoke(callType,strData);
        }
    }
    // Unity使用，不要手动调用
    public static void onUnityCallAsync(String timeStamp,String strData){
        IUnityCallback callback = unityCallbackMap.get(timeStamp);
        if(callback!=null){
            callback.invoke(strData);
        }
    }

    // 原生端调用，同步返回值
    public String callUnitySync(int callType, String strData) {
        if(unityCaller !=null){
            return unityCaller.callUnitySync(callType,strData);
        }
        return null;
    }

    // 原生端调用，异步返回值
    public void callUnity(int callType, String strData,IUnityCallback callback) {
        if(unityCaller !=null){
            // 使用当前时间戳生成一个String作为timeStamp
            String timeStamp = String.valueOf(System.currentTimeMillis());
            unityCallbackMap.put(timeStamp,callback);
            unityCaller.callUnityWait(callType,timeStamp,strData);
        }
    }

    // 原生端调用 添加监听
    public void addListener(int callType, INativeCallback receiver){
        nativeCallbackMap.put(callType, receiver);
    }

    // 原生端调用 移除监听
    public void removeListener(int callType){
        nativeCallbackMap.remove(callType);
    }
}
