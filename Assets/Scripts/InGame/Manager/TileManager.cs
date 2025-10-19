using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static TileManager Instance { get; private set; }

    // 모든 ActionTile 인스턴스를 저장할 리스트
    [SerializeField]
    private List<ActionTile> activeTiles = new List<ActionTile>();

    [SerializeField]
    public GameObject mainBoardObj;
    [SerializeField]
    public List<Tile> allTileList_MainBoard = new List<Tile>();

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요하다면 추가
        }
        else
        {
            Destroy(gameObject);
        }
        mainBoardObj = GameObject.Find("MainBoard");
    }

    /// <summary>
    /// 모든 등록된 타일의 TileActive() 메서드를 호출
    /// </summary>
    public void ActivateAllTiles()
    {
        foreach (ActionTile tile in activeTiles)
        {
            tile.TileActive();
        }
    }
}
