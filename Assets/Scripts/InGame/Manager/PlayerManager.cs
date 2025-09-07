using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    // 자원 교환 로직을 담당하는 인스턴스
    private ExchangeResources resourceExchanger;

    public void OnClickTradeButton()
    {
        // 버튼 클릭 시, 자원 교환 로직 호출
        // 나무 50개를 소모하여 골드 10개로 교환
        resourceExchanger.Exchange_AToB("Wood", 50, "Gold", 10);
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
}
