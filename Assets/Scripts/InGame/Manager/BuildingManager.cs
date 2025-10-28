using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildingManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BuildingManager Instance { get; private set; }

    [SerializeField]
    private List<Sprite> _buildingSpriteList;

    const string mineText = "Mine";

    //건물 데이터
    [SerializeField]
    public List<BuildingData> buildingDataList = new();

    //건물 UI
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
            // DontDestroyOnLoad(gameObject); // 필요시 주석 해제
        }
        else
        {
            Destroy(gameObject);
        }

        EnrollBuldingImage();//건물 이미지 등록
    }
   
    /// <summary>
    /// 건물 이미지 등록
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

        // Enum.GetValues()를 사용하여 Planet enum의 모든 값을 배열로 가져옵니다.
        Planet[] planets = (Planet[])Enum.GetValues(typeof(Planet));

        // foreach 루프를 사용하여 배열의 각 값을 순서대로 탐색합니다.
        foreach (Planet p in planets)
        {
            if (p == Planet.None || p == Planet.Count) continue;

            Image mineImage = GameObject.Find(p + mineText).GetComponent<Image>();
            mineImage.enabled = false;
            planetInstallMineDic.Add(p, mineImage);
        }
    }

    /// <summary>
    /// 건물 스프라이트 리스트 반환
    /// </summary>
    public Sprite GetBuildingSprite(Building buildingType)
    {
        // buildingType에 해당하는 스프라이트 반환
        return _buildingSpriteList[(int)buildingType];
    }
    public void InstallMine(Tile clickTile, int disCount)
    {
        //추가 삽비용
        (int addSabCost, int sabCount) = TileSystem.RequireSabCount(clickTile.PlanetType, disCount);

        //사거리 검사
        int nearestDist = TileSystem.NearestNavigaitionDist(clickTile);
        int addQICCost = TileSystem.AddQIC_Navigaition(nearestDist);

        //비용 검사
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[0])) return;
        if (ResourcesManager.Instance.HasEnoughResources("Ore", addSabCost) == false) return;
        if (ResourcesManager.Instance.HasEnoughResources("Quantum Intelligence Cube", addQICCost) == false) return;
        if (clickTile.PlanetType == Planet.Gaia)//가이아 행성은 정보 큐브 1개 추가 필요, 총 지불은 사거리 비용까지 포함
        {
            if (ResourcesManager.Instance.HasEnoughResources("Quantum Intelligence Cube", 1 + addQICCost) == false) return;
        }
        if (clickTile.PlanetType == Planet.Dimension)//차원 변환 행성은 에너지 4개 추가 필요
        {
            if (ResourcesManager.Instance.HasEnoughResources("Energy", 4) == false) return;
        }

        //비용 지불
        PayForBuilding(BuildingManager.Instance.buildingDataList[0]);
        ResourcesManager.Instance.ConsumeResource("Ore", addSabCost);
        ResourcesManager.Instance.ConsumeResource("Quantum Intelligence Cube", addQICCost);
        //가이아, 차원 변환 행성 개수 증가
        if (clickTile.PlanetType == Planet.Gaia)//가이아 행성은 정보 큐브 1개 추가 필요
        {
            ResourcesManager.Instance.ConsumeResource("Quantum Intelligence Cube", 1);
            GameManager.Instance.finalBonusList[0].CountUP();
        }
        if (clickTile.PlanetType == Planet.Dimension)//차원 변환 행성은 에너지 4개 추가 필요
        {
            ResourcesManager.Instance.ConsumeResource("Energy", 4);
            GameManager.Instance.finalBonusList[0].CountUP();
        }

        //광산 설치
        clickTile.ChangeBuildingImageAndPower(Building.Mine);
        PlayerManager.Instance._installBuidingCount[Building.Mine] += 1;

        //광석 수입 증가(빈칸 예외)
        if (PlayerManager.Instance._installBuidingCount[Building.Mine]!=3)
            ResourcesManager.Instance.ImportResourceAmount_UpDown("Ore", 1);

        //점수 증가
        if (PlayerManager.Instance.IsGaiaScore)
        {
            PlayerManager.Instance.GetScore(3);
        }
        //라운드 보너스 점수
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

        //최종 점수 증가
        GameManager.Instance.finalBonusList[4].CountUP();

        //점령행성 종류 추가
        if (PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] == false)
        {
            PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] = true;
            planetInstallMineDic[clickTile.PlanetType].enabled = true;
            GameManager.Instance.finalBonusList[1].CountUP();
        }

        //플레이어 UI에서 광산 제거
        mineImage_UIList[PlayerManager.Instance._installBuidingCount[Building.Mine]-1].enabled = false;

        //타일 표시 초기화
        PlayerManager.Instance.AllClear_ClickTile();
    }

    /// <summary>
    /// 건물 설치 비용 지불 가능 여부 체크
    /// </summary>
    /// <param name="buildingData">확인할 건물 데이터</param>
    /// <returns>비용 지불 가능 여부</returns>
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

        //광산 추가 비용
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
    /// 건물 설치 비용 지불
    /// </summary>
    /// <param name="buildingData">비용을 지불할 건물 데이터</param>
    public void PayForBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return;

        foreach (var cost in buildingData.costs)
        {
            ResourcesManager.Instance.ConsumeResource(cost.resourceName, cost.amount);
        }

        //광산 추가 비용
        if (buildingData.type == Building.Mine)
        {
            ResourcesManager.Instance.ConsumeResource("Ore", 0);
        }
    }
}
