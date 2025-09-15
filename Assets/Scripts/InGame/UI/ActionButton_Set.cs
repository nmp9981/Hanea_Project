using UnityEngine;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    //�ǹ� ���׷��̵� UI(���� �����̼�)
    public GameObject detailInstallBuildingButtonSetObj;

    private void Awake()
    {
        BindingActionButtons();
    }

    /// <summary>
    /// �׼� ��ư ���ε�
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
    /// Ŭ���� Ÿ�� ��������
    /// </summary>
    /// <returns></returns>
    private Tile GetClickedTile()
    {
        Tile clickedTile = PlayerManager.Instance.ClickedTile();
        if (clickedTile == null)
        {
            Debug.LogWarning("Ŭ���� Ÿ���� �����ϴ�.");
        }
        return clickedTile;
    }

    /// <summary>
    /// ���� ��ġ
    /// </summary>
    /// <param name="cilckTile"></param>
    public void Action_InstallMine()
    {
        //Ŭ���� Ÿ�� ��������
        Tile clickTile = GetClickedTile();
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
        switch (clickTile.InstallBuilding)
        {
            case Building.Mine://���� -> ���� �����̼�
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                resourcesManager.ImportResourceAmount_UpDown("Money", 3);
                break;
            case Building.TradingStation://���� �����̼� -> �༺ ��ȸ or ������
                detailInstallBuildingButtonSetObj.SetActive(true);
                break;
            case Building.ResearchLab://������ -> ��ī����
                clickTile.ChangeBuildingImageAndPower(Building.Academy);
                resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ���� �����̼� -> ������
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_ResearchLab()
    {
        //Ŭ���� Ÿ�� ��������
        Tile clickTile = GetClickedTile();
        //Ŭ���� Ÿ���� �����ؾ���
        if (clickTile == null) return;

        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
        detailInstallBuildingButtonSetObj.SetActive(false);
    }

    /// <summary>
    /// ���� �����̼� -> �༺ ��ȸ
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_PlanetaryInstitute()
    {
        //Ŭ���� Ÿ�� ��������
        Tile clickTile = GetClickedTile();
        //Ŭ���� Ÿ���� �����ؾ���
        if (clickTile == null) return;

        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        detailInstallBuildingButtonSetObj.SetActive(false);
    }
}
