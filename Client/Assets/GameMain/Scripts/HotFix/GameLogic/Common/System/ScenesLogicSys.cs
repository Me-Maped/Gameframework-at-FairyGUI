using Cysharp.Threading.Tasks;
using UGFExtensions.Await;
using UnityGameFramework.Runtime;

namespace GameLogic.Common
{
    public class ScenesLogicSys : BaseLogicSys<ScenesLogicSys>
    {
        private const string BaseEnterScene = "Assets/AssetRaw/Scenes/Game.unity";
        public async UniTask EnterDefaultScene()
        {
            if (GameModule.Resource.CheckLocationValid(GameModule.Base.EntrySceneName))
            {
                await GameModule.Scene.LoadSceneAsync(GameModule.Base.EntrySceneName);
            }
            else 
            {
                if(string.IsNullOrEmpty(GameModule.Base.EntrySceneName))
                {
                    Log.Error("Entry scene name is empty");
                    await GameModule.Scene.LoadSceneAsync(BaseEnterScene);
                    return;
                }
#if UNITY_EDITOR
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(GameModule.Base.EntrySceneName, new UnityEngine.SceneManagement.LoadSceneParameters()
                {
                    loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single,
                    localPhysicsMode = UnityEngine.SceneManagement.LocalPhysicsMode.None,
                });
                GameModule.UI.UICameraAttach(null);
                return;
#endif
                Log.Error("Entry scene not in bundle: {0}. enter default scene", GameModule.Base.EntrySceneName);
                await GameModule.Scene.LoadSceneAsync(BaseEnterScene);
            }
        }
    }
}