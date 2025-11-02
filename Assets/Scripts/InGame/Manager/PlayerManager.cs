using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private HashSet<Tile> _clickTileList = new();

    //UI
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI _noticeText;

    #region 플레이어 정보
    //플레이어 정보
    private int _score { get; set; }
    private int _distanceLimit = 1;
    private int _addOrePrice = 3;
    private bool _isGaiaScore = false;
    private int _restunionCount = 0;
    private int _addInstituteBonusPower = 0;
    public int DistanceLimit { get { return _distanceLimit; } set { _distanceLimit = value; } }//사거리
    public int AddOrePrice { get { return _addOrePrice; } set { _addOrePrice = value; } }//추가 삽비용
    public bool IsGaiaScore { get { return _isGaiaScore; }set { _isGaiaScore = value; } }//가이아 행성 개수
    public int RestUnionCount { get { return _restunionCount; } set { _restunionCount = value; } }//남은 연방타일 개수
    public int AddInstituteBonusPower { get { return _addInstituteBonusPower; } set { _addInstituteBonusPower = value; } }//모행성 보너스
    public Dictionary<Planet, bool> _planetOccupyDic = new Dictionary<Planet, bool>();//행성 점령 유형
    public Dictionary<Building, int> _installBuidingCount = new Dictionary<Building, int>();//각 건물별 지어진 개수
    #endregion

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
        //점수 초기화
        ScoreInit();
        //행성 점령 기록 초기화
        OccupyInit();
        //건물 설치 개수 초기화
        BuildingCountInit();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 포인터가 UI 위에 있는지 확인
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SettingClickTile();
            }
        }
    }

    //점수 초기화
    void ScoreInit()
    {
        _score = 10;
        scoreText.text = _score.ToString();
    }
    //점령 기록 초기화
    void OccupyInit()
    {
        Array enumPlanetValues = Enum.GetValues(typeof(Planet));

        foreach (Planet planet in enumPlanetValues)
        {
            if (planet == Planet.None || planet == Planet.Count) continue;
            _planetOccupyDic.Add(planet, false); 
        }
    }
    //건물 기록 초기화
    void BuildingCountInit()
    {
        Array enumBuildingValues = Enum.GetValues(typeof(Building));

        foreach (Building build in enumBuildingValues) {
            if (build == Building.None || build == Building.Count || build == Building.Satellite) continue;
            _installBuidingCount.Add(build,0);
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
                //빈 행성 타일은 사거리 표시
                TileSystem.ShowNavigaitionDist_ClickTile(_clickTile);
                //검은 행성(1개만 설치 가능)
                if (_clickTile.PlanetType == Planet.None &&
                    KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[ResearchType.Navigation] == 5
                    && _planetOccupyDic[Planet.Black] == false) BlackPlanet.Effect_BlackPlanetTile(_clickTile);
                //삽기능
                if (BuildingManager.Instance.sabDecreaseCount != 0)
                {
                    TileSystem.Effect_SabMine(_clickTile);
                }
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
    public void AllClear_ClickTile()
    {
        foreach (var tile in TileManager.Instance.allTileList_MainBoard)
        {
            tile.HideClickedTile();
            tile.HideDistanceTile();
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
            ShowMessage("현재 파워값 : "+powerCheck.sumPower + ", 파워 값이 모자릅니다.");
            return false;
        }
        if (!powerCheck.isMinPower)
        {
            ShowMessage("현재 파워값 : " + powerCheck.sumPower + ", 최소 파워값으로 연방을 구성해야합니다.");
            return false;
        }
        
        //이미 연방에 등록된 타일이 있는지 검사
        foreach (var tile in _clickTileList)
        {
            //빈 타일은 연방 겨쳐도 무관
            if (tile.PlanetType == Planet.None) continue;

            //이미 등록된 타일은 사용할 수 없음
            if (tile.isUnion)
            {
                ShowMessage("이미 연방에 등록된 타일이 있습니다.");
                return false;
            }
            //빈행성은 연방 불가능
            if (tile.PlanetType!=Planet.None && tile.InstallBuilding == Building.None)
            {
                ShowMessage("빈행성이 포함되어 있습니다.");
                return false;
            }
        }
       
        //전부 붙어있어야함
        if (!TileSystem.IsConnect_AllFactor(_clickTileList)) return false;

        //최소 위성 수
        if (!TileSystem.IsMin_Satellite_AllFactor(_clickTileList)) return false;
        return true;
    }

    /// <summary>
    /// 위성 개수 반환
    /// </summary>
    public int ReturnCountSatellite()
    {
        int countSatellite = 0;
        foreach (var tile in _clickTileList)
        {
            //이미 등록된 타일은 사용할 수 없음
            if (!tile.isUnion)
            {
                //빈공간이면 에너지 비용 추가
                if (tile.PlanetType == Planet.None) countSatellite += 1;
            }
        }
        return countSatellite;
    }

    /// <summary>
    /// 연방 등록 및 위성 개수 반환
    /// </summary>
    public void EnrollUnion()
    {
        foreach (var tile in _clickTileList)
        {
            //이미 등록된 타일은 사용할 수 없음
            if (!tile.isUnion)
            {
                //건물이면 건물 개수 추가
                if (tile.PlanetType != Planet.None) GameManager.Instance.finalBonusList[3].CountUP();
                //연방 표시
                tile.ShowUnion();
            }
        }
        //클릭한 타일 초기화
        AllClear_ClickTile();
        return;
    }

    #region 자원 변환 - Free Action
    /// <summary>
    /// 광석 -> 돈
    /// </summary>
    public void OnClick_Exchange_OreToMoney()
    {
        resourceExchanger.Exchange_AToB("Ore", 1, "Money", 1);
    }
    /// <summary>
    /// 광석 -> 에너지
    /// </summary>
    public void OnClick_Exchange_OreToEnergy()
    {
        resourceExchanger.Exchange_AToB("Ore", 1, "Energy", 1);
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
    /// <summary>
    /// 정보큐브 -> 광석
    /// </summary>
    public void OnClick_Exchange_QuantumIntelligenceCubeToOre()
    {
        resourceExchanger.Exchange_AToB("Quantum Intelligence Cube", 1,"Ore",1);
    }
    #endregion

    /// <summary>
    /// 점수 획득
    /// </summary>
    /// <param name="amount"></param>
    public void GetScore(int amount)
    {
        _score += amount;
        _score = Mathf.Max(0, _score);//음수는 불가
        scoreText.text = _score.ToString();
    }
    /// <summary>
    /// 최종 점수
    /// </summary>
    /// <returns>최종 점수</returns>
    public int FinalScore()
    {
        return _score;
    }

    /// <summary>
    /// 알림 메세지 코루틴 호출
    /// </summary>
    public void ShowMessage(string message)
    {
        StartCoroutine(ShowMessage_Cor(message));
    }

    /// <summary>
    /// 알림 메세지 보이기
    /// </summary>
    /// <param name="message">메세지</param>
    /// <returns></returns>
    private IEnumerator ShowMessage_Cor(string message)
    {
        _noticeText.transform.parent.gameObject.SetActive(true);
        _noticeText.text = message;
        yield return new WaitForSecondsRealtime(2.0f);
        _noticeText.transform.parent.gameObject.SetActive(false);
    }
}
