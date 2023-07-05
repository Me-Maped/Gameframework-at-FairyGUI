using System;
using GameFramework.UI;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public abstract class UIJumpHelperBase : MonoBehaviour,IUIJumpHelper
    {
        public abstract void Record(Type formType);
        
        public abstract void Back();

        public abstract void GoHome();
    }
}