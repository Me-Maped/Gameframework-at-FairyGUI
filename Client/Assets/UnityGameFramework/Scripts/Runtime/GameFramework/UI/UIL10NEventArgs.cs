using FairyGUI;
using GameFramework.Event;
using GameFramework.Localization;

namespace GameFramework.UI
{
    public class UIL10NEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UIL10NEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public string L10NId { get; private set; }
        public GTextField TextField { get; private set; }

        public static UIL10NEventArgs Create(GTextField textField, string l10NId)
        {
            UIL10NEventArgs uiL10NEventArgs = ReferencePool.Acquire<UIL10NEventArgs>();
            uiL10NEventArgs.TextField = textField;
            uiL10NEventArgs.L10NId = l10NId;
            uiL10NEventArgs.RegisterL10NEvent();
            uiL10NEventArgs.InitDefault();
            return uiL10NEventArgs;
        }

        private void InitDefault()
        {
            OnL10nChanged(null, null);
        }

        private void RegisterL10NEvent()
        {
            GameFrameworkEntry.GetModule<IEventManager>().Subscribe(LanguageChangeEventArgs.EventId, OnL10nChanged);
        }

        private void OnL10nChanged(object sender, GameEventArgs e)
        {
            ILocalizationManager localizationManager = GameFrameworkEntry.GetModule<ILocalizationManager>();
            TextField.text = GameFrameworkEntry.GetModule<IUIManager>().L10NHelper.GetL10NString(localizationManager.Language, L10NId);
        }

        public override void Clear()
        {
            GameFrameworkEntry.GetModule<IEventManager>().Unsubscribe(LanguageChangeEventArgs.EventId, OnL10nChanged);
        }
    }
}