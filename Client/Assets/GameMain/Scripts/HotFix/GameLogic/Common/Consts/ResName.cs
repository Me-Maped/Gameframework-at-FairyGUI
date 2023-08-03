using GameFramework;

namespace GameLogic.Common
{
    public static class ResName
    {
        public const string UI_ATLAS = "Assets/AssetRaw/Atlas/UI/";
        public const string SCENE = "Assets/AssetRaw/Scenes/";
        public static string GetUIAtlas(string atlasName)
        {
            return Utility.Text.Format("{0}{1}", UI_ATLAS, atlasName);
        }

        public static string GetScene(string sceneName)
        {
            return Utility.Text.Format("{0}{1}.unity", SCENE, sceneName);
        }
    }
}