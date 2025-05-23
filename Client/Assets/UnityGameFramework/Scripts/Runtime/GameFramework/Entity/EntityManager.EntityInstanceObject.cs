﻿using GameFramework.ObjectPool;

namespace GameFramework.Entity
{
    internal sealed partial class EntityManager : GameFrameworkModule, IEntityManager
    {
        /// <summary>
        /// 实体实例对象。
        /// </summary>
        private sealed class EntityInstanceObject : ObjectBase
        {
            private IEntityHelper m_EntityHelper;

            public EntityInstanceObject()
            {
                m_EntityHelper = null;
            }

            public static EntityInstanceObject Create(string name, object entityAsset, IEntityHelper entityHelper)
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
                entityInstanceObject.m_EntityHelper = entityHelper;
                return entityInstanceObject;
            }

            public override void Clear()
            {
                base.Clear();
                m_EntityHelper = null;
            }
            
            protected internal override void Release(bool isShutdown)
            {
                m_EntityHelper.ReleaseEntity(Name, Target);
            }
        }
    }
}
