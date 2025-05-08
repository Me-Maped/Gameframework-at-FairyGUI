using GameFramework;
using GameFramework.Event;

namespace GameFramework.Localization
{
    public class LanguageChangeEventArgs : GameEventArgs
    {
        public static int EventId => typeof(LanguageChangeEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public Language Lang { get; private set; }

        public static LanguageChangeEventArgs Create(Language lang)
        {
            LanguageChangeEventArgs arg = ReferencePool.Acquire<LanguageChangeEventArgs>();
            arg.Lang = lang;
            return arg;
        }

        public override void Clear()
        {
            Lang = Language.Unspecified;
        }
    }
}