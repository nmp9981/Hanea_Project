using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillTile : MonoBehaviour, TileInterface
{
    // 타일 습득 시 호출될 이벤트
    public event System.Action OnChanged;

    //속성 정의
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet = false;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();
    [SerializeField] private Button _button;
    [SerializeField] private ResearchType _researchTypeArea;

    //읽기 전용
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public Button Button => GetComponent<Button>();
    
    //타일이 있는 영역
    public ResearchType ResearchTypeArea { get; set; }


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
        _button.onClick.AddListener(GetReward);
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
                case RewardResourcesType.Import://수입 증가
                    ResourcesManager.Instance.ImportResourceAmount_UpDown(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.SingleUse://자원 획득
                    if(reward.ResourceName == "Knowledge")
                    {
                        int addKnowledge = TileSystem.CountOccupyPlanet();
                        ResourcesManager.Instance.GainResource(reward.ResourceName,  addKnowledge);
                    }
                    else ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.Score://점수 획득
                    if(reward.ResourceName == "Gaia")//가이아 행성
                    {
                        PlayerManager.Instance.IsGaiaScore = true;
                    }
                    else//7점
                    {
                        PlayerManager.Instance.GetScore(reward.RewardAmount);
                    }
                    break;
                case RewardResourcesType.Etc://기타 효과
                    
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
    }

    /// <summary>
    /// 타일 획득 표시
    /// </summary>
    void ShowGetTile()
    {
        _isGet = true;
        _button.interactable = false;
    }
}
