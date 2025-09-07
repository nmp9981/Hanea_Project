using UnityEngine;

/// <summary>
/// 플레이어의 액션 관리
/// </summary>
public class PlayerManager : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    // 자원 교환 로직을 담당하는 인스턴스
    private ExchangeResources resourceExchanger;

    public void OnClickTradeButton()
    {
        
    }
    private void Awake()
    {
        resourceExchanger = new ExchangeResources(resourcesManager.resources);
    }

    private void Start()
    {
        //자원 수입
        resourcesManager.ImportAllResources();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnClick_Exchange_OreToMoney();
        }
    }


    #region 자원 변환 - Free Action
    /// <summary>
    /// 광석 -> 돈
    /// </summary>
    public void OnClick_Exchange_OreToMoney()
    {
        resourceExchanger.Exchange_AToB("Ore", 1, "Money", 2);
    }
    /// <summary>
    /// 에너지 -> 돈
    /// </summary>
    public void OnClick_Exchange_EnergyToMoney()
    {
        resourceExchanger.Exchange_AToB("Energy", 1, "Money", 1);
    }
    /// <summary>
    /// 에너지 -> 광석
    /// </summary>
    public void OnClick_Exchange_EnergyToOre()
    {
        resourceExchanger.Exchange_AToB("Energy", 3, "Ore", 1);
    }
    /// <summary>
    /// 에너지 -> 지식
    /// </summary>
    public void OnClick_Exchange_EnergyToKnowledge()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Knowledge", 1);
    }
    /// <summary>
    /// 에너지 -> 돈
    /// </summary>
    public void OnClick_Exchange_EnergyToQuantumIntelligenceCube()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Quantum Intelligence Cube", 1);
    }

    #endregion
}
