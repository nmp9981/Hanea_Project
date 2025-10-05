using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTile : MonoBehaviour, TileInterface
{
    // 타일 습득 시 호출될 이벤트
    public event System.Action OnChanged;

    //속성 정의
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();
    [SerializeField] private Button _button;
    [SerializeField] private int _costAmount;
    private string _costResource;
    
    //읽기 전용
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public Button Button => GetComponent<Button>();
    public int CostAmount => _costAmount;

    void Awake()
    {
        InitTile();
    }



    /// <summary>
    /// 타일 초기화
    /// </summary>
    public void InitTile()
    {
        _button = this.Button;
        TileActive();//타일 활성화
        SetCostResource();//지불 자원 설정

        _button.onClick.AddListener(ClickKnowLedgeTile);
    }

    /// <summary>
    /// 타일 활성화
    /// </summary>
    public void TileActive()
    {
        _isGet = false;
        _button.interactable = true;//버튼 활성화
    }

    /// <summary>
    /// 지불 자원 설정
    /// </summary>
    void SetCostResource()
    {
        switch (_type)
        {
            case TileType.Energy:
                _costResource = "Energy";
                break;
            case TileType.Information:
                _costResource = "Quantum Intelligence Cube";
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// 보상
    /// </summary>
    public void GetReward()
    {
        //정보큐브 액션인지 에너지 액션인지에 따라 구분
        if (_type == TileType.Energy)
        {
            //각 유형에 따른 보상 획득
            if (_rewardResourceList.Count == 0) return;
            foreach (var reward in _rewardResourceList)
            {
                switch (reward.RewardResourcesType)
                {
                    case RewardResourcesType.Import://수입 증가
                        ResourcesManager.Instance.ImportResourceAmount_UpDown(reward.ResourceName, reward.RewardAmount);
                        break;
                    case RewardResourcesType.SingleUse://자원 획득
                        if(reward.ResourceName == "Energy")//에너지는 1점 감정
                        {
                            PlayerManager.Instance.GetScore(-1);
                        }
                        ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                        break;
                    case RewardResourcesType.Etc://기타 효과
                                                 //놓을 수 있는곳 표시
                        break;
                    default:
                        break;
                }
            }
        }
        else if (_type == TileType.Information)//정보큐브 액션
        {
            switch(_costAmount)
            {
                case 4://타일 가져오기
                    KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();
                    break;
                case 3://연방 효과 재사용
                    break;
                case 2://점수
                    int addScore = 3 + TileSystem.CountOccupyPlanet();
                    PlayerManager.Instance.GetScore(addScore);
                    break;
                default:
                    break;
            }
        }

        //값 변경
        if (_isGet == true)
        {
            OnChanged?.Invoke();
        }
    }

    /// <summary>
    /// 지식 타일을 누름
    /// </summary>
    public void ClickKnowLedgeTile()
    {
        //자원을 낼 수 있는가?
        if (ResourcesManager.Instance.HasEnoughResources(_costResource, _costAmount))
        {
            ResourcesManager.Instance.ConsumeResource(_costResource, _costAmount);
        }else return;
        
        //보상 자원 획득
        GetReward();

        //버튼 비활성화 (각 라운드 별로 1회용)
        _button.interactable = false;
        _isGet = true;
    }
}
