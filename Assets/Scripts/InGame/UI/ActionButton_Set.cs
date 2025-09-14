using UnityEngine;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    private void Awake()
    {
        BindingActionButtons();
    }

    /// <summary>
    /// �׼� ��ư ���ε�
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
    /// ���� ��ġ
    /// </summary>
    /// <param name="cilckTile"></param>
    public void Action_InstallMine()
    {
        //Ŭ���� Ÿ�� ��������
        Tile clickTile = PlayerManager.Instance.ClickedTile();
        //Ŭ���� Ÿ���� �����ؾ���
        if (clickTile == null) return;

        //��ġ�� �ǹ��� �������
        if (clickTile.InstallBuildingImage.sprite != null) return;
      
        //���� ��ġ
        clickTile.ChangeBuildingImageAndPower(Building.Mine);
    }

    /// <summary>
    /// �ǹ� ���׷��̵�
    /// </summary>
    public void Action_BuildingUpgrade()
    {
        //Ŭ���� Ÿ�� ��������
        Tile clickTile = PlayerManager.Instance.ClickedTile();
        //Ŭ���� Ÿ���� �����ؾ���
        if (clickTile == null) return;

        //��ġ�� �ǹ��� �־����
        if (clickTile.InstallBuildingImage.sprite == null) return;

        //�Ŀ� 3�̻��̸� ���׷��̵� �Ұ�(�༺ ��ȸ, ��ī����)
        if (clickTile.TilePower >= 3) return;

        //�ǹ� ������ ���� ���׷��̵尡 �ٸ�
        if(clickTile.TilePower == 1)//������ ���׷��̵�
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
