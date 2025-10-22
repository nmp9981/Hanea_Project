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
    
    //타일이 있는 영역
    public ResearchType ResearchTypeArea { get; set; }
    //기술타일 표시 영역
    [SerializeField]
    Transform skillTileArea;

    void Awake()
    {
        skillTileArea = GameObject.Find("OwnSkillTileArea").GetComponent<Transform>();
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
        _isGet = false;
        _button = this.Button;
        _button.interactable = false;
        _button.onClick.AddListener(GetReward);
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
                    }else if (reward.ResourceName == "Lab")//연구소 점수
                    {
                        int getScore = reward.RewardAmount * PlayerManager.Instance._installBuidingCount[Building.ResearchLab];
                        PlayerManager.Instance.GetScore(getScore);
                    }
                    else//7점
                    {
                        PlayerManager.Instance.GetScore(reward.RewardAmount);
                    }
                    break;
                case RewardResourcesType.Etc://기타 효과
                    PowerValueUp_Academy_Institute();
                    break;
                default:
                    break;
            }
        }

        //획득 표시
        ShowGetTile();

        //지식 타일 올리기
        if(ResearchTypeArea == ResearchType.Count)
        {
            KnowledgeBoard_Manager.Instance.ActivateKnowledgeTile(ResearchType.Count);
        }else KnowledgeBoard_Manager.Instance.ActivateKnowledgeTile(ResearchTypeArea);

        //값 변경
        if (_isGet == true)
        {
            OnChanged?.Invoke();
        }
        //다시 비활성화
        KnowledgeBoard_Manager.Instance.UnActivate_AllSkillTile();
    }

    /// <summary>
    /// 타일 획득 표시
    /// </summary>
    void ShowGetTile()
    {
        _isGet = true;
        _button.interactable = false;

        GameObject tileGm = Instantiate(this.gameObject);
        tileGm.transform.parent = skillTileArea;
    }

    /// <summary>
    /// 의회, 아카데미 파워값 증가
    /// </summary>
    void PowerValueUp_Academy_Institute()
    {
        BuildingManager.Instance.buildingDataList[3].powerValue = 4;
        BuildingManager.Instance.buildingDataList[4].powerValue = 4;
        //이미 설치되어 있는 건물도 포함
        foreach(var tile in TileManager.Instance.allTileList_MainBoard)
        {
            if (tile.InstallBuilding == Building.PlanetaryInstitute ||
               tile.InstallBuilding == Building.Academy) tile.TilePowerUP();
        }
    }
    
}
