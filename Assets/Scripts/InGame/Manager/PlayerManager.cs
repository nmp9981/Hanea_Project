using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    private void Awake()
    {
        resourcesManager = GameManager.Instance.resourcesManager;
    }
}
