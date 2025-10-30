using UnityEngine;
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
    //���� �׼� UI
    public GameObject freeActionUI;
    //��ī���� UI
    public GameObject academyUI;

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
                case "FreeAction"://���� �׼� �ൿ
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
    /// Ŭ���� Ÿ�� ��������
    /// </summary>
    /// <returns></returns>
    private Tile GetClickedTile()
    {
        Tile clickedTile = PlayerManager.Instance.ClickedTile();
        if (clickedTile == null)
        {
            PlayerManager.Instance.ShowMessage("���õ� Ÿ���� �����ϴ�.");
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
        if (PlayerManager.Instance._installBuidingCount[Building.Mine] == 8) return;

        BuildingManager.Instance.InstallMine(clickTile,0);
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
                if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[1])) return;

                //���� �����̼��� �����ִ°�?
                if (PlayerManager.Instance._installBuidingCount[Building.TradingStation] == 4) return;

                //���� ��ġ
                BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[1]);
                clickTile.ChangeBuildingImageAndPower(Building.TradingStation);
                PlayerManager.Instance._installBuidingCount[Building.TradingStation] += 1;
                PlayerManager.Instance._installBuidingCount[Building.Mine] -= 1;
                resourcesManager.ImportResourceAmount_UpDown("Money", 4);
                if (PlayerManager.Instance._installBuidingCount[Building.Mine]!=2)
                    resourcesManager.ImportResourceAmount_UpDown("Ore", -1);

                //�÷��̾� UI���� ���� , ���������̼� ��ü
                BuildingManager.Instance.mineImage_UIList[PlayerManager.Instance._installBuidingCount[Building.Mine]].enabled = true;
                BuildingManager.Instance.tradingStation_UIList[PlayerManager.Instance._installBuidingCount[Building.TradingStation]-1].enabled = false;

                //���� ���ʽ� ����
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
            case Building.TradingStation://���� �����̼� -> �༺ ��ȸ or ������
                detailInstallBuildingButtonSetObj.SetActive(true);
                break;
            case Building.ResearchLab://������ -> ��ī����
                if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[3])) return;

                //��ī���̰� �����ִ°�?
                if (PlayerManager.Instance._installBuidingCount[Building.Academy] == 2) return;

                //��ī���� UI
                academyUI.SetActive(true);
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

        //��� ������ �Ǵ°�?
        if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[2])) return;

        //�����Ұ� �� ������ ���� �� ����
        if (PlayerManager.Instance._installBuidingCount[Building.ResearchLab] == 3) return;

        BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[2]);
        clickTile.ChangeBuildingImageAndPower(Building.ResearchLab);
        resourcesManager.ImportResourceAmount_UpDown("Knowledge", 1);
        resourcesManager.ImportResourceAmount_UpDown("Money", -4);
        detailInstallBuildingButtonSetObj.SetActive(false);
        PlayerManager.Instance._installBuidingCount[Building.ResearchLab] += 1;
        PlayerManager.Instance._installBuidingCount[Building.TradingStation] -= 1;
        KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();//��� Ÿ�� ȹ��

        //�÷��̾� UI���� ���������̼�, ������ ��ü
        BuildingManager.Instance.tradingStation_UIList[PlayerManager.Instance._installBuidingCount[Building.TradingStation]].enabled = true;
        BuildingManager.Instance.researchLab_UIList[PlayerManager.Instance._installBuidingCount[Building.ResearchLab] - 1].enabled = false;
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
        if (!BuildingManager.Instance.CanAffordBuilding(BuildingManager.Instance.buildingDataList[4])) return;

        //�༺ ��ȸ�� 1���� ���� �� ����
        if (PlayerManager.Instance._installBuidingCount[Building.PlanetaryInstitute] == 1) return;

        BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[4]);
        clickTile.ChangeBuildingImageAndPower(Building.PlanetaryInstitute);
        resourcesManager.ImportResourceAmount_UpDown("Energy", 5);
        resourcesManager.ImportResourceAmount_UpDown("Money", -4);

        PlayerManager.Instance._installBuidingCount[Building.PlanetaryInstitute] += 1;
        PlayerManager.Instance._installBuidingCount[Building.TradingStation] -= 1;
        PlayerManager.Instance.AddInstituteBonusPower = 1;//���༺�� �Ŀ�1 �߰�
        detailInstallBuildingButtonSetObj.SetActive(false);

        //�÷��̾� UI���� ���������̼�, �༺ ��ȸ ��ü
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
    /// ���� �����̼� -> �༺ ��ȸ
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_Cancel()
    {
        detailInstallBuildingButtonSetObj.SetActive(false);
    }
    /// <summary>
    /// ���� �����̼� -> �༺ ��ȸ
    /// </summary>
    /// <param name="clickTile"></param>
    private void Action_Upgrade_Academy_Calcel()
    {
        academyUI.SetActive(false);
    }

    /// <summary>
    /// ���� �ൿ 
    /// ���� Ʈ�� ��ĭ ����
    /// </summary>
    private void Action_Research()
    {
        //��� ������ �����Ѱ�?
        if (!resourcesManager.HasEnoughResources("Knowledge", 4)) return;

        //�����ൿ ǥ��
        KnowledgeBoard_Manager.Instance.IsPushButton = true;

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
        int pay = PlayerManager.Instance.ReturnCountSatellite();

        //��� ���� �����Ѱ�?
        if (ResourcesManager.Instance.HasEnoughResources("Energy", pay) == false) return;

        //���� ���
        //�ǹ��ִ��� �˻��ϰ� ������ ������ ���´�.(������ ���� ������ 1��)
        PlayerManager.Instance.EnrollUnion();

        //��� ����, ���� ���� ����
        for (int i = 0; i < pay; i++)
        {
            BuildingManager.Instance.PayForBuilding(BuildingManager.Instance.buildingDataList[5]);
            GameManager.Instance.finalBonusList[2].CountUP();
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
        passUIObj.SetActive(false);
        if (GameManager.Instance.currentRound != 6)//������ ���尡 �ƴҶ���
        {
            resourcesManager.ImportAllResources();//����
            TileManager.Instance.ActivateAllTiles();//Ÿ�� Ȱ��ȭ
            //����ť�� ��ư Ȱ��ȭ
            if (BuildingManager.Instance.isActiveRightAcademy)
            {
                BuildingManager.Instance.Action_QIC_Get_Button.interactable = true;
            }
        }
        GameManager.Instance.ShowRoundText();//���� ����
    }
    /// <summary>
    /// �н� ���
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
