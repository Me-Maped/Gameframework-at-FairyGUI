using FairyGUI;
using GameFramework.Resource;
using UnityEngine;

namespace GameFramework.UI
{
    public class UILoader : GLoader
    {
        private IResourceManager m_ResourceManager;
        protected override async void LoadExternal()
        {
            m_ResourceManager ??= GameFrameworkEntry.GetModule<IResourceManager>();
            Texture tex = await m_ResourceManager.LoadAssetAsync<Texture>(url);
            if (isDisposed)
            {
                m_ResourceManager.UnloadAsset(url);
                return;
            }
            if (tex != null)
                onExternalLoadSuccess(new NTexture(tex));
            else
                onExternalLoadFailed();
        }

        protected override void FreeExternal(NTexture texture)
        {
            m_ResourceManager ??= GameFrameworkEntry.GetModule<IResourceManager>();
            m_ResourceManager.UnloadAsset(url);
        }
    }
}