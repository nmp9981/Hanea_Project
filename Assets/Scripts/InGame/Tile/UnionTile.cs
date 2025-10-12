using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnionTile : MonoBehaviour, TileInterface
{
    // Ÿ�� ���� �� ȣ��� �̺�Ʈ
    public event System.Action OnChanged;

    //�Ӽ� ����
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet = false;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();
    [SerializeField] private Button _button;
    [SerializeField] private UnionRewardResourcesType _unionRewardResourcesType;
    
    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public int RestCount { get; set; }
    public UnionRewardResourcesType UnionRewardResourcesType {
        get { return _unionRewardResourcesType; }
        set { _unionRewardResourcesType = value; }
    }
    public Button Button
    {
        // �б� (Get): �ʵ忡 ���� ������(null�̸�) GetComponent�� ȣ���Ͽ� ������ �� ��ȯ.
        // �̴� �Ź� GetComponent�� ȣ���ϴ� ���� �����մϴ�.
        get
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
            return _button;
        }

        // ���� (Set): �ܺο��� ���ο� Button ������Ʈ�� �Ҵ��� �� �ֵ��� ���.
        set
        {
            _button = value;
        }
    }

    //����Ÿ�� ǥ�� ����
    [SerializeField]
    Transform unionTileArea;

    void Awake()
    {
        unionTileArea = GameObject.Find("UnionArea").GetComponent<Transform>();
    }

    void OnEnable()
    {
        InitTile();
    }

    /// <summary>
    /// Ÿ�� �ʱ�ȭ
    /// </summary>
    public void InitTile()
    {
        _button = this.Button;
        _button.interactable = false;
        _button.onClick.AddListener(GetReward);

        //�÷��̾ ȹ���� ���濵���� �ƴҶ�
        if(this.transform.parent != unionTileArea)
        {
            _isGet = false;
        }
        else
        {
            _isGet = true;
        }
    }

    /// <summary>
    /// Ÿ�� Ȱ��ȭ
    /// </summary>
    public void TileActive()
    {
        _button.interactable = true;
    }
    /// <summary>
    /// Ÿ�� ��Ȱ��ȭ
    /// </summary>
    public void TileUnActive()
    {
        _button.interactable = false;
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
                case RewardResourcesType.SingleUse://�ڿ� ȹ��
                    ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.Score://���� ȹ��
                    PlayerManager.Instance.GetScore(reward.RewardAmount);
                    break;
                default:
                    break;
            }
        }

        //ȹ�� ǥ��
        ShowGetTile();

        //�� ����
        if (_isGet == true)
        {
            OnChanged?.Invoke();
        }

        //���� ���ʽ�
        if (GameManager.Instance.IsRoundEffectDic[RoundEffect.Union] == true)
        {
            PlayerManager.Instance.GetScore(5);
        }
        KnowledgeBoard_Manager.Instance.UnActivate_AllUnionTile();
        KnowledgeBoard_Manager.Instance.UnActivate_AllUnionTile_UI();
    }

    /// <summary>
    /// Ÿ�� ȹ�� ǥ��
    /// </summary>
    void ShowGetTile()
    {
        _isGet = true;
        _button.interactable = false;

        if (this.transform.parent != unionTileArea)
        {
            KnowledgeBoard_Manager.Instance.Change_RestUnionTileCount(this.UnionRewardResourcesType);

            GameObject tileGm = Instantiate(this.gameObject);
            tileGm.transform.parent = unionTileArea;
            UnionTile union = tileGm.GetComponent<UnionTile>();
            union.UnionRewardResourcesType = this.UnionRewardResourcesType;
            KnowledgeBoard_Manager.Instance._getUnionTileList.Add(union);
        }
    }
}
