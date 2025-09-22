using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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
    //�� ����Ÿ�� ���
    [SerializeField]
    List<SerializableList<KnowledgeTile>> knowledgeTileList = new();

    [SerializeField]
    GameObject statePrefab;//���� ��

    //���� �÷��̾��� ���� ����
    [SerializeField]
    Dictionary<ResearchType, int> playerKnowledgeLevel = new();

    //���� ������ ����Ÿ�ϵ�
    [SerializeField]
    List<KnowledgeTile> ableKnowledgeTile = new();
    //������ ���� Ÿ��
    [SerializeField]
    public KnowledgeTile _selectKnoeledgeTile;
    
    private void Awake()
    {
        InitKnowledgeState();
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
                    GameObject state = Instantiate(statePrefab);
                    //Ÿ���� �ڽ� ������Ʈ�� �����ϸ� ����.
                    state.GetComponent<RectTransform>().position = tile.RectTransform.position;
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
    public void ActivateKnowledgeTile()
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
                targetTile.Button.interactable = true;
                ableKnowledgeTile.Add(targetTile);
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
    /// ���� Ÿ�� �̵�
    /// </summary>
    public void Move_KnoeledgeTile()
    {
        
    }
}
