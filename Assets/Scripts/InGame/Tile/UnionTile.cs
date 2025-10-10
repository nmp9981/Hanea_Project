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
    [SerializeField] private TextMeshProUGUI _restCountText;//���� ����
    private int _restCount = 2;//ó������ 2�� ����
    
    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public int RestCount { get; set; }
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
            //���� ǥ��
            _restCountText.text = $"X {_restCount}";
        }
        else
        {
            _isGet = true;
            _restCountText.text = string.Empty;
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
        KnowledgeBoard_Manager.Instance.UnActivate_AllUnionTile();
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
            _restCount = Mathf.Max(0, _restCount - 1);
            _restCountText.text = $"X {_restCount}";

            GameObject tileGm = Instantiate(this.gameObject);
            tileGm.transform.parent = unionTileArea;
            KnowledgeBoard_Manager.Instance._getUnionTileList.Add(tileGm.GetComponent<UnionTile>());
        }
    }
}
