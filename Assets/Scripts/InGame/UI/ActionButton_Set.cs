using UnityEngine;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    //건물 업그레이드 UI(무역 스테이션)
    public GameObject detailInstallBuildingButtonSetObj;

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
                case "Union":
                    break;
                case "Pass":
                    break;
                case "Knowledge UP":
                    break;
                case "ResearchLab":
                    btn.onClick.AddListener(delegate { Action_Upgrade_ResearchLab(); });
                    break;
                case "PlanetaryInstitute":
                    btn.onClick.AddListener(delegate { Action_Upgrade_PlanetaryInstitute(); });
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
      
        //광산 설치
        clickTile.ChangeBuildingImageAndPower(Building.Mine);
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
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                resourcesManager.ImportResourceAmount_UpDown("Money", 3);
                break;
            case Building.TradingStation://무역 스테이션 -> 행성 의회 or 연구소
                detailInstallBuildingButtonSetObj.SetActive(true);
                break;
            case Building.ResearchLab://연구소 -> 아카데미
                clickTile.ChangeBuildingImageAndPower(Building.Academy);
                resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
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

        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
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

        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        detailInstallBuildingButtonSetObj.SetActive(false);
    }
}
