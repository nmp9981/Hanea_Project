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

    #region 타일 클릭 관련 함수 - 연방 관련 함수 포함
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

    #endregion

    /// <summary>
    /// 연방 가능한지 검사
    /// </summary>
    /// <returns></returns>
    public bool Check_AbleUnion(HashSet<Tile> _clickTileList)
    {
        //파워값 합 계산
        (int sumPower, bool isMinPower) powerCheck = TileSystem.SumPower(_clickTileList);

        //파워 조건(7이상, 최소 파워)
        if (powerCheck.sumPower < 7)
        {
            Debug.Log("파워값이 모자릅니다");
            return false;
        }
        if (!powerCheck.isMinPower)
        {
            Debug.Log("최소 파워값으로 연방 구성해야합니다.");
            return false;
        }

        //이미 연방에 등록된 타일이 있는지 검사
        foreach (var tile in _clickTileList)
        {
            //이미 등록된 타일은 사용할 수 없음
            if (tile.isUnion)
            {
                return false;
            }
            //빈행성은 연방 대상이 아님
            if(tile.PlanetType!=Planet.None && tile.InstallBuilding == Building.None)
            {
                return false;
            }
        }

        //전부 붙어있어야함

        //최소 위성 수


        return true;
    }

    /// <summary>
    /// 연방 등록
    /// </summary>
    public void EnrollUnion()
    {
        foreach (var tile in _clickTileList)
        {
            //이미 등록된 타일은 사용할 수 없음
            if (!tile.isUnion)
            {
                tile.ShowUnion();
            }
        }
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
