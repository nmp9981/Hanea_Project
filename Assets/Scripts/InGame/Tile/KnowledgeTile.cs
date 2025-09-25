using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ÿ�� Ÿ��
/// </summary>
public enum TileType
{
    Knowledge,
    Skill,
    Energy,
    Information,
    Count
}


/// <summary>
/// Ÿ�� �������̽� ����
/// </summary>
public interface TileInterface
{
    //����
    TileType Type { get; }//Ÿ�� ����

    bool IsGet { get; }//���� ����

    RewardResource RewardResources { get; }//�ڿ� ����

    //����
    void GetReward();
}

public class RewardResource
{
    public RewardResourcesType RewardResourcesType;//�ڿ� ���� ����
    public string ResourceName;//���� �ڿ���
    public int RewardAmount;//����
}

public class KnowledgeTile : MonoBehaviour, TileInterface
{
    // Ÿ�� ���� �� ȣ��� �̺�Ʈ
    public event System.Action OnChanged;

    //�Ӽ� ����
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet;
    [SerializeField] private RewardResource _rewardResource;

    [SerializeField] private int _level;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _button;
    [SerializeField] private TileData _tileData;

    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public RewardResource RewardResources => _rewardResource;
    public int Level => _level;
    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Button Button => GetComponent<Button>();
    public TileData TileData => _tileData;

    // �������̽� ������ ���� Dictionary
    public Dictionary<Resource, int> Costs { get; private set; }

    void Awake()
    {
        //LoadCostsFromData();
    }

    /// <summary>
    /// Ÿ�� �ʱ�ȭ
    /// </summary>
    public void InitTile()
    {
        _isGet = false;

        if(_level==0) _isGet = true;

        _rectTransform = this.RectTransform;
        _button = this.Button;
        _button.interactable = false;//��ư ��Ȱ��ȭ

        _button.onClick.AddListener(ClickKnowLedgeTile);
    }

    /// <summary>
    /// Ÿ�� �ڿ� ������ �ε�
    /// </summary>
    private void LoadCostsFromData()
    {
        Costs = new Dictionary<Resource, int>();
        foreach (var costAmount in _tileData.costs)
        {
            Costs[costAmount.type] = costAmount.amount;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public void GetReward()
    {
        //2->3 �Ŀ� ����
        if (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] == 3)
        {
            ResourcesManager.Instance.GainResource("Energy", 3);
        }

        //�� ������ ���� ���� ȹ��
        switch (_rewardResource.RewardResourcesType)
        {
            case RewardResourcesType.Import://���� ����
                ResourcesManager.Instance.ImportResourceAmount_UpDown(_rewardResource.ResourceName, _rewardResource.RewardAmount);
                break;
            case RewardResourcesType.SingleUse://�ڿ� ȹ��
                ResourcesManager.Instance.GainResource(_rewardResource.ResourceName, _rewardResource.RewardAmount);
                break;
            case RewardResourcesType.Etc://��Ÿ ȿ��
                break;
            default:
                break;
        }


        switch (_tileData.researchType)
        {
            case ResearchType.Terraforming:
                Debug.Log("���� ����");
                break;
            case ResearchType.Navigation:
                Debug.Log("���Ÿ� ����");
                break;
            case ResearchType.AI:
                Debug.Log("���� ����");
                break;
            case ResearchType.Economy:
                break;
            case ResearchType.Science:
                break;
            default:
                break;
        }


        //�� ����
        if (_isGet==true)
        {
            OnChanged?.Invoke();
        }
    }
    /// <summary>
    /// ���� Ÿ���� ����
    /// </summary>
    public void ClickKnowLedgeTile()
    {
        //���� 4�Ҹ�
        ResourcesManager.Instance.ConsumeResource("Knowledge", 4);

        //���� Ʈ�� �̵�
        KnowledgeBoard_Manager.Instance.stateObjDic[this.TileData.researchType].GetComponent<RectTransform>().position 
            = this._rectTransform.position+ this.RectTransform.sizeDelta.y * Vector3.up;
        
        //�÷��̾��� ���� ���� ��ȭ
        KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] += 1;

        //���� �ڿ� ȹ��
        GetReward();
        
        //2�� ȹ��
        PlayerManager.Instance.GetScore(2);
        //�ٽ� �������
        KnowledgeBoard_Manager.Instance.UnActivateKnowledgeTile();
    }
}
