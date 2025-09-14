using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static BuildingManager Instance { get; private set; }

    [SerializeField]
    private List<Sprite> _buildingSpriteList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // �ʿ�� �ּ� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
    /// <summary>
    /// �ǹ� ����
    /// ��� Ÿ�Ͽ� �ش� �ǹ��� ���´�.
    /// </summary>
    public void InstallBuiling(Tile tile, Building buildingType)
    {

    }
    /// <summary>
    /// �ǹ� ��������Ʈ ����Ʈ ��ȯ
    /// </summary>
    public Sprite GetBuildingSprite(Building buildingType)
    {
        // buildingType�� �ش��ϴ� ��������Ʈ ��ȯ
        return _buildingSpriteList[(int)buildingType];
    }
}
