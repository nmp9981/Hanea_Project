using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    //�ǹ� ���׷��̵� UI(���� �����̼�)
    public GameObject detailInstallBuildingButtonSetObj;
    //�н� UI
    public GameObject passUIObj;

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
                case "Union"://���� ����
                    btn.onClick.AddListener(delegate { Action_Union(); });
                    break;
                case "Pass"://�н�
                    btn.onClick.AddListener(delegate { Action_Pass_Question(); });
                    break;
                case "Research"://���� �ൿ
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

        //��� �˻�
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[0])) return;

        //��� ����
        PayForBuilding(BuildingManager.Instance.buildingDataList[0]);

        //���� ��ġ
        clickTile.ChangeBuildingImageAndPower(Building.Mine);

        //���� ���� ����
        resourcesManager.ImportResourceAmount_UpDown("Ore", 1);
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
                if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[1])) return;
                PayForBuilding(BuildingManager.Instance.buildingDataList[1]);
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                resourcesManager.ImportResourceAmount_UpDown("Money", 3);
                resourcesManager.ImportResourceAmount_UpDown("Ore", -1);
                break;
            case Building.TradingStation://���� �����̼� -> �༺ ��ȸ or ������
                detailInstallBuildingButtonSetObj.SetActive(true);
                break;
            case Building.ResearchLab://������ -> ��ī����
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
    /// �ǹ� ��ġ ��� ���� ���� ���� üũ
    /// </summary>
    /// <param name="buildingData">Ȯ���� �ǹ� ������</param>
    /// <returns>��� ���� ���� ����</returns>
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
    /// �ǹ� ��ġ ��� ����
    /// </summary>
    /// <param name="buildingData">����� ������ �ǹ� ������</param>
    private void PayForBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return;

        foreach (var cost in buildingData.costs)
        {
            resourcesManager.ConsumeResource(cost.resourceName, cost.amount);
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

        //��� ������ �Ǵ°�?
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[2])) return;
        
        PayForBuilding(BuildingManager.Instance.buildingDataList[2]);
        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
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

        //��� ������ �Ǵ°�?
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[4])) return;

        PayForBuilding(BuildingManager.Instance.buildingDataList[4]);
        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
        detailInstallBuildingButtonSetObj.SetActive(false);
    }
    /// <summary>
    /// ���� �����̼� -> �༺ ��ȸ
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_Cancel()
    {
        detailInstallBuildingButtonSetObj.SetActive(false);
    }

    /// <summary>
    /// ���� �ൿ 
    /// ���� Ʈ�� ��ĭ ����
    /// </summary>
    private void Action_Research()
    {

    }

    /// <summary>
    /// ���� �׼�
    /// </summary>
    private void Action_Union()
    {
        //���� �Ұ�
        if (!PlayerManager.Instance.Check_AbleUnion(PlayerManager.Instance.ClickedTileList())) return;
        
        //���� ���
        //�ǹ��ִ��� �˻��ϰ� ������ ������ ���´�.(������ ���� ������ 1��)
        PlayerManager.Instance.EnrollUnion();
        
    }

    #region Pass
    /// <summary>
    /// �н� ����
    /// </summary>
    private void Action_Pass_Question()
    {
        //UI�� ����.
        passUIObj.SetActive(true);
    }
    /// <summary>
    /// �н�
    /// </summary>
    private void Action_Pass_OK()
    {
        resourcesManager.ImportAllResources();//����
        passUIObj.SetActive(false);
    }
    /// <summary>
    /// �н� ���
    /// </summary>
    private void Action_Pass_Cancel()
    {
        passUIObj.SetActive(false);
    }
    #endregion
}
