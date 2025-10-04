using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private HashSet<Tile> _clickTileList = new();

    //UI
    [SerializeField]
    private TextMeshProUGUI scoreText;

    #region �÷��̾� ����
    //�÷��̾� ����
    private int _score { get; set; }
    private int _distanceLimit = 1;
    private int _addOrePrice = 3;
    public int DistanceLimit { get { return _distanceLimit; } set { _distanceLimit = value; } }//��Ÿ�
    public int AddOrePrice { get { return _addOrePrice; } set { _addOrePrice = value; } }//�߰� ����
    #endregion

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
        //���� �ʱ�ȭ
        ScoreInit();
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

    //���� �ʱ�ȭ
    void ScoreInit()
    {
        _score = 10;
        scoreText.text = _score.ToString();
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
            //�� Ÿ���� ���� ���ĵ� ����
            if (tile.PlanetType == Planet.None) continue;

            //�̹� ��ϵ� Ÿ���� ����� �� ����
            if (tile.isUnion)
            {
                Debug.Log("�̹� ���濡 ��ϵ� Ÿ��");
                return false;
            }
            //���༺�� ���� �Ұ���
            if(tile.PlanetType!=Planet.None && tile.InstallBuilding == Building.None)
            {
                Debug.Log("���༺ ����");
                return false;
            }
        }
       
        //���� �پ��־����
        if (!TileSystem.IsConnect_AllFactor(_clickTileList)) return false;

        //�ּ� ���� ��
        if (!TileSystem.IsMin_Satellite_AllFactor(_clickTileList)) return false;
        return true;
    }

    /// <summary>
    /// ���� ��� �� ���� ���� ��ȯ
    /// </summary>
    public int EnrollUnion_ReturnCountSatellite()
    {
        int countSatellite = 0;
        foreach (var tile in _clickTileList)
        {
            //�̹� ��ϵ� Ÿ���� ����� �� ����
            if (!tile.isUnion)
            {
                //������̸� ������ ��� �߰�
                if (tile.PlanetType == Planet.None) countSatellite += 1;
                //���� ǥ��
                tile.ShowUnion();
            }
        }
        //Ŭ���� Ÿ�� �ʱ�ȭ
        AllClear_ClickTile();
        return countSatellite;
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

    /// <summary>
    /// ���� ȹ��
    /// </summary>
    /// <param name="amount"></param>
    public void GetScore(int amount)
    {
        _score += amount;
        _score = Mathf.Max(0, _score);//������ �Ұ�
        scoreText.text = _score.ToString();
    }
}
