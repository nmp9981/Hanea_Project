using UnityEngine;
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
    //아카데미 UI
    public GameObject academyUI;

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
                case "LeftAcademy":
                    btn.onClick.AddListener(delegate { BuildingManager.Instance.Install_LeftAcademy(); });
                    break;
                case "RightAcademy":
                    btn.onClick.AddListener(delegate { BuildingManager.Instance.Install_rightAcademy(); });
                    break;
                case "AcademyCancel":
                    btn.onClick.AddListener(delegate {Action_Upgrade_Academy_Calcel();});
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
                case "ChangeQICToOre":
                    btn.onClick.AddListener(delegate { PlayerManager.Instance.OnClick_Exchange_QuantumIntelligenceCubeToOre(); });
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
            PlayerManager.Instance.ShowMessage("선택된 타일이 없습니다.");
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
        if (PlayerManager.Instance._installBuidingCount[Building.Mine] == 8) return;

        BuildingManager.Instance.InstallMine(clickTile,0);
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
                if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[1])) return;

                //무역 스테이션이 남아있는가?
                if (PlayerManager.Instance._installBuidingCount[Building.TradingStation] == 4) return;

                //광산 설치
                BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[1]);
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                PlayerManager.Instance._installBuidingCount[Building.TradingStation] += 1;
                PlayerManager.Instance._installBuidingCount[Building.Mine] -= 1;
                resourcesManager.ImportResourceAmount_UpDown("Money", 4);
                if (PlayerManager.Instance._installBuidingCount[Building.Mine]!=2)
                    resourcesManager.ImportResourceAmount_UpDown("Ore", -1);

                //플레이어 UI에서 광산 , 무역스테이션 교체
                BuildingManager.Instance.mineImage_UIList[PlayerManager.Instance._installBuidingCount[Building.Mine]].enabled = true;
                BuildingManager.Instance.tradingStation_UIList[PlayerManager.Instance._installBuidingCount[Building.TradingStation]-1].enabled = false;

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
                if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[3])) return;

                //아카데미가 남아있는가?
                if (PlayerManager.Instance._installBuidingCount[Building.Academy] == 2) return;

                //아카데미 UI
                academyUI.SetActive(true);
                break;
            default:
                break;
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
        if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[2])) return;

        //연구소가 더 없으면 지을 수 없다
        if (PlayerManager.Instance._installBuidingCount[Building.ResearchLab] == 3) return;

        BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[2]);
        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
        resourcesManager.ImportResourceAmount_UpDown("Money", -4);
        detailInstallBuildingButtonSetObj.SetActive(false);
        PlayerManager.Instance._installBuidingCount[Building.ResearchLab] += 1;
        PlayerManager.Instance._installBuidingCount[Building.TradingStation] -= 1;
        KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();//기술 타일 획득

        //플레이어 UI에서 무역스테이션, 연구소 교체
        BuildingManager.Instance.tradingStation_UIList[PlayerManager.Instance._installBuidingCount[Building.TradingStation]].enabled = true;
        BuildingManager.Instance.researchLab_UIList[PlayerManager.Instance._installBuidingCount[Building.ResearchLab] - 1].enabled = false;
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
        if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[4])) return;

        //행성 의회는 1개만 지을 수 있음
        if (PlayerManager.Instance._installBuidingCount[Building.PlanetaryInstitute] == 1) return;

        BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[4]);
        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        resourcesManager.ImportResourceAmount_UpDown("Money", -4);

        PlayerManager.Instance._installBuidingCount[Building.PlanetaryInstitute] += 1;
        PlayerManager.Instance._installBuidingCount[Building.TradingStation] -= 1;
        PlayerManager.Instance.AddInstituteBonusPower = 1;//모행성은 파워1 추가
        detailInstallBuildingButtonSetObj.SetActive(false);

        //플레이어 UI에서 무역스테이션, 행성 의회 교체
        BuildingManager.Instance.tradingStation_UIList[PlayerManager.Instance._installBuidingCount[Building.TradingStation]].enabled = true;
        BuildingManager.Instance.institute_UIList[PlayerManager.Instance._installBuidingCount[Building.PlanetaryInstitute] - 1].enabled = false;

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
    /// 무역 스테이션 -> 행성 의회
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_Academy_Calcel()
    {
        academyUI.SetActive(false);
    }

    /// <summary>
    /// 연구 행동 
    /// 지식 트랙 한칸 전진
    /// </summary>
    private void Action_Research()
    {
        //비용 지불이 가능한가?
        if (!resourcesManager.HasEnoughResources("Knowledge", 4)) return;

        //연구행동 표시
        KnowledgeBoard_Manager.Instance.IsPushButton = true;

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
        
        //연방 비용
        int pay = PlayerManager.Instance.ReturnCountSatellite();

        //비용 지불 가능한가?
        if (ResourcesManager.Instance.HasEnoughResources("Energy", pay) == false) return;

        //연방 등록
        //건물있는지 검사하고 없으면 위성을 놓는다.(위성은 개당 에너지 1개)
        PlayerManager.Instance.EnrollUnion();

        //비용 지불, 위성 개수 증가
        for (int i = 0; i < pay; i++)
        {
            BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[5]);
            GameManager.Instance.finalBonusList[2].CountUP();
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
            //정보큐브 버튼 활성화
            if (BuildingManager.Instance.isActiveRightAcademy)
            {
                BuildingManager.Instance.Action_QIC_Get_Button.interactable = true;
            }
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
