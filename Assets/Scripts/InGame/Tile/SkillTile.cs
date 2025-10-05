using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class SkillTile : MonoBehaviour, TileInterface
{
    // Ÿ�� ���� �� ȣ��� �̺�Ʈ
    public event System.Action OnChanged;

    //�Ӽ� ����
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet = false;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();
    [SerializeField] private Button _button;
    [SerializeField] private ResearchType _researchTypeArea;

    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public Button Button => GetComponent<Button>();
    
    //Ÿ���� �ִ� ����
    public ResearchType ResearchTypeArea { get; set; }
    //���Ÿ�� ǥ�� ����
    [SerializeField]
    Transform skillTileArea;

    void Awake()
    {
        skillTileArea = GameObject.Find("OwnSkillTileArea").GetComponent<Transform>();
    }

    void OnEnable()
    {
        InitTile();
        TileActive();
    }

    /// <summary>
    /// Ÿ�� �ʱ�ȭ
    /// </summary>
    public void InitTile()
    {
        _button = this.Button;
        _button.onClick.AddListener(GetReward);
    }

    /// <summary>
    /// Ÿ�� Ȱ��ȭ
    /// </summary>
    public void TileActive()
    {
        _isGet = false;
        _button.interactable = false;//��ư Ȱ��ȭ
    }

    /// <summary>
    /// ���� : ȹ��� �ѹ� ����
    /// </summary>
    public void GetReward()
    {
        //�̹� ȹ���Ѱ��� ȹ�� �Ұ� 
        if (_isGet) return;
        //�� ������ ���� ���� ȹ��
        if (_rewardResourceList.Count == 0) return;
        foreach (var reward in _rewardResourceList)
        {
            switch (reward.RewardResourcesType)
            {
                case RewardResourcesType.Import://���� ����
                    ResourcesManager.Instance.ImportResourceAmount_UpDown(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.SingleUse://�ڿ� ȹ��
                    if(reward.ResourceName == "Knowledge")
                    {
                        int addKnowledge = TileSystem.CountOccupyPlanet();
                        ResourcesManager.Instance.GainResource(reward.ResourceName,  addKnowledge);
                    }
                    else ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.Score://���� ȹ��
                    if(reward.ResourceName == "Gaia")//���̾� �༺
                    {
                        PlayerManager.Instance.IsGaiaScore = true;
                    }
                    else//7��
                    {
                        PlayerManager.Instance.GetScore(reward.RewardAmount);
                    }
                    break;
                case RewardResourcesType.Etc://��Ÿ ȿ��
                    PowerValueUp_Academy_Institute();
                    break;
                default:
                    break;
            }
        }

        //ȹ�� ǥ��
        ShowGetTile();

        //���� Ÿ�� �ø���
        if(ResearchTypeArea == ResearchType.Count)
        {
            KnowledgeBoard_Manager.Instance.ActivateKnowledgeTile(ResearchType.Count);
        }else KnowledgeBoard_Manager.Instance.ActivateKnowledgeTile(ResearchTypeArea);

        //�� ����
        if (_isGet == true)
        {
            OnChanged?.Invoke();
        }
        //�ٽ� ��Ȱ��ȭ
        KnowledgeBoard_Manager.Instance.UnActivate_AllSkillTile();
    }

    /// <summary>
    /// Ÿ�� ȹ�� ǥ��
    /// </summary>
    void ShowGetTile()
    {
        _isGet = true;
        _button.interactable = false;

        GameObject tileGm = Instantiate(this.gameObject);
        tileGm.transform.parent = skillTileArea;
    }

    /// <summary>
    /// ��ȸ, ��ī���� �Ŀ��� ����
    /// </summary>
    void PowerValueUp_Academy_Institute()
    {
        BuildingManager.Instance.buildingDataList[3].powerValue = 4;
        BuildingManager.Instance.buildingDataList[4].powerValue = 4;
    }
    
}
