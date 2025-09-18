using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    //건물 업그레이드 UI(무역 스테이션)
    public GameObject detailInstallBuildingButtonSetObj;
    //패스 UI
    public GameObject passUIObj;

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

        //비용 검사
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[0])) return;

        //비용 지불
        PayForBuilding(BuildingManager.Instance.buildingDataList[0]);

        //광산 설치
        clickTile.ChangeBuildingImageAndPower(Building.Mine);

        //광석 수입 증가
        resourcesManager.ImportResourceAmount_UpDown("Ore", 1);
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
                PayForBuilding(BuildingManager.Instance.buildingDataList[1]);
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                resourcesManager.ImportResourceAmount_UpDown("Money", 3);
                resourcesManager.ImportResourceAmount_UpDown("Ore", -1);
                break;
            case Building.TradingStation://무역 스테이션 -> 행성 의회 or 연구소
                detailInstallBuildingButtonSetObj.SetActive(true);
                break;
            case Building.ResearchLab://연구소 -> 아카데미
                if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[3])) return;
                PayForBuilding(BuildingManager.Instance.buildingDataList[3]);
                clickTile.ChangeBuildingImageAndPower(Building.Academy);
                resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
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
        
        PayForBuilding(BuildingManager.Instance.buildingDataList[2]);
        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
        detailInstallBuildingButtonSetObj.SetActive(false);
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

        PayForBuilding(BuildingManager.Instance.buildingDataList[4]);
        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
        detailInstallBuildingButtonSetObj.SetActive(false);
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
        PlayerManager.Instance.EnrollUnion();
        
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
        resourcesManager.ImportAllResources();//수입
        passUIObj.SetActive(false);
    }
    /// <summary>
    /// 패스 취소
    /// </summary>
    private void Action_Pass_Cancel()
    {
        passUIObj.SetActive(false);
    }
    #endregion
}
