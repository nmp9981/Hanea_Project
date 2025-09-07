using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    // �ڿ� ��ȯ ������ ����ϴ� �ν��Ͻ�
    private ExchangeResources resourceExchanger;

    public void OnClickTradeButton()
    {
        // ��ư Ŭ�� ��, �ڿ� ��ȯ ���� ȣ��
        // ���� 50���� �Ҹ��Ͽ� ��� 10���� ��ȯ
        resourceExchanger.Exchange_AToB("Wood", 50, "Gold", 10);
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
}
