using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ActionButton_Set : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;
    // ���� Ÿ�� ����
    public KnowledgeBoard_Manager knowledgeBoardManager;

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

        //�༺�� �����ؾ���
        if (clickTile.PlanetType == Planet.None || clickTile.PlanetType == Planet.Count) return;

        //������ �����ִ°�?
        if (BuildingManager.Instance.mineImage_UIStack.Count == 0) return;
        
        //�߰� ����
        int addSabCost = TileSystem.RequireSabCount(clickTile.PlanetType);

        //��Ÿ� �˻�

        
        //��� �˻�
        if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[0])) return;
        if (resourcesManager.HasEnoughResources("Ore", addSabCost) == false) return;
        if(clickTile.PlanetType == Planet.Gaia)//���̾� �༺�� ���� ť�� 1�� �߰� �ʿ�
        {
            if (resourcesManager.HasEnoughResources("Quantum Intelligence Cube", 1) == false) return;
        }

        //��� ����
        PayForBuilding(BuildingManager.Instance.buildingDataList[0]);
        resourcesManager.ConsumeResource("Ore", addSabCost);
        if (clickTile.PlanetType == Planet.Gaia)//���̾� �༺�� ���� ť�� 1�� �߰� �ʿ�
        {
            resourcesManager.ConsumeResource("Quantum Intelligence Cube", 1);
        }

        //���� ��ġ
        clickTile.ChangeBuildingImageAndPower(Building.Mine);

        //���� ���� ����
        resourcesManager.ImportResourceAmount_UpDown("Ore", 1);

        //���� ����
        if (PlayerManager.Instance.IsGaiaScore)
        {
            PlayerManager.Instance.GetScore(3);
        }

        //�����༺ ���� �߰�
        if (PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] == false)
        {
            PlayerManager.Instance._planetOccupyDic[clickTile.PlanetType] = true;
        }

        //�÷��̾� UI���� ���� ����
        BuildingManager.Instance.last_mineImage = BuildingManager.Instance.mineImage_UIStack.Peek();
        BuildingManager.Instance.last_mineImage.enabled = false;
        BuildingManager.Instance.mineImage_UIStack.Pop();
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

                //���� �����̼��� �����ִ°�?
                if (BuildingManager.Instance.tradingStation_UIStack.Count == 0) return;

                PayForBuilding(BuildingManager.Instance.buildingDataList[1]);
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                resourcesManager.ImportResourceAmount_UpDown("Money", 3);
                resourcesManager.ImportResourceAmount_UpDown("Ore", -1);

                //�÷��̾� UI���� ���� , ���������̼� ��ü
                BuildingManager.Instance.last_tradingStationImage = BuildingManager.Instance.tradingStation_UIStack.Peek();
                BuildingManager.Instance.last_tradingStationImage.enabled = false;
                BuildingManager.Instance.tradingStation_UIStack.Pop();
                BuildingManager.Instance.mineImage_UIStack.Push(BuildingManager.Instance.last_mineImage);
                BuildingManager.Instance.mineImage_UIStack.Peek().enabled = true;
                break;
            case Building.TradingStation://���� �����̼� -> �༺ ��ȸ or ������
                detailInstallBuildingButtonSetObj.SetActive(true);
                break;
            case Building.ResearchLab://������ -> ��ī����
                if (!CanAffordBuilding(BuildingManager.Instance.buildingDataList[3])) return;

                //��ī���̰� �����ִ°�?
                if (BuildingManager.Instance.academy_UIStack.Count == 0) return;

                PayForBuilding(BuildingManager.Instance.buildingDataList[3]);
                clickTile.ChangeBuildingImageAndPower(Building.Academy);
                resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
                KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();//��� Ÿ�� ȹ��

                //�÷��̾� UI���� ������, ��ī���� ��ü
                BuildingManager.Instance.academy_UIStack.Peek().enabled = false;
                BuildingManager.Instance.academy_UIStack.Pop();
                BuildingManager.Instance.researchLab_UIStack.Push(BuildingManager.Instance.researchLabImage);
                BuildingManager.Instance.researchLab_UIStack.Peek().enabled = true;
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

        //���� �߰� ���
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

        //���� �߰� ���
        if (buildingData.type == Building.Mine)
        {
            resourcesManager.ConsumeResource("Ore", 0);
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

        //�����Ұ� �� ������ ���� �� ����
        if (BuildingManager.Instance.researchLab_UIStack.Count == 0) return;

        PayForBuilding(BuildingManager.Instance.buildingDataList[2]);
        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
        detailInstallBuildingButtonSetObj.SetActive(false);
        KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();//��� Ÿ�� ȹ��

        //�÷��̾� UI���� ���������̼�, ������ ��ü
        BuildingManager.Instance.researchLabImage = BuildingManager.Instance.researchLab_UIStack.Peek();
        BuildingManager.Instance.researchLabImage.enabled = false;
        BuildingManager.Instance.researchLab_UIStack.Pop();
        BuildingManager.Instance.tradingStation_UIStack.Push(BuildingManager.Instance.last_tradingStationImage);
        BuildingManager.Instance.tradingStation_UIStack.Peek().enabled = true;
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

        //�༺ ��ȸ�� 1���� ���� �� ����
        if (BuildingManager.Instance.institute_UIStack.Count == 0) return;

        PayForBuilding(BuildingManager.Instance.buildingDataList[4]);
        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        resourcesManager.ImportResourceAmount_UpDown("Money", -3);
        detailInstallBuildingButtonSetObj.SetActive(false);

        //�÷��̾� UI���� ���������̼�, �༺ ��ȸ ��ü
        BuildingManager.Instance.institute_UIStack.Peek().enabled = false;
        BuildingManager.Instance.institute_UIStack.Pop();
        BuildingManager.Instance.tradingStation_UIStack.Push(BuildingManager.Instance.last_tradingStationImage);
        BuildingManager.Instance.tradingStation_UIStack.Peek().enabled = true;
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
        //��� ������ �����Ѱ�?
        if (!resourcesManager.HasEnoughResources("Knowledge", 4)) return;

        //� ����Ÿ���� ������ ���ΰ�?
        knowledgeBoardManager.ActivateKnowledgeTile(ResearchType.Count);
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
        int pay = PlayerManager.Instance.EnrollUnion_ReturnCountSatellite();
        //��� ����
        for (int i = 0; i < pay; i++)
        {
            PayForBuilding(BuildingManager.Instance.buildingDataList[5]);
        }
        //���� ��ū ��������
        KnowledgeBoard_Manager.Instance.Activate_AllUnionTile_UI();
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
        TileManager.Instance.ActivateAllTiles();//Ÿ�� Ȱ��ȭ
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
