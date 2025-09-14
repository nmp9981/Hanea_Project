using UnityEngine;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
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
}
