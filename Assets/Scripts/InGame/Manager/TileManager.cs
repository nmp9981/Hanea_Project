using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static TileManager Instance { get; private set; }

    // ��� ActionTile �ν��Ͻ��� ������ ����Ʈ
    [SerializeField]
    private List<ActionTile> activeTiles = new List<ActionTile>();

    [SerializeField]
    public GameObject mainBoardObj;
    [SerializeField]
    public List<Tile> allTileList_MainBoard = new List<Tile>();

    void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // �ʿ��ϴٸ� �߰�
        }
        else
        {
            Destroy(gameObject);
        }
        mainBoardObj = GameObject.Find("MainBoard");
    }

    /// <summary>
    /// ��� ��ϵ� Ÿ���� TileActive() �޼��带 ȣ��
    /// </summary>
    public void ActivateAllTiles()
    {
        foreach (ActionTile tile in activeTiles)
        {
            tile.TileActive();
        }
    }
}
