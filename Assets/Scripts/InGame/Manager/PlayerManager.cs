using UnityEngine;

/// <summary>
/// �÷��̾��� �׼� ����
/// </summary>
public class PlayerManager : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    // �ڿ� ��ȯ ������ ����ϴ� �ν��Ͻ�
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
        //�ڿ� ����
        resourcesManager.ImportAllResources();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnClick_Exchange_OreToMoney();
        }
    }


    #region �ڿ� ��ȯ - Free Action
    /// <summary>
    /// ���� -> ��
    /// </summary>
    public void OnClick_Exchange_OreToMoney()
    {
        resourceExchanger.Exchange_AToB("Ore", 1, "Money", 2);
    }
    /// <summary>
    /// ������ -> ��
    /// </summary>
    public void OnClick_Exchange_EnergyToMoney()
    {
        resourceExchanger.Exchange_AToB("Energy", 1, "Money", 1);
    }
    /// <summary>
    /// ������ -> ����
    /// </summary>
    public void OnClick_Exchange_EnergyToOre()
    {
        resourceExchanger.Exchange_AToB("Energy", 3, "Ore", 1);
    }
    /// <summary>
    /// ������ -> ����
    /// </summary>
    public void OnClick_Exchange_EnergyToKnowledge()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Knowledge", 1);
    }
    /// <summary>
    /// ������ -> ��
    /// </summary>
    public void OnClick_Exchange_EnergyToQuantumIntelligenceCube()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Quantum Intelligence Cube", 1);
    }

    #endregion
}
