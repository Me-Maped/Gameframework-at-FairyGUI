using System;
using UnityGameFramework.Runtime;

namespace GameLogic.Login
{
    public class LoginFormModel : UIModelBase<LoginFormModel>
    {
        public Action<int> OnTestNumChange;

        private int _testNum;
        public int TestNum
        {
            get => _testNum;
            set
            {
                if (_testNum == value) return;
                _testNum = value;
                Trigger(nameof(TestNum), value);
            }
        }

        public override void Clear()
        {
            
        }

        public void Test()
        {
            TestNum++;
        }
    }
}