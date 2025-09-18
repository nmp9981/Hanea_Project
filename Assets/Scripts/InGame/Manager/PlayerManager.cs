using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �÷��̾��� �׼� ����
/// </summary>
public class PlayerManager : MonoBehaviour
{
    //�̱���
    public static PlayerManager Instance { get; private set; }

    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    // �ڿ� ��ȯ ������ ����ϴ� �ν��Ͻ�
    private ExchangeResources resourceExchanger;

    //Ŭ���� Ÿ��
    private Tile _clickTile { get; set; }
    private string _tileTag = "Tile";
    private HashSet<Tile> _clickTileList = new();

    public void OnClickTradeButton()
    {
        
    }
    private void Awake()
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

        resourceExchanger = new ExchangeResources(resourcesManager.resources);
    }

    private void Start()
    {
        //�ڿ� ����
        resourcesManager.ImportAllResources();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnClick_Exchange_OreToMoney();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 �����Ͱ� UI ���� �ִ��� Ȯ��
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SettingClickTile();
            }
        }
    }

    #region Ÿ�� Ŭ�� ���� �Լ� - ���� ���� �Լ� ����
    /// <summary>
    /// Ŭ���� Ÿ�� ����
    /// </summary>
    private void SettingClickTile()
    {
        // ���콺 Ŭ�� ��ġ���� Ray�� ���� (raycast �Ÿ��� ����ؾ���)
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero, Mathf.Infinity
        );

        // Ray�� �浹�� ������Ʈ�� �ִ��� Ȯ��
        if (hit.collider != null)
        {
            // �浹�� ������Ʈ�� "Tile" �±׸� �������� Ȯ��
            if (hit.collider.gameObject.CompareTag(_tileTag))
            {
                // �浹�� ������Ʈ���� Tile ������Ʈ�� ������
                _clickTile = hit.collider.GetComponent<Tile>();
                _clickTileList.Add(_clickTile);
                //Ŭ�� ǥ��
                _clickTile.ShowClickedTile();
            }
            else
            {
                AllClear_ClickTile();
            }
        }
        else
        {
            AllClear_ClickTile();
        }
    }

    /// <summary>
    /// Ŭ���� Ÿ�ϵ� �ʱ�ȭ(Ŭ�� ǥ�� ��� �����)
    /// </summary>
    private void AllClear_ClickTile()
    {
        foreach (var tile in _clickTileList)
        {
            tile.HideClickedTile();
        }
        _clickTile = null;
        _clickTileList.Clear();
    }

    /// <summary>
    /// Ŭ���� Ÿ�� ��������
    /// </summary>
    /// <returns></returns>
    public Tile ClickedTile()
    {
        return _clickTile;
    }

    /// <summary>
    /// Ŭ���� Ÿ�� ����Ʈ ��������
    /// </summary>
    /// <returns></returns>
    public HashSet<Tile> ClickedTileList()
    {
        return _clickTileList;
    }

    #endregion

    /// <summary>
    /// ���� �������� �˻�
    /// </summary>
    /// <returns></returns>
    public bool Check_AbleUnion(HashSet<Tile> _clickTileList)
    {
        //�Ŀ��� �� ���
        (int sumPower, bool isMinPower) powerCheck = TileSystem.SumPower(_clickTileList);

        //�Ŀ� ����(7�̻�, �ּ� �Ŀ�)
        if (powerCheck.sumPower < 7)
        {
            Debug.Log("�Ŀ����� ���ڸ��ϴ�");
            return false;
        }
        if (!powerCheck.isMinPower)
        {
            Debug.Log("�ּ� �Ŀ������� ���� �����ؾ��մϴ�.");
            return false;
        }

        //�̹� ���濡 ��ϵ� Ÿ���� �ִ��� �˻�
        foreach (var tile in _clickTileList)
        {
            //�̹� ��ϵ� Ÿ���� ����� �� ����
            if (tile.isUnion)
            {
                return false;
            }
            //���༺�� ���� ����� �ƴ�
            if(tile.PlanetType!=Planet.None && tile.InstallBuilding == Building.None)
            {
                return false;
            }
        }

        //���� �پ��־����

        //�ּ� ���� ��


        return true;
    }

    /// <summary>
    /// ���� ���
    /// </summary>
    public void EnrollUnion()
    {
        foreach (var tile in _clickTileList)
        {
            //�̹� ��ϵ� Ÿ���� ����� �� ����
            if (!tile.isUnion)
            {
                tile.ShowUnion();
            }
        }
    }

    #region �ڿ� ��ȯ - Free Action
    /// <summary>
    /// ���� -> ��
    /// </summary>
    public void OnClick_Exchange_OreToMoney()
    {
        resourceExchanger.Exchange_AToB("Ore", 1, "Money", 2);
    }
    /// <summary>
    /// ������ -> ��
    /// </summary>
    public void OnClick_Exchange_EnergyToMoney()
    {
        resourceExchanger.Exchange_AToB("Energy", 1, "Money", 1);
    }
    /// <summary>
    /// ������ -> ����
    /// </summary>
    public void OnClick_Exchange_EnergyToOre()
    {
        resourceExchanger.Exchange_AToB("Energy", 3, "Ore", 1);
    }
    /// <summary>
    /// ������ -> ����
    /// </summary>
    public void OnClick_Exchange_EnergyToKnowledge()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Knowledge", 1);
    }
    /// <summary>
    /// ������ -> ��
    /// </summary>
    public void OnClick_Exchange_EnergyToQuantumIntelligenceCube()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Quantum Intelligence Cube", 1);
    }
    #endregion


}
