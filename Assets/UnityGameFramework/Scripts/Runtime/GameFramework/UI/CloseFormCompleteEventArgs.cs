namespace GameFramework.UI
{
    public class CloseFormCompleteEventArgs: GameFrameworkEventArgs
    {
        public string ConfigResName { get; private set; }
        public UIGroupEnum ConfigGroupEnum { get; private set; }
        public CloseFormCompleteEventArgs()
        {
             ConfigResName = null;
             ConfigGroupEnum = UIGroupEnum.NONE;
        }
        public override void Clear()
        {
            ConfigResName = null;
            ConfigGroupEnum = UIGroupEnum.NONE;
        }

        public static CloseFormCompleteEventArgs Create(string configResName, UIGroupEnum configGroupEnum)
        {
            CloseFormCompleteEventArgs args = ReferencePool.Acquire<CloseFormCompleteEventArgs>();
            args.ConfigResName = configResName;
            args.ConfigGroupEnum = configGroupEnum;
            return args;
        }
    }
}