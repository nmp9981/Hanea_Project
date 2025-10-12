using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 연구 타일 타입
/// </summary>
public enum ResearchType
{
    Terraforming,
    Navigation,
    AI,
    Economy,
    Science,
    Count
}

/// <summary>
/// 자원 보상 타입
/// </summary>
public enum RewardResourcesType
{
    Import,
    SingleUse,
    Etc,
    Score,
    Count
}

/// <summary>
/// 연방 토큰 보상 종류
/// </summary>
public enum UnionRewardResourcesType
{
    Ore,
    Money,
    Knowledge,
    QIC,
    Score,
    Count
}

/// <summary>
/// 인스펙터 노출을 위한 리스트 정의
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class SerializableList<T>
{
    public List<T> list;

    public SerializableList()
    {
        list = new List<T>();
    }
}

public class KnowledgeBoard_Manager : MonoBehaviour
{
    //싱글톤
    public static KnowledgeBoard_Manager Instance { get; private set; }

    //각 지식타일 등록
    [SerializeField]
    List<SerializableList<KnowledgeTile>> knowledgeTileList = new();

    //각 기술타일 등록
    [SerializeField]
    List<SkillTile> skillTile_List = new();
    //씬에 인스턴스화된 SkillTile 객체들을 저장할 리스트
    List<SkillTile> _instantiatedSkillTiles = new();
    Transform skillTile_EachArea;
    Transform skillTile_CommonArea;

    //상태 말
    [SerializeField]
    List<GameObject> statePrefabList = new();

    //상태말 관리
    [SerializeField]
    public Dictionary<ResearchType, GameObject> stateObjDic = new();

    //현재 플레이어의 지식 레벨
    [SerializeField]
    public Dictionary<ResearchType, int> playerKnowledgeLevel = new();

    //선택 가능한 지식타일들
    [SerializeField]
    List<KnowledgeTile> ableKnowledgeTile = new();
    //선택한 지식 타일
    [SerializeField]
    public KnowledgeTile _selectKnoeledgeTile;

    //얻은 연방 타일
    [SerializeField]
    public List<UnionTile> _getUnionTileList = new();
    //전체 연방 타일
    [SerializeField]
    public List<UnionTile> _unionTileList = new();

    
    private void Awake()
    {
        Set_Sington();
        InitKnowledgeState();
    }

    private void OnEnable()
    {
        SettingSkilltile();
    }

    //싱글톤 설정
    void Set_Sington()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요시 주석 해제
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 스킬 타일 세팅
    /// </summary>
    void SettingSkilltile()
    {
        skillTile_EachArea = GameObject.Find("EachArea").GetComponent<Transform>();
        skillTile_CommonArea = GameObject.Find("CommonArea").GetComponent<Transform>();

        //리스트 순서 랜덤 지정
        HashSet<int> orderList = TileSystem.OrderNumberList(8, 8);

        //지정한 순서대로 배치
        int cnt = 0;
        foreach(int idx in orderList)
        {
            GameObject skillTilePrefab = Instantiate(skillTile_List[idx].gameObject);
            SkillTile tileInstance = skillTilePrefab.GetComponent<SkillTile>();

            if (cnt < 5)//EachArea 영역
              {
                skillTilePrefab.transform.parent = skillTile_EachArea.transform;
                
                switch (cnt)
                {
                    case 0:
                        tileInstance.ResearchTypeArea = ResearchType.Terraforming;
                        break;
                    case 1:
                        tileInstance.ResearchTypeArea = ResearchType.Navigation;
                        break;
                    case 2:
                        tileInstance.ResearchTypeArea = ResearchType.AI;
                        break;
                    case 3:
                        tileInstance.ResearchTypeArea = ResearchType.Economy;
                        break;
                    case 4:
                        tileInstance.ResearchTypeArea = ResearchType.Science;
                        break;
                    default:
                        break;
                }
            }
            else//Common 영역
            {
                skillTilePrefab.transform.parent = skillTile_CommonArea.transform;
                tileInstance.ResearchTypeArea = ResearchType.Count;
            }

            //생성된 인스턴스를 새 리스트에 추가
            _instantiatedSkillTiles.Add(tileInstance);

            cnt++;
        }
    }

    /// <summary>
    /// 지식 상태초기화
    /// </summary>
    void InitKnowledgeState()
    {
        foreach (var eachTileList in knowledgeTileList)
        {
            foreach (var tile in eachTileList.list)
            {
                tile.InitTile();
                //처음엔 0단계
                if (tile.Level == 0)
                {
                    GameObject curState = statePrefabList[(int)tile.TileData.researchType];
                    //타일의 자식 오브젝트로 설정하면 좋다.
                    if (!stateObjDic.ContainsKey(tile.TileData.researchType))
                        stateObjDic.Add(tile.TileData.researchType,curState);
                    
                }
            }
        }
       
        //처음엔 모두 0
        // Enum.GetValues()를 사용하여 Planet enum의 모든 값을 배열로 가져옵니다.
        ResearchType[] researchs = (ResearchType[])Enum.GetValues(typeof(ResearchType));

        // foreach 루프를 사용하여 배열의 각 값을 순서대로 탐색합니다.
        foreach (ResearchType p in researchs)
        {
            playerKnowledgeLevel[p] = 0;
        }
    }

    /// <summary>
    /// 지식타일 활성화
    /// </summary>
    public void ActivateKnowledgeTile(ResearchType research)
    {
        // 이전 활성화 타일 초기화
        foreach (var tile in ableKnowledgeTile)
        {
            tile.Button.interactable = false;
        }
        ableKnowledgeTile.Clear();

        // ResearchType Enum 값들을 순서대로 탐색
        ResearchType[] researchs = (ResearchType[])Enum.GetValues(typeof(ResearchType));

        // 마지막 값인 Count를 제외하기 위해 루프 범위 조정
        for (int i = 0; i < researchs.Length - 1; i++)
        {
            ResearchType researchType = researchs[i];
            int nextLevel = playerKnowledgeLevel[researchType] + 1;
            
            // 다음 레벨이 5를 초과하면 건너뜁니다.
            if (nextLevel > 5)
            {
                continue;
            }

            // 전체 타일 리스트에서 해당 연구 타입과 레벨에 맞는 타일을 찾습니다.
            KnowledgeTile targetTile = FindTile(researchType, nextLevel);

            if (targetTile != null)
            {
                //타입이 정해질 경우 해당 타입만 적용
                if(research == ResearchType.Count)
                {
                    targetTile.Button.interactable = true;
                    ableKnowledgeTile.Add(targetTile);
                }
                else
                {
                    if(researchType == research)
                    {
                        targetTile.Button.interactable = true;
                        ableKnowledgeTile.Add(targetTile);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 특정 연구 타입과 레벨에 해당하는 타일을 찾는 헬퍼 메서드
    /// </summary>
    private KnowledgeTile FindTile(ResearchType researchType, int level)
    {
        foreach (var rowList in knowledgeTileList)
        {
            foreach (var tile in rowList.list)
            {
                if (tile.TileData.researchType == researchType && tile.Level == level)
                {
                    return tile;
                }
            }
        }
        return null; // 해당하는 타일을 찾지 못하면 null 반환
    }

    /// <summary>
    /// 지식타일 비활성화
    /// </summary>
    public void UnActivateKnowledgeTile()
    {
        if (ableKnowledgeTile.Count == 0) return;

        foreach(var tile in ableKnowledgeTile)
        {
            tile.Button.interactable = false;
        }
    }

    /// <summary>
    /// 기술 타일 활성화
    /// </summary>
    public void Activate_AllSkillTile()
    {
        foreach(var tile in _instantiatedSkillTiles)
        {
            if (tile.IsGet) continue;//이미 얻은것 제외
            tile.TileActive();
        }
    }
    /// <summary>
    /// 기술 타일 비활성화
    /// </summary>
    public void UnActivate_AllSkillTile()
    {
        foreach (var tile in _instantiatedSkillTiles)
        {
            tile.TileUnActive();
        }
    }

    /// <summary>
    /// 연방타일 활성화
    /// </summary>
    public void Activate_AllUnionTile()
    {
        foreach(var tile in _getUnionTileList)
        {
            tile.TileActive();
        }
    }
    /// <summary>
    /// 연방타일 활성화
    /// </summary>
    public void Activate_AllUnionTile_UI()
    {
        foreach (var tile in _unionTileList)
        {
            TextMeshProUGUI restCountText = tile.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (restCountText.text == "X 0") continue;//남아있는 연방토큰이 없는 경우 넘어감

            tile.TileActive();
        }
    }

    /// <summary>
    /// 연방타일 비활성화
    /// </summary>
    public void UnActivate_AllUnionTile()
    {
        foreach (var tile in _getUnionTileList)
        {
            tile.TileUnActive();
        }
    }
    /// <summary>
    /// 연방타일 비활성화
    /// </summary>
    public void UnActivate_AllUnionTile_UI()
    {
        foreach (var tile in _unionTileList)
        {
            tile.TileUnActive();
        }
    }
    /// <summary>
    /// 남은 연방 타일 개수 변화
    /// </summary>
    public void Change_RestUnionTileCount(UnionRewardResourcesType uniontype)
    {
        foreach (var tile in _unionTileList)
        {
            //고른 타일과 일치
            if(tile.UnionRewardResourcesType == uniontype)
            {
                TextMeshProUGUI restCountText = tile.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
                int curRestCount = int.Parse(restCountText.text.Substring(2))-1;
                restCountText.text = $"X {curRestCount}";
            }
        }
    }
}