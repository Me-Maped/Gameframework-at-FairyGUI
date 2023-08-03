using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    public class ProcedureClearCache:ProcedureBase
    {
        private ProcedureOwner _procedureOwner;
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;
            Log.Info("清理未使用的缓存文件！");

            UILoadMgr.Instance.SetProgress("清理未使用的缓存文件...");
            
            var operation = GameModule.Resource.ClearUnusedCacheFilesAsync();
            operation.Completed += Operation_Completed;
        }
        
        
        private void Operation_Completed(YooAsset.AsyncOperationBase obj)
        {
            UILoadMgr.Instance.SetProgress("清理完成 即将进入游戏...");
            ChangeState<ProcedureLoadAssembly>(_procedureOwner);
        }
    }
}