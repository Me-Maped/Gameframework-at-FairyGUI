package com.unity.bridge;

public interface IUnityCaller {
    String callUnitySync(int callType, String strData);
    void callUnityWait(int callType, String timeStamp, String strData);
}
