using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ���� Ÿ�� Ÿ��
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
/// �ڿ� ���� Ÿ��
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
/// ���� ��ū ���� ����
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
/// �ν����� ������ ���� ����Ʈ ����
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
    //�̱���
    public static KnowledgeBoard_Manager Instance { get; private set; }

    //�� ����Ÿ�� ���
    [SerializeField]
    List<SerializableList<KnowledgeTile>> knowledgeTileList = new();

    //�� ���Ÿ�� ���
    [SerializeField]
    List<SkillTile> skillTile_List = new();
    //���� �ν��Ͻ�ȭ�� SkillTile ��ü���� ������ ����Ʈ
    List<SkillTile> _instantiatedSkillTiles = new();
    Transform skillTile_EachArea;
    Transform skillTile_CommonArea;

    //���� ��
    [SerializeField]
    List<GameObject> statePrefabList = new();

    //���¸� ����
    [SerializeField]
    public Dictionary<ResearchType, GameObject> stateObjDic = new();

    //���� �÷��̾��� ���� ����
    [SerializeField]
    public Dictionary<ResearchType, int> playerKnowledgeLevel = new();

    //���� ������ ����Ÿ�ϵ�
    [SerializeField]
    List<KnowledgeTile> ableKnowledgeTile = new();
    //������ ���� Ÿ��
    [SerializeField]
    public KnowledgeTile _selectKnoeledgeTile;

    //���� ���� Ÿ��
    [SerializeField]
    public List<UnionTile> _getUnionTileList = new();
    //��ü ���� Ÿ��
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

    //�̱��� ����
    void Set_Sington()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // �ʿ�� �ּ� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��ų Ÿ�� ����
    /// </summary>
    void SettingSkilltile()
    {
        skillTile_EachArea = GameObject.Find("EachArea").GetComponent<Transform>();
        skillTile_CommonArea = GameObject.Find("CommonArea").GetComponent<Transform>();

        //����Ʈ ���� ���� ����
        HashSet<int> orderList = TileSystem.OrderNumberList(8, 8);

        //������ ������� ��ġ
        int cnt = 0;
        foreach(int idx in orderList)
        {
            GameObject skillTilePrefab = Instantiate(skillTile_List[idx].gameObject);
            SkillTile tileInstance = skillTilePrefab.GetComponent<SkillTile>();

            if (cnt < 5)//EachArea ����
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
            else//Common ����
            {
                skillTilePrefab.transform.parent = skillTile_CommonArea.transform;
                tileInstance.ResearchTypeArea = ResearchType.Count;
            }

            //������ �ν��Ͻ��� �� ����Ʈ�� �߰�
            _instantiatedSkillTiles.Add(tileInstance);

            cnt++;
        }
    }

    /// <summary>
    /// ���� �����ʱ�ȭ
    /// </summary>
    void InitKnowledgeState()
    {
        foreach (var eachTileList in knowledgeTileList)
        {
            foreach (var tile in eachTileList.list)
            {
                tile.InitTile();
                //ó���� 0�ܰ�
                if (tile.Level == 0)
                {
                    GameObject curState = statePrefabList[(int)tile.TileData.researchType];
                    //Ÿ���� �ڽ� ������Ʈ�� �����ϸ� ����.
                    if (!stateObjDic.ContainsKey(tile.TileData.researchType))
                        stateObjDic.Add(tile.TileData.researchType,curState);
                    
                }
            }
        }
       
        //ó���� ��� 0
        // Enum.GetValues()�� ����Ͽ� Planet enum�� ��� ���� �迭�� �����ɴϴ�.
        ResearchType[] researchs = (ResearchType[])Enum.GetValues(typeof(ResearchType));

        // foreach ������ ����Ͽ� �迭�� �� ���� ������� Ž���մϴ�.
        foreach (ResearchType p in researchs)
        {
            playerKnowledgeLevel[p] = 0;
        }
    }

    /// <summary>
    /// ����Ÿ�� Ȱ��ȭ
    /// </summary>
    public void ActivateKnowledgeTile(ResearchType research)
    {
        // ���� Ȱ��ȭ Ÿ�� �ʱ�ȭ
        foreach (var tile in ableKnowledgeTile)
        {
            tile.Button.interactable = false;
        }
        ableKnowledgeTile.Clear();

        // ResearchType Enum ������ ������� Ž��
        ResearchType[] researchs = (ResearchType[])Enum.GetValues(typeof(ResearchType));

        // ������ ���� Count�� �����ϱ� ���� ���� ���� ����
        for (int i = 0; i < researchs.Length - 1; i++)
        {
            ResearchType researchType = researchs[i];
            int nextLevel = playerKnowledgeLevel[researchType] + 1;
            
            // ���� ������ 5�� �ʰ��ϸ� �ǳʶݴϴ�.
            if (nextLevel > 5)
            {
                continue;
            }

            // ��ü Ÿ�� ����Ʈ���� �ش� ���� Ÿ�԰� ������ �´� Ÿ���� ã���ϴ�.
            KnowledgeTile targetTile = FindTile(researchType, nextLevel);

            if (targetTile != null)
            {
                //Ÿ���� ������ ��� �ش� Ÿ�Ը� ����
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
    /// Ư�� ���� Ÿ�԰� ������ �ش��ϴ� Ÿ���� ã�� ���� �޼���
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
        return null; // �ش��ϴ� Ÿ���� ã�� ���ϸ� null ��ȯ
    }

    /// <summary>
    /// ����Ÿ�� ��Ȱ��ȭ
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
    /// ��� Ÿ�� Ȱ��ȭ
    /// </summary>
    public void Activate_AllSkillTile()
    {
        foreach(var tile in _instantiatedSkillTiles)
        {
            if (tile.IsGet) continue;//�̹� ������ ����
            tile.TileActive();
        }
    }
    /// <summary>
    /// ��� Ÿ�� ��Ȱ��ȭ
    /// </summary>
    public void UnActivate_AllSkillTile()
    {
        foreach (var tile in _instantiatedSkillTiles)
        {
            tile.TileUnActive();
        }
    }

    /// <summary>
    /// ����Ÿ�� Ȱ��ȭ
    /// </summary>
    public void Activate_AllUnionTile()
    {
        foreach(var tile in _getUnionTileList)
        {
            tile.TileActive();
        }
    }
    /// <summary>
    /// ����Ÿ�� Ȱ��ȭ
    /// </summary>
    public void Activate_AllUnionTile_UI()
    {
        foreach (var tile in _unionTileList)
        {
            TextMeshProUGUI restCountText = tile.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (restCountText.text == "X 0") continue;//�����ִ� ������ū�� ���� ��� �Ѿ

            tile.TileActive();
        }
    }

    /// <summary>
    /// ����Ÿ�� ��Ȱ��ȭ
    /// </summary>
    public void UnActivate_AllUnionTile()
    {
        foreach (var tile in _getUnionTileList)
        {
            tile.TileUnActive();
        }
    }
    /// <summary>
    /// ����Ÿ�� ��Ȱ��ȭ
    /// </summary>
    public void UnActivate_AllUnionTile_UI()
    {
        foreach (var tile in _unionTileList)
        {
            tile.TileUnActive();
        }
    }
    /// <summary>
    /// ���� ���� Ÿ�� ���� ��ȭ
    /// </summary>
    public void Change_RestUnionTileCount(UnionRewardResourcesType uniontype)
    {
        foreach (var tile in _unionTileList)
        {
            //�� Ÿ�ϰ� ��ġ
            if(tile.UnionRewardResourcesType == uniontype)
            {
                TextMeshProUGUI restCountText = tile.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
                int curRestCount = int.Parse(restCountText.text.Substring(2))-1;
                restCountText.text = $"X {curRestCount}";
            }
        }
    }
}