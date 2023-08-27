using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace GameFramework.Resource
{
    internal partial class ResourceManager
    {
        /// <summary>
        /// StreamingAssets目录下资源查询帮助类
        /// </summary>
        public sealed class StreamingAssetsHelper
        {
#if UNITY_EDITOR
            public static void Init()
            {
            }

            public static bool FileExists(string packageName, string fileName)
            {
                string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, StreamingAssetsDefine.RootFolderName,
                    packageName, fileName);
                return System.IO.File.Exists(filePath);
            }
#else
            private static bool _isInit = false;
            private static readonly HashSet<string> _cacheData = new();

            /// <summary>
            /// 初始化
            /// </summary>
            public static void Init()
            {
                if (_isInit == false)
                {
                    _isInit = true;
                    var manifest = Resources.Load<BuildinFileManifest>("BuildinFileManifest");
                    if (manifest != null)
                    {
                        foreach (string fileName in manifest.BuildinFiles)
                        {
                            _cacheData.Add(fileName);
                        }
                    }
                }
            }

            /// <summary>
            /// 内置文件查询方法
            /// </summary>
            public static bool FileExists(string packageName, string fileName)
            {
                if (_isInit == false)
                    Init();
                return _cacheData.Contains(fileName);
            }
#endif
        }
    }
}