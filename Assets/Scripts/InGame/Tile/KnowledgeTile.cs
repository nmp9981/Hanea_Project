using System.Collections.Generic;
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

    List<RewardResource> RewardResourcesList { get; }//자원 보상

    //보상
    void GetReward();
}

[System.Serializable]
public class RewardResource
{
    public RewardResourcesType RewardResourcesType;//자원 습득 유형
    public string ResourceName;//습득 자원명
    public int RewardAmount;//보상량
}

public class KnowledgeTile : MonoBehaviour, TileInterface
{
    // 타일 습득 시 호출될 이벤트
    public event System.Action OnChanged;

    //속성 정의
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();

    [SerializeField] private int _level;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _button;
    [SerializeField] private TileData _tileData;

    //읽기 전용
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public int Level => _level;
    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Button Button => GetComponent<Button>();
    public TileData TileData => _tileData;

    private const float knowledge_MoveOffsetRate = 0.8f;
    private const float knowledge_MoveOffset = 30f;

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
    /// 보상
    /// </summary>
    public void GetReward()
    {
        //2->3 파워 증가
        if (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] == 3)
        {
            ResourcesManager.Instance.GainResource("Energy", 3);
        }

        //각 유형에 따른 보상 획득
        if (_rewardResourceList.Count != 0)
        {
            foreach (var reward in _rewardResourceList)
            {
                switch (reward.RewardResourcesType)
                {
                    case RewardResourcesType.Import://수입 증가
                        ResourcesManager.Instance.ImportResourceAmount_UpDown(reward.ResourceName, reward.RewardAmount);
                        break;
                    case RewardResourcesType.SingleUse://자원 획득
                        ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                        break;
                    case RewardResourcesType.Etc://기타 효과
                        EtcRewardEffect(_tileData.researchType);
                        break;
                    default:
                        break;
                }
            }
        }
        
        //레벨 5 예외사항 : 레벨5 도달 시 수입감소
        if (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] == 5)
        {
            PlayerManager.Instance.RestUnionCount -= 1;//연방 토큰 소비
            //테라포밍 : 연방 획득
            if (this.TileData.researchType == ResearchType.Terraforming)
            {
                KnowledgeBoard_Manager.Instance._lv5_UnionTile.GetReward();
                PlayerManager.Instance.RestUnionCount += 1;//습득 연방 개수 1 증가
            }
            //사거리 : 검은 행성 배치
            if (this.TileData.researchType == ResearchType.Navigation)
            {
                BlackPlanet.AbleInstallBlackPlanet();
            }
            //경제
            if (this.TileData.researchType == ResearchType.Economy)
            {
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Ore", -2);
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Money", -4);
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Energy", -4);
            }
            //지식
            if (this.TileData.researchType == ResearchType.Science)
            {
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Knowledge", -4);
            }
        }
        //라운드 보너스
        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.Knowledge))
        {
            if (GameManager.Instance.IsRoundEffectDic[RoundEffect.Knowledge] == true)
            {
                PlayerManager.Instance.GetScore(2);
            }
        }

        //값 변경
        if (_isGet==true)
        {
            OnChanged?.Invoke();
        }
    }

    /// <summary>
    /// 기타 보상 효과
    /// </summary>
    private void EtcRewardEffect(ResearchType tileType)
    {
        switch (tileType)
        {
            case ResearchType.Terraforming:
                PlayerManager.Instance.AddOrePrice -= 1;
                break;
            case ResearchType.Navigation:
                PlayerManager.Instance.DistanceLimit += 1;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 지식 타일을 누름
    /// </summary>
    public void ClickKnowLedgeTile()
    {
        //지식 4소모 : 연구 버튼을 누를 경우만
        if(KnowledgeBoard_Manager.Instance.IsPushButton)
            ResourcesManager.Instance.ConsumeResource("Knowledge", 4);

        //레벨 5로 가기 위해서는 남은 연방 토큰 1개이상 필요
        if (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] == 4)
        {
            if (PlayerManager.Instance.RestUnionCount < 1) return;
        }

        //연구 트랙 이동
#if UNITY_EDITOR
        KnowledgeBoard_Manager.Instance.stateObjDic[this.TileData.researchType].GetComponent<RectTransform>().position 
            = (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType]==4) ?
            this._rectTransform.position+ this.RectTransform.sizeDelta.y * Vector3.up*knowledge_MoveOffsetRate
            + Vector3.down*knowledge_MoveOffset:
            this._rectTransform.position + this.RectTransform.sizeDelta.y * Vector3.up * knowledge_MoveOffsetRate;
#else
            KnowledgeBoard_Manager.Instance.stateObjDic[this.TileData.researchType].GetComponent<RectTransform>().position 
            = (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType]==4) ?
            this._rectTransform.position+ this.RectTransform.sizeDelta.y * Vector3.up*knowledge_MoveOffsetRate *0.5f:
            this._rectTransform.position + this.RectTransform.sizeDelta.y * Vector3.up * knowledge_MoveOffsetRate*0.8f;
#endif
        //플레이어의 지식 레벨 변화
        KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] += 1;

        //보상 자원 획득
        GetReward();
       
        //다시 원래대로
        KnowledgeBoard_Manager.Instance.UnActivateKnowledgeTile();
        KnowledgeBoard_Manager.Instance.IsPushButton = false;
    }
}
