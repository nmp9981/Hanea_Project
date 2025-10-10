using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnionTile : MonoBehaviour, TileInterface
{
    // 타일 습득 시 호출될 이벤트
    public event System.Action OnChanged;

    //속성 정의
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet = false;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _restCountText;//남은 개수
    private int _restCount = 2;//처음에는 2개 남음
    
    //읽기 전용
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public int RestCount { get; set; }
    public Button Button
    {
        // 읽기 (Get): 필드에 값이 없으면(null이면) GetComponent를 호출하여 저장한 후 반환.
        // 이는 매번 GetComponent를 호출하는 것을 방지합니다.
        get
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
            return _button;
        }

        // 쓰기 (Set): 외부에서 새로운 Button 컴포넌트를 할당할 수 있도록 허용.
        set
        {
            _button = value;
        }
    }

    //연방타일 표시 영역
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
    /// 타일 초기화
    /// </summary>
    public void InitTile()
    {
        _button = this.Button;
        _button.interactable = false;
        _button.onClick.AddListener(GetReward);

        //플레이어가 획득한 연방영역이 아닐때
        if(this.transform.parent != unionTileArea)
        {
            _isGet = false;
            //개수 표시
            _restCountText.text = $"X {_restCount}";
        }
        else
        {
            _isGet = true;
            _restCountText.text = string.Empty;
        }
    }

    /// <summary>
    /// 타일 활성화
    /// </summary>
    public void TileActive()
    {
        _button.interactable = true;
    }
    /// <summary>
    /// 타일 비활성화
    /// </summary>
    public void TileUnActive()
    {
        _button.interactable = false;
    }

    /// <summary>
    /// 보상 : 획득시 한번 적용
    /// </summary>
    public void GetReward()
    {
        //이미 획득한것은 획득 불가 
        if (_isGet) return;
        //각 유형에 따른 보상 획득
        if (_rewardResourceList.Count == 0) return;

        foreach (var reward in _rewardResourceList)
        {
            switch (reward.RewardResourcesType)
            {
                case RewardResourcesType.SingleUse://자원 획득
                    ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.Score://점수 획득
                    PlayerManager.Instance.GetScore(reward.RewardAmount);
                    break;
                default:
                    break;
            }
        }

        //획득 표시
        ShowGetTile();

        //값 변경
        if (_isGet == true)
        {
            OnChanged?.Invoke();
        }
        KnowledgeBoard_Manager.Instance.UnActivate_AllUnionTile();
    }

    /// <summary>
    /// 타일 획득 표시
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
