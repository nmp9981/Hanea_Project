using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플레이어의 액션 관리
/// </summary>
public class PlayerManager : MonoBehaviour
{
    //싱글톤
    public static PlayerManager Instance { get; private set; }

    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    // 자원 교환 로직을 담당하는 인스턴스
    private ExchangeResources resourceExchanger;

    //클릭한 타일
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
            // DontDestroyOnLoad(gameObject); // 필요시 주석 해제
        }
        else
        {
            Destroy(gameObject);
        }

        resourceExchanger = new ExchangeResources(resourcesManager.resources);
    }

    private void Start()
    {
        //자원 수입
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
            // 마우스 포인터가 UI 위에 있는지 확인
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SettingClickTile();
            }
        }
    }

    /// <summary>
    /// 클릭한 타일 설정
    /// </summary>
    private void SettingClickTile()
    {
        // 마우스 클릭 위치에서 Ray를 생성 (raycast 거리가 충분해야함)
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero, Mathf.Infinity
        );

        // Ray가 충돌한 오브젝트가 있는지 확인
        if (hit.collider != null)
        {
            // 충돌한 오브젝트가 "Tile" 태그를 가졌는지 확인
            if (hit.collider.gameObject.CompareTag(_tileTag))
            {
                // 충돌한 오브젝트에서 Tile 컴포넌트를 가져옴
                _clickTile = hit.collider.GetComponent<Tile>();
                _clickTileList.Add(_clickTile);
                //클릭 표시
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
    /// 클릭한 타일들 초기화(클릭 표시 모두 숨기기)
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
    /// 클릭한 타일 가져오기
    /// </summary>
    /// <returns></returns>
    public Tile ClickedTile()
    {
        return _clickTile;
    }

    /// <summary>
    /// 클릭한 타일 리스트 가져오기
    /// </summary>
    /// <returns></returns>
    public HashSet<Tile> ClickedTileList()
    {
        return _clickTileList;
    }



    #region 자원 변환 - Free Action
    /// <summary>
    /// 광석 -> 돈
    /// </summary>
    public void OnClick_Exchange_OreToMoney()
    {
        resourceExchanger.Exchange_AToB("Ore", 1, "Money", 2);
    }
    /// <summary>
    /// 에너지 -> 돈
    /// </summary>
    public void OnClick_Exchange_EnergyToMoney()
    {
        resourceExchanger.Exchange_AToB("Energy", 1, "Money", 1);
    }
    /// <summary>
    /// 에너지 -> 광석
    /// </summary>
    public void OnClick_Exchange_EnergyToOre()
    {
        resourceExchanger.Exchange_AToB("Energy", 3, "Ore", 1);
    }
    /// <summary>
    /// 에너지 -> 지식
    /// </summary>
    public void OnClick_Exchange_EnergyToKnowledge()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Knowledge", 1);
    }
    /// <summary>
    /// 에너지 -> 돈
    /// </summary>
    public void OnClick_Exchange_EnergyToQuantumIntelligenceCube()
    {
        resourceExchanger.Exchange_AToB("Energy", 4, "Quantum Intelligence Cube", 1);
    }
    #endregion


}
