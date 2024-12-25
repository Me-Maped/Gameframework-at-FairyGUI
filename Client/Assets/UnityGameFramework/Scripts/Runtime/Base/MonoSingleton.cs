using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static volatile T _instance;
        private static object _lock = new Object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            T[] instances = FindObjectsOfType<T>();
                            if (instances != null)
                            {
                                foreach (var inst in instances)
                                {
                                    Destroy(inst);
                                }
                            }

                            GameObject go = new GameObject(typeof(T).Name);
                            _instance = go.AddComponent<T>();
                            DontDestroyOnLoad(go);
                            _instance.transform.SetParent(Utility.Unity.Entity);
                            _instance.Init();
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void Init() { }
    }
}