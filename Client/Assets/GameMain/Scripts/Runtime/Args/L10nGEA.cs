using GameFramework;
using GameFramework.Event;
using GameFramework.Localization;

namespace GameMain
{
    public class L10nGEA : GameEventArgs
    {
        public static int EventId = typeof(L10nGEA).GetHashCode();
        public override int Id => EventId;
        public Language Lang { get; private set; }
        public override void Clear()
        {
        }

        public static L10nGEA Create(Language lang)
        {
            var arg = ReferencePool.Acquire<L10nGEA>();
            arg.Lang = lang;            
            return arg;
        }
    }
}