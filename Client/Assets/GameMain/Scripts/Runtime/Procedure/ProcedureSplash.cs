using GameFramework.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 闪屏。
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // 播放 Splash 动画
            //Splash.Active(splashTime:3f);
            //热更新阶段文本初始化
            UILoadMgr.Instance.InitUI();
            UILoadMgr.Instance.ShowForm();
            //初始化资源包
            ChangeState<ProcedureInitPackage>(procedureOwner);
        }
    }
}
