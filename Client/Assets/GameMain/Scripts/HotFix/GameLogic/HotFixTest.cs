using UnityEngine;

public class HotFixTest : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(0,0,Screen.width,Screen.height), "看到此条日志代表你成功运行了热更新代码");
    }
}