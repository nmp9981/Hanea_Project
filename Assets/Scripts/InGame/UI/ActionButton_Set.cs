using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;
    // 지식 타일 관리
    public KnowledgeBoard_Manager knowledgeBoardManager;

    //건물 업그레이드 UI(무역 스테이션)
    public GameObject detailInstallBuildingButtonSetObj;
    //패스 UI
    public GameObject passUIObj;
    //프리 액션 UI
    public GameObject freeActionUI;

    private void Awake()
    {
        BindingActionButtons();
    }

    /// <summary>
    /// 액션 버튼 바인딩
    /// </summary>
    void BindingActionButtons()
    {
        foreach (Button btn in gameObject.GetComponentsInChildren<Button>(true))
        {
            string buttonName = btn.gameObject.name;
            switch (buttonName)
            {
                case "InstallMine":
                    btn.onClick.AddListener(delegate { Action_InstallMine(); });
                    break;
                case "BuildingUpgrade":
                    btn.onClick.AddListener(delegate { Action_BuildingUpgrade(); });
                    break;
                case "Union"://연방 선언
                    btn.onClick.AddListener(delegate { Action_Union(); });
                    break;
                case "Pass"://패스
                    btn.onClick.AddListener(delegate { Action_Pass_Question(); });
                    break;
                case "Research"://연구 행동
                    btn.onClick.AddListener(delegate { Action_Research(); });
                    break;
                case "FreeAction"://프리 액션 행동
                    btn.onClick.AddListener(delegate { Action_Free(); });
                    break;
                case "ResearchLab":
                    btn.onClick.AddListener(delegate { Action_Upgrade_ResearchLab(); });
                    break;
                case "PlanetaryInstitute":
                    btn.onClick.AddListener(delegate { Action_Upgrade_PlanetaryInstitute(); });
                    break;
                case "InstallCancel":
                    btn.onClick.AddListener(delegate { Action_Upgrade_Cancel(); });
                    break;
                case "PassOK":
                    btn.onClick.AddListener(delegate { Action_Pass_OK(); });
                    break;
                case "PassNO":
                    btn.onClick.AddListener(delegate { Action_Pass_Cancel(); });
                    break;
                case "ChangeEnergyToQIC":
                    btn.onClick.AddListener(delegate { PlayerManager.Instance.OnClick_Exchange_EnergyToQuantumIntelligenceCube(); });
                    break;
                case "ChangeEnergyToOre":
                    btn.onClick.AddListener(delegate { PlayerManager.Instance.OnClick_Exchange_EnergyToOre(); });
                    break;
                case "ChangeEnergyToKnowledge":
                    btn.onClick.AddListener(delegate { PlayerManager.Instance.OnClick_Exchange_EnergyToKnowledge(); });
                    break;
                case "ChangeEnergyToMoney":
                    btn.onClick.AddListener(delegate { PlayerManager.Instance.OnClick_Exchange_EnergyToMoney(); });
                    break;
                case "ChangeOreToMoney":
                    btn.onClick.AddListener(delegate { PlayerManager.Instance.OnClick_Exchange_OreToMoney(); });
                    break;
                case "ChangeOreToEnergy":
                    btn.onClick.AddListener(delegate { PlayerManager.Instance.OnClick_Exchange_OreToEnergy(); });
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 클릭한 타일 가져오기
    /// </summary>
    /// <returns></returns>
    private Tile GetClickedTile()
    {
        Tile clickedTile = PlayerManager.Instance.ClickedTile();
        if (clickedTile == null)
        {
            Debug.LogWarning("클릭된 타일이 없습니다.");
        }
        return clickedTile;
    }

    /// <summary>
    /// 광산 설치
    /// </summary>
    /// <param name="cilckTile"></param>
    public void Action_InstallMine()
    {
        //클릭한 타일 가져오기
        Tile clickTile = GetClickedTile();
        //클릭한 타일이 존재해야함
        if (clickTile == null) return;

        //설치된 건물이 없어야함
        if (clickTile.InstallBuildingImage.sprite != null) return;

        //행성이 존재해야함
        if (clickTile.PlanetType == Planet.None || clickTile.PlanetType == Planet.Count) return;

        //광산이 남아있는가?
        if (BuildingManager.Instance.mineImage_UIStack.Count == 0) return;
        
        //추가 삽비용
        (int addSabCost, int sabCount)= TileSystem.RequireSabCount(clickTile.PlanetType);

        //사거리 검사

        
        //비용 검사
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[0])) return;
        if (resourcesManager.HasEnoughResources("Ore", addSabCost) == false) return;
        if(clickTile.PlanetType == Planet.Gaia)//가이아 행성은 정보 큐브 1개 추가 필요
        {
            if (resourcesManager.HasEnoughResources("Quantum Intelligence Cube", 1) == false) return;
        }

        //비용 지불
        PayForBuilding(BuildingManager.Instance.buildingDataList[0]);
        resourcesManager.ConsumeResource("Ore", addSabCost);
        if (clickTile.PlanetType == Planet.Gaia)//가이아 행성은 정보 큐브 1개 추가 필요
        {
            resourcesManager.ConsumeResource("Quantum Intelligence Cube", 1);
        }

        //광산 설치
        clickTile.ChangeBuildingImageAndPower(Building.Mine);

        //광석 수입 증가
        resourcesManager.ImportResourceAmount_UpDown("Ore", 1);

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

        //점령행성 종류 추가
        if (PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] == false)
        {
            PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] = true;
            BuildingManager.Instance.planetInstallMineDic[clickTile.PlanetType].enabled = true;
        }

        //플레이어 UI에서 광산 제거
        BuildingManager.Instance.last_mineImage = BuildingManager.Instance.mineImage_UIStack.Peek();
        BuildingManager.Instance.last_mineImage.enabled = false;
        BuildingManager.Instance.mineImage_UIStack.Pop();
    }

    /// <summary>
    /// 건물 업그레이드
    /// </summary>
    public void Action_BuildingUpgrade()
    {
        //클릭한 타일 가져오기
        Tile clickTile = PlayerManager.Instance.ClickedTile();
        //클릭한 타일이 존재해야함
        if (clickTile == null) return;

        //설치된 건물이 있어야함
        if (clickTile.InstallBuildingImage.sprite == null) return;

        //파워 3이상이면 업그레이드 불가(행성 의회, 아카데미)
        if (clickTile.TilePower >= 3) return;

        //건물 종류에 따라 업그레이드가 다름
        switch (clickTile.InstallBuilding)
        {
            case Building.Mine://광산 -> 무역 스테이션
                if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[1])) return;

                //무역 스테이션이 남아있는가?
                if (BuildingManager.Instance.tradingStation_UIStack.Count == 0) return;

                PayForBuilding(BuildingManager.Instance.buildingDataList[1]);
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                resourcesManager.ImportResourceAmount_UpDown("Money", 3);
                resourcesManager.ImportResourceAmount_UpDown("Ore", -1);

                //플레이어 UI에서 광산 , 무역스테이션 교체
                BuildingManager.Instance.last_tradingStationImage = BuildingManager.Instance.tradingStation_UIStack.Peek();
                BuildingManager.Instance.last_tradingStationImage.enabled = false;
                BuildingManager.Instance.tradingStation_UIStack.Pop();
                BuildingManager.Instance.mineImage_UIStack.Push(BuildingManager.Instance.last_mineImage);
                BuildingManager.Instance.mineImage_UIStack.Peek().enabled = true;

                //라운드 보너스 점수
                if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.TradingI))
                {
                    if (GameManager.Instance.IsRoundEffectDic[RoundEffect.TradingI] == true)
                    {
                        PlayerManager.Instance.GetScore(3);
                    }
                }
                if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.TragindII))
                {
                    if (GameManager.Instance.IsRoundEffectDic[RoundEffect.TragindII] == true)
                    {
                        PlayerManager.Instance.GetScore(4);
                    }
                }
                break;
            case Building.TradingStation://무역 스테이션 -> 행성 의회 or 연구소
                detailInstallBuildingButtonSetObj.SetActive(true);
                break;
            case Building.ResearchLab://연구소 -> 아카데미
                if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[3])) return;

                //아카데미가 남아있는가?
                if (BuildingManager.Instance.academy_UIStack.Count == 0) return;

                PayForBuilding(BuildingManager.Instance.buildingDataList[3]);
                clickTile.ChangeBuildingImageAndPower(Building.Academy);
                resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
                KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();//기술 타일 획득

                //플레이어 UI에서 연구소, 아카데미 교체
                BuildingManager.Instance.academy_UIStack.Peek().enabled = false;
                BuildingManager.Instance.academy_UIStack.Pop();
                BuildingManager.Instance.researchLab_UIStack.Push(BuildingManager.Instance.researchLabImage);
                BuildingManager.Instance.researchLab_UIStack.Peek().enabled = true;

                if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.Power3))
                {
                    if (GameManager.Instance.IsRoundEffectDic[RoundEffect.Power3] == true)
                    {
                        PlayerManager.Instance.GetScore(5);
                    }
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 건물 설치 비용 지불 가능 여부 체크
    /// </summary>
    /// <param name="buildingData">확인할 건물 데이터</param>
    /// <returns>비용 지불 가능 여부</returns>
    private bool CanAffordBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return false;

        foreach (var cost in buildingData.costs)
        {
            if (resourcesManager.HasEnoughResources(cost.resourceName, cost.amount) == false)
            {
                return false;
            }
        }

        //광산 추가 비용
        if (buildingData.type == Building.Mine)
        {
            if (resourcesManager.HasEnoughResources("Ore", 0) == false)
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
    private void PayForBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return;

        foreach (var cost in buildingData.costs)
        {
            resourcesManager.ConsumeResource(cost.resourceName, cost.amount);
        }

        //광산 추가 비용
        if (buildingData.type == Building.Mine)
        {
            resourcesManager.ConsumeResource("Ore", 0);
        }
    }

    /// <summary>
    /// 무역 스테이션 -> 연구소
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_ResearchLab()
    {
        //클릭한 타일 가져오기
        Tile clickTile = GetClickedTile();
        //클릭한 타일이 존재해야함
        if (clickTile == null) return;

        //비용 지불이 되는가?
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[2])) return;

        //연구소가 더 없으면 지을 수 없다
        if (BuildingManager.Instance.researchLab_UIStack.Count == 0) return;

        PayForBuilding(BuildingManager.Instance.buildingDataList[2]);
        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
        detailInstallBuildingButtonSetObj.SetActive(false);
        KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();//기술 타일 획득

        //플레이어 UI에서 무역스테이션, 연구소 교체
        BuildingManager.Instance.researchLabImage = BuildingManager.Instance.researchLab_UIStack.Peek();
        BuildingManager.Instance.researchLabImage.enabled = false;
        BuildingManager.Instance.researchLab_UIStack.Pop();
        BuildingManager.Instance.tradingStation_UIStack.Push(BuildingManager.Instance.last_tradingStationImage);
        BuildingManager.Instance.tradingStation_UIStack.Peek().enabled = true;
    }

    /// <summary>
    /// 무역 스테이션 -> 행성 의회
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_PlanetaryInstitute()
    {
        //클릭한 타일 가져오기
        Tile clickTile = GetClickedTile();
        //클릭한 타일이 존재해야함
        if (clickTile == null) return;

        //비용 지불이 되는가?
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[4])) return;

        //행성 의회는 1개만 지을 수 있음
        if (BuildingManager.Instance.institute_UIStack.Count == 0) return;

        PayForBuilding(BuildingManager.Instance.buildingDataList[4]);
        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
        detailInstallBuildingButtonSetObj.SetActive(false);

        //플레이어 UI에서 무역스테이션, 행성 의회 교체
        BuildingManager.Instance.institute_UIStack.Peek().enabled = false;
        BuildingManager.Instance.institute_UIStack.Pop();
        BuildingManager.Instance.tradingStation_UIStack.Push(BuildingManager.Instance.last_tradingStationImage);
        BuildingManager.Instance.tradingStation_UIStack.Peek().enabled = true;

        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.Power3))
        {
            if (GameManager.Instance.IsRoundEffectDic[RoundEffect.Power3] == true)
            {
                PlayerManager.Instance.GetScore(5);
            }
        }
    }
    /// <summary>
    /// 무역 스테이션 -> 행성 의회
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_Cancel()
    {
        detailInstallBuildingButtonSetObj.SetActive(false);
    }

    /// <summary>
    /// 연구 행동 
    /// 지식 트랙 한칸 전진
    /// </summary>
    private void Action_Research()
    {
        //비용 지불이 가능한가?
        if (!resourcesManager.HasEnoughResources("Knowledge", 4)) return;

        //어떤 지식타일을 선택할 것인가?
        knowledgeBoardManager.ActivateKnowledgeTile(ResearchType.Count);
    }

    /// <summary>
    /// 연방 액션
    /// </summary>
    private void Action_Union()
    {
        //연방 불가
        if (!PlayerManager.Instance.Check_AbleUnion(PlayerManager.Instance.ClickedTileList())) return;
        
        //연방 등록
        //건물있는지 검사하고 없으면 위성을 놓는다.(위성은 개당 에너지 1개)
        int pay = PlayerManager.Instance.EnrollUnion_ReturnCountSatellite();
        //비용 지불
        for (int i = 0; i < pay; i++)
        {
            PayForBuilding(BuildingManager.Instance.buildingDataList[5]);
        }
        //연방 토큰 가져오기
        KnowledgeBoard_Manager.Instance.Activate_AllUnionTile_UI();
    }

    #region Pass
    /// <summary>
    /// 패스 여부
    /// </summary>
    private void Action_Pass_Question()
    {
        //UI를 띄운다.
        passUIObj.SetActive(true);
    }
    /// <summary>
    /// 패스
    /// </summary>
    private void Action_Pass_OK()
    {
        passUIObj.SetActive(false);
        if (GameManager.Instance.currentRound != 6)//마지막 라운드가 아닐때만
        {
            resourcesManager.ImportAllResources();//수입
            TileManager.Instance.ActivateAllTiles();//타일 활성화
        }
        GameManager.Instance.ShowRoundText();//다음 라운드
    }
    /// <summary>
    /// 패스 취소
    /// </summary>
    private void Action_Pass_Cancel()
    {
        passUIObj.SetActive(false);
    }
    #endregion

    private void Action_Free()
    {
        if(freeActionUI.activeSelf) freeActionUI.SetActive(false);
        else freeActionUI.SetActive(true);
    }
}
