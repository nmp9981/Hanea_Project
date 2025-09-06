using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    private void Awake()
    {
        resourcesManager = GameManager.Instance.resourcesManager;
    }
}
