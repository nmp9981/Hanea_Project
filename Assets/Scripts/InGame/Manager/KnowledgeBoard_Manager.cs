using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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
        HashSet<int> orderList = new HashSet<int>();
        while (orderList.Count < 8)
        {
            int ran = UnityEngine.Random.Range(0, 8);
            if (!orderList.Contains(ran))
            {
                orderList.Add(ran);
            }
        }

        //지정한 순서대로 배치
        int cnt = 0;
        foreach(int idx in orderList)
        {
            if (cnt < 5)//EachArea 영역
            {
                GameObject skillTilePrefab = Instantiate(skillTile_List[idx].gameObject);
                skillTilePrefab.transform.parent = skillTile_EachArea.transform;
                switch (idx)
                {
                    case 0:
                        skillTile_List[idx].ResearchTypeArea = ResearchType.Terraforming;
                        break;
                    case 1:
                        skillTile_List[idx].ResearchTypeArea = ResearchType.Navigation;
                        break;
                    case 2:
                        skillTile_List[idx].ResearchTypeArea = ResearchType.AI;
                        break;
                    case 3:
                        skillTile_List[idx].ResearchTypeArea = ResearchType.Economy;
                        break;
                    case 4:
                        skillTile_List[idx].ResearchTypeArea = ResearchType.Science;
                        break;
                    default:
                        break;
                }
            }
            else//Common 영역
            {
                GameObject skillTilePrefab = Instantiate(skillTile_List[idx].gameObject);
                skillTilePrefab.transform.parent = skillTile_CommonArea.transform;
                skillTile_List[idx].ResearchTypeArea = ResearchType.Count;
            }
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
    public void ActivateKnowledgeTile()
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
                targetTile.Button.interactable = true;
                ableKnowledgeTile.Add(targetTile);
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
}