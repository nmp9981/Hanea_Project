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
    //각 지식타일 등록
    [SerializeField]
    List<SerializableList<KnowledgeTile>> knowledgeTileList = new();

    [SerializeField]
    GameObject statePrefab;//상태 말

    //현재 플레이어의 지식 레벨
    [SerializeField]
    Dictionary<ResearchType, int> playerKnowledgeLevel = new();

    //선택 가능한 지식타일들
    [SerializeField]
    List<KnowledgeTile> ableKnowledgeTile = new();
    //선택한 지식 타일
    [SerializeField]
    public KnowledgeTile _selectKnoeledgeTile;
    
    private void Awake()
    {
        InitKnowledgeState();
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
                    GameObject state = Instantiate(statePrefab);
                    //타일의 자식 오브젝트로 설정하면 좋다.
                    state.GetComponent<RectTransform>().position = tile.RectTransform.position;
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
    /// 지식 타일 이동
    /// </summary>
    public void Move_KnoeledgeTile()
    {
        
    }
}
