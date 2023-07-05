using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.ObjectPool;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UGFExtensions.Await;
using UGFExtensions.Timer;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UGFExtensions.SpriteCollection
{
    public partial class SpriteCollectionComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 散图集合对象池
        /// </summary>
        private IObjectPool<SpriteCollectionItemObject> m_SpriteCollectionPool;
        
        /// <summary>
        /// 检查是否可以释放间隔
        /// </summary>
        [SerializeField] private float m_CheckCanReleaseInterval = 30f;

        private float m_CheckCanReleaseTime = 0.0f;

        /// <summary>
        /// 对象池自动释放时间间隔
        /// </summary>
        [SerializeField] private float m_AutoReleaseInterval = 60f;
#if ODIN_INSPECTOR
        [ReadOnly] [ShowInInspector]
#endif
        private LinkedList<LoadSpriteObject> m_LoadSpriteObjectsLinkedList;
        
        private HashSet<string> m_SpriteCollectionBeingLoaded;
        private Dictionary<string, LinkedList<ISetSpriteObject>> m_WaitSetObjects;
#if UNITY_EDITOR
        public LinkedList<LoadSpriteObject> LoadSpriteObjectsLinkedList
        {
            get => m_LoadSpriteObjectsLinkedList;
            set => m_LoadSpriteObjectsLinkedList = value;
        }
#endif
        private void Start()
        {
            ObjectPoolComponent objectPoolComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ObjectPoolComponent>();
            m_SpriteCollectionPool = objectPoolComponent.CreateMultiSpawnObjectPool<SpriteCollectionItemObject>(
                "SpriteCollection",
                m_AutoReleaseInterval, 16, 60, 0);
            m_LoadSpriteObjectsLinkedList = new LinkedList<LoadSpriteObject>();
            m_SpriteCollectionBeingLoaded = new HashSet<string>();
            m_WaitSetObjects = new Dictionary<string, LinkedList<ISetSpriteObject>>();

            InitializedResources();
        }
        
        private void Update()
        {
            m_CheckCanReleaseTime += Time.unscaledDeltaTime;
            if (m_CheckCanReleaseTime < (double)m_CheckCanReleaseInterval)
                return;
            ReleaseUnused();
        }
        /// <summary>
        /// 回收无引用的 Image 对应图集。
        /// </summary>
#if ODIN_INSPECTOR
        [Button("Release Unused")]
#endif
        public void ReleaseUnused()
        {
            LinkedListNode<LoadSpriteObject> current = m_LoadSpriteObjectsLinkedList.First;
            while (current != null)
            {
                var next = current.Next;
                if (current.Value.SpriteObject.IsCanRelease())
                {
                    m_SpriteCollectionPool.Unspawn(current.Value.Collection);
                    ReferencePool.Release(current.Value.SpriteObject);
                    m_LoadSpriteObjectsLinkedList.Remove(current);
                }

                current = next;
            }
        }
    }
}