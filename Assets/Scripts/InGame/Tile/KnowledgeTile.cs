using Mono.Cecil;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ÿ�� Ÿ��
/// </summary>
public enum TileType
{
    Knowledge,
    Skill,
    Energy,
    Information,
    Count
}


/// <summary>
/// Ÿ�� �������̽� ����
/// </summary>
public interface TileInterface
{
    //����
    TileType Type { get; }//Ÿ�� ����

    bool IsGet { get; }//���� ����

    //����
    void Reward();
}
public class KnowledgeTile : MonoBehaviour, TileInterface
{
    // Ÿ�� ���� �� ȣ��� �̺�Ʈ
    public event System.Action OnChanged;

    //�Ӽ� ����
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet;
    [SerializeField] private int _level;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _button;
    [SerializeField] private TileData _tileData;

    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public int Level => _level;
    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Button Button => GetComponent<Button>();
    public TileData TileData => _tileData;

    // �������̽� ������ ���� Dictionary
    public Dictionary<Resource, int> Costs { get; private set; }

    void Awake()
    {
        InitTile();
        //LoadCostsFromData();
    }

    /// <summary>
    /// Ÿ�� �ʱ�ȭ
    /// </summary>
    public void InitTile()
    {
        _isGet = false;

        if(_level==0) _isGet = true;

        _rectTransform = this.RectTransform;
        _button = this.Button;
        _button.interactable = false;//��ư ��Ȱ��ȭ

        _button.onClick.AddListener(ClickKnowLedgeTile);
    }

    /// <summary>
    /// Ÿ�� �ڿ� ������ �ε�
    /// </summary>
    private void LoadCostsFromData()
    {
        Costs = new Dictionary<Resource, int>();
        foreach (var costAmount in _tileData.costs)
        {
            Costs[costAmount.type] = costAmount.amount;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Reward()
    {
        switch (_tileData.researchType)
        {
            case ResearchType.Terraforming:
                Debug.Log("���� ����");
                break;
            case ResearchType.Navigation:
                Debug.Log("���Ÿ� ����");
                break;
            case ResearchType.AI:
                Debug.Log("���� ����");
                break;
            case ResearchType.Economy:
                break;
            case ResearchType.Science:
                break;
            default:
                break;
        }


        //�� ����
        if (_isGet==true)
        {
            OnChanged?.Invoke();
        }
    }
    /// <summary>
    /// ���� Ÿ���� ����
    /// </summary>
    public void ClickKnowLedgeTile()
    {
        //���� 4�Ҹ�
        if (ResourcesManager.Instance.HasEnoughResources("Knowledge", 4))
            ResourcesManager.Instance.ConsumeResource("Knowledge", 4);

        //���� Ʈ�� �̵�
        KnowledgeBoard_Manager.Instance.stateObjDic[this.TileData.researchType].GetComponent<RectTransform>().position 
            += this.RectTransform.sizeDelta.y*Vector3.up*0.5f;

        //�÷��̾��� ���� ���� ��ȭ
        KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] += 1;
        //2�� ȹ��
        PlayerManager.Instance.GetScore(2);
        //�ٽ� �������
        KnowledgeBoard_Manager.Instance.UnActivateKnowledgeTile();
    }
}
