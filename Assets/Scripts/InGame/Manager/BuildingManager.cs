using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildingManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static BuildingManager Instance { get; private set; }

    [SerializeField]
    private List<Sprite> _buildingSpriteList;

    const string mineText = "Mine";

    //�ǹ� ������
    [SerializeField]
    public List<BuildingData> buildingDataList = new();

    //�ǹ� UI
    [SerializeField]
    public List<Image> mineImage_UIList = new();
    [SerializeField]
    public Stack<Image> mineImage_UIStack = new();
    [SerializeField]
    public Image last_mineImage;
    [SerializeField]
    public List<Image> tradingStation_UIList = new();
    [SerializeField]
    public Stack<Image> tradingStation_UIStack = new();
    [SerializeField]
    public Image last_tradingStationImage;
    [SerializeField]
    public List<Image> researchLab_UIList = new();
    [SerializeField]
    public Stack<Image> researchLab_UIStack = new();
    [SerializeField]
    public Image researchLabImage;
    [SerializeField]
    public List<Image> academy_UIList = new();
    [SerializeField]
    public Stack<Image> academy_UIStack = new();
    [SerializeField]
    public List<Image> institute_UIList = new();
    [SerializeField]
    public Stack<Image> institute_UIStack = new();

    [SerializeField]
    public Dictionary<Planet, Image> planetInstallMineDic = new();
    public int sabDecreaseCount = 0;

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

        EnrollBuldingImage();//�ǹ� �̹��� ���
    }
   
    /// <summary>
    /// �ǹ� �̹��� ���
    /// </summary>
    private void EnrollBuldingImage()
    {
        foreach(var image in mineImage_UIList)
        {
            mineImage_UIStack.Push(image);
        }
        foreach (var image in tradingStation_UIList)
        {
            tradingStation_UIStack.Push(image);
        }
        foreach (var image in researchLab_UIList)
        {
            researchLab_UIStack.Push(image);
        }
        foreach (var image in academy_UIList)
        {
            academy_UIStack.Push(image);
        }
        foreach (var image in institute_UIList)
        {
            institute_UIStack.Push(image);
        }

        // Enum.GetValues()�� ����Ͽ� Planet enum�� ��� ���� �迭�� �����ɴϴ�.
        Planet[] planets = (Planet[])Enum.GetValues(typeof(Planet));

        // foreach ������ ����Ͽ� �迭�� �� ���� ������� Ž���մϴ�.
        foreach (Planet p in planets)
        {
            if (p == Planet.None || p == Planet.Count) continue;

            Image mineImage = GameObject.Find(p + mineText).GetComponent<Image>();
            mineImage.enabled = false;
            planetInstallMineDic.Add(p, mineImage);
        }
    }

    /// <summary>
    /// �ǹ� ��������Ʈ ����Ʈ ��ȯ
    /// </summary>
    public Sprite GetBuildingSprite(Building buildingType)
    {
        // buildingType�� �ش��ϴ� ��������Ʈ ��ȯ
        return _buildingSpriteList[(int)buildingType];
    }
    public void InstallMine(Tile clickTile, int disCount)
    {
        //�߰� ����
        (int addSabCost, int sabCount) = TileSystem.RequireSabCount(clickTile.PlanetType, disCount);

        //��Ÿ� �˻�
        int nearestDist = TileSystem.NearestNavigaitionDist(clickTile);
        int addQICCost = TileSystem.AddQIC_Navigaition(nearestDist);

        //��� �˻�
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[0])) return;
        if (ResourcesManager.Instance.HasEnoughResources("Ore", addSabCost) == false) return;
        if (ResourcesManager.Instance.HasEnoughResources("Quantum Intelligence Cube", addQICCost) == false) return;
        if (clickTile.PlanetType == Planet.Gaia)//���̾� �༺�� ���� ť�� 1�� �߰� �ʿ�, �� ������ ��Ÿ� ������ ����
        {
            if (ResourcesManager.Instance.HasEnoughResources("Quantum Intelligence Cube", 1 + addQICCost) == false) return;
        }
        if (clickTile.PlanetType == Planet.Dimension)//���� ��ȯ �༺�� ������ 4�� �߰� �ʿ�
        {
            if (ResourcesManager.Instance.HasEnoughResources("Energy", 4) == false) return;
        }

        //��� ����
        PayForBuilding(BuildingManager.Instance.buildingDataList[0]);
        ResourcesManager.Instance.ConsumeResource("Ore", addSabCost);
        ResourcesManager.Instance.ConsumeResource("Quantum Intelligence Cube", addQICCost);
        //���̾�, ���� ��ȯ �༺ ���� ����
        if (clickTile.PlanetType == Planet.Gaia)//���̾� �༺�� ���� ť�� 1�� �߰� �ʿ�
        {
            ResourcesManager.Instance.ConsumeResource("Quantum Intelligence Cube", 1);
            GameManager.Instance.finalBonusList[0].CountUP();
        }
        if (clickTile.PlanetType == Planet.Dimension)//���� ��ȯ �༺�� ������ 4�� �߰� �ʿ�
        {
            ResourcesManager.Instance.ConsumeResource("Energy", 4);
            GameManager.Instance.finalBonusList[0].CountUP();
        }

        //���� ��ġ
        clickTile.ChangeBuildingImageAndPower(Building.Mine);
        PlayerManager.Instance._installBuidingCount[Building.Mine] += 1;

        //���� ���� ����(��ĭ ����)
        if (PlayerManager.Instance._installBuidingCount[Building.Mine]!=3)
            ResourcesManager.Instance.ImportResourceAmount_UpDown("Ore", 1);

        //���� ����
        if (PlayerManager.Instance.IsGaiaScore)
        {
            PlayerManager.Instance.GetScore(3);
        }
        //���� ���ʽ� ����
        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.Mine))
        {
            if (GameManager.Instance.IsRoundEffectDic[RoundEffect.Mine] == true)
            {
                PlayerManager.Instance.GetScore(3);
            }
        }
        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.Terafoming))
        {
            if (GameManager.Instance.IsRoundEffectDic[RoundEffect.Terafoming] == true)
            {
                PlayerManager.Instance.GetScore(2 * sabCount);
            }
        }
        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.GaiaMineI))
        {
            if (GameManager.Instance.IsRoundEffectDic[RoundEffect.GaiaMineI] == true)
            {
                if (clickTile.PlanetType == Planet.Gaia)
                    PlayerManager.Instance.GetScore(3);
            }
        }
        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.GaiaMineII))
        {
            if (GameManager.Instance.IsRoundEffectDic[RoundEffect.GaiaMineII] == true)
            {
                if (clickTile.PlanetType == Planet.Gaia)
                    PlayerManager.Instance.GetScore(4);
            }
        }

        //���� ���� ����
        GameManager.Instance.finalBonusList[4].CountUP();

        //�����༺ ���� �߰�
        if (PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] == false)
        {
            PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] = true;
            planetInstallMineDic[clickTile.PlanetType].enabled = true;
            GameManager.Instance.finalBonusList[1].CountUP();
        }

        //�÷��̾� UI���� ���� ����
        mineImage_UIList[PlayerManager.Instance._installBuidingCount[Building.Mine]-1].enabled = false;

        //Ÿ�� ǥ�� �ʱ�ȭ
        PlayerManager.Instance.AllClear_ClickTile();
    }

    /// <summary>
    /// �ǹ� ��ġ ��� ���� ���� ���� üũ
    /// </summary>
    /// <param name="buildingData">Ȯ���� �ǹ� ������</param>
    /// <returns>��� ���� ���� ����</returns>
    public bool CanAffordBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return false;

        foreach (var cost in buildingData.costs)
        {
            if (ResourcesManager.Instance.HasEnoughResources(cost.resourceName, cost.amount) == false)
            {
                return false;
            }
        }

        //���� �߰� ���
        if (buildingData.type == Building.Mine)
        {
            if (ResourcesManager.Instance.HasEnoughResources("Ore", 0) == false)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// �ǹ� ��ġ ��� ����
    /// </summary>
    /// <param name="buildingData">����� ������ �ǹ� ������</param>
    public void PayForBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return;

        foreach (var cost in buildingData.costs)
        {
            ResourcesManager.Instance.ConsumeResource(cost.resourceName, cost.amount);
        }

        //���� �߰� ���
        if (buildingData.type == Building.Mine)
        {
            ResourcesManager.Instance.ConsumeResource("Ore", 0);
        }
    }
}
