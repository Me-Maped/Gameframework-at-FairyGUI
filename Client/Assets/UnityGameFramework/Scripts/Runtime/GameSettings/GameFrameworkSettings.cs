using UnityEngine;

[CreateAssetMenu(fileName = "GameFrameworkGlobalSettings", menuName = "Game Framework/GameFrameworkSettings")]
public class GameFrameworkSettings : ScriptableObject
{
    [Header("Framework")] [SerializeField] private FrameworkGlobalSettings m_FrameworkGlobalSettings;

    public FrameworkGlobalSettings FrameworkGlobalSettings
    {
        get { return m_FrameworkGlobalSettings; }
    }

    [Header("HybridCLR")] [SerializeField] private HybridCLRCustomGlobalSettings m_BybridCLRCustomGlobalSettings;

    public HybridCLRCustomGlobalSettings BybridCLRCustomGlobalSettings
    {
        get { return m_BybridCLRCustomGlobalSettings; }
    }
}
