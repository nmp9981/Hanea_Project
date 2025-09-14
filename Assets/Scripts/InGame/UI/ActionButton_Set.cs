using UnityEngine;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    private void Awake()
    {
        BindingActionButtons();
    }

    /// <summary>
    /// 액션 버튼 바인딩
    /// </summary>
    void BindingActionButtons()
    {
        foreach (Button btn in gameObject.GetComponentsInChildren<Button>())
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
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 광산 설치
    /// </summary>
    /// <param name="cilckTile"></param>
    public void Action_InstallMine()
    {
        //클릭한 타일 가져오기
        Tile clickTile = PlayerManager.Instance.ClickedTile();
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
        if(clickTile.TilePower == 1)//교역소 업그레이드
        {
            clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
            resourcesManager.ImportResourceAmount_UpDown("Money",3);
        }
        else if (clickTile.TilePower == 2)
        {
            clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        }
    }
}
