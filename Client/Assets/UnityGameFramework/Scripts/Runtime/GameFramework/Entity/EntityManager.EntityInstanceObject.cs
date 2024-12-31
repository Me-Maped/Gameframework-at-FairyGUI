using GameFramework.ObjectPool;

namespace GameFramework.Entity
{
    internal sealed partial class EntityManager : GameFrameworkModule, IEntityManager
    {
        /// <summary>
        /// 实体实例对象。
        /// </summary>
        private sealed class EntityInstanceObject : ObjectBase
        {
            private object m_EntityAsset;
            private IEntityHelper m_EntityHelper;
            private IObjectPool<EntityInstanceObject> m_ObjectPool;

            public EntityInstanceObject()
            {
                m_EntityAsset = null;
                m_EntityHelper = null;
                m_ObjectPool = null;
            }

            public static EntityInstanceObject Create(string name, object entityAsset, IEntityHelper entityHelper, IObjectPool<EntityInstanceObject> objectPool)
            {
                if (entityAsset == null)
                {
                    throw new GameFrameworkException("Entity asset is invalid.");
                }

                if (entityHelper == null)
                {
                    throw new GameFrameworkException("Entity helper is invalid.");
                }

                EntityInstanceObject entityInstanceObject = ReferencePool.Acquire<EntityInstanceObject>();
                entityInstanceObject.Initialize(name, entityHelper.InstantiateEntity(entityAsset));
                entityInstanceObject.m_EntityAsset = entityAsset;
                entityInstanceObject.m_EntityHelper = entityHelper;
                entityInstanceObject.m_ObjectPool = objectPool;
                return entityInstanceObject;
            }

            public override void Clear()
            {
                base.Clear();
                m_EntityAsset = null;
                m_EntityHelper = null;
            }

            protected internal override void Release(bool isShutdown)
            {
                m_EntityHelper.ReleaseEntity(m_EntityAsset, Target, m_ObjectPool.GetObjectCount(Name) == 0);
            }
        }
    }
}
