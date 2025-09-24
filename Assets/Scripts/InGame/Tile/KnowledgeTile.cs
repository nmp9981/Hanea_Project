using Mono.Cecil;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타일 타입
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
/// 타일 인터페이스 정의
/// </summary>
public interface TileInterface
{
    //유형
    TileType Type { get; }//타일 유형

    bool IsGet { get; }//습득 여부

    //보상
    void Reward();
}
public class KnowledgeTile : MonoBehaviour, TileInterface
{
    // 타일 습득 시 호출될 이벤트
    public event System.Action OnChanged;

    //속성 정의
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet;
    [SerializeField] private int _level;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _button;
    [SerializeField] private TileData _tileData;

    //읽기 전용
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public int Level => _level;
    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Button Button => GetComponent<Button>();
    public TileData TileData => _tileData;

    // 인터페이스 구현을 위한 Dictionary
    public Dictionary<Resource, int> Costs { get; private set; }

    void Awake()
    {
        InitTile();
        //LoadCostsFromData();
    }

    /// <summary>
    /// 타일 초기화
    /// </summary>
    public void InitTile()
    {
        _isGet = false;

        if(_level==0) _isGet = true;

        _rectTransform = this.RectTransform;
        _button = this.Button;
        _button.interactable = false;//버튼 비활성화

        _button.onClick.AddListener(ClickKnowLedgeTile);
    }

    /// <summary>
    /// 타일 자원 데이터 로드
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
    /// 보상
    /// </summary>
    public void Reward()
    {
        switch (_tileData.researchType)
        {
            case ResearchType.Terraforming:
                Debug.Log("삽비용 할인");
                break;
            case ResearchType.Navigation:
                Debug.Log("허용거리 증가");
                break;
            case ResearchType.AI:
                Debug.Log("삽비용 할인");
                break;
            case ResearchType.Economy:
                break;
            case ResearchType.Science:
                break;
            default:
                break;
        }


        //값 변경
        if (_isGet==true)
        {
            OnChanged?.Invoke();
        }
    }
    /// <summary>
    /// 지식 타일을 누름
    /// </summary>
    public void ClickKnowLedgeTile()
    {
        //지식 4소모
        if (ResourcesManager.Instance.HasEnoughResources("Knowledge", 4))
            ResourcesManager.Instance.ConsumeResource("Knowledge", 4);

        //연구 트랙 이동
        KnowledgeBoard_Manager.Instance.stateObjDic[this.TileData.researchType].GetComponent<RectTransform>().position 
            += this.RectTransform.sizeDelta.y*Vector3.up*0.5f;

        //플레이어의 지식 레벨 변화
        KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] += 1;
        //2점 획득
        PlayerManager.Instance.GetScore(2);
        //다시 원래대로
        KnowledgeBoard_Manager.Instance.UnActivateKnowledgeTile();
    }
}
