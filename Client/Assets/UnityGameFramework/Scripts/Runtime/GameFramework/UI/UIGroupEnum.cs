namespace GameFramework.UI
{
    /// <summary>
    /// 界面组枚举。值越小，sortingOrder越大（取反了），z轴越小
    /// </summary>
    public enum UIGroupEnum
    {
        /// <summary>
        /// 无,不应该出现
        /// </summary>
        NONE = -1,
        
        /// <summary>
        /// 背景层
        /// </summary>
        BACKGROUND = 0,
        
        /// <summary>
        /// 战斗层
        /// </summary>
        BATTLE = -200,
        
        /// <summary>
        /// 界面层
        /// </summary>
        PANEL = -400,
        
        /// <summary>
        /// Tab层
        /// </summary>
        TAB = -500,
        
        /// <summary>
        /// 弹出层
        /// </summary>
        POP = -600,
        
        /// <summary>
        /// 提示层
        /// </summary>
        TIPS = -800,
        
        /// <summary>
        /// 引导层
        /// </summary>
        GUIDE=-1000,
        
        /// <summary>
        /// 加载层
        /// </summary>
        LOADING = -1200,
        
        /// <summary>
        /// 错误层
        /// </summary>
        ERROR = -1400,
    }
}