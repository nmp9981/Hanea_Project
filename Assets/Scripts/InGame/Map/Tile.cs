using System;
using UnityEngine;

public enum Planet
{
    Fire,
    Volcano, 
    Desert,
    Swamp,
    Titanum,
    Ice,
    Earth,
    Gaia,
    Dimension,
    Black,
    None,
    Count
}

/// <summary>
/// 타일 위치
/// </summary>
[System.Serializable]
public struct TilePosition
{
    public int x, y, z;//큐브 좌표계
};


public class Tile : MonoBehaviour
{
    [SerializeField]
    private PlanetList _planetList; // 모든 행성 데이터 리스트 참조
    [SerializeField]
    private Planet _planetType;//행성 타입
    [SerializeField]
    private SpriteRenderer _installBuildingImage;//설치한 건물 이미지
    [SerializeField]
    private GameObject _clickImage;//타일 클릭 이미지
    [SerializeField]
    private GameObject _distShowImage;//사거리 확인용 이미지
    [SerializeField]
    private Building _installBuilding = Building.None;//설치한 건물
    [SerializeField]    
    private TilePosition _tilePos;//타일 위치
    private int _tilePower = 0;//타일 파워
    private bool _isUnion = false;//연방 여부
    [SerializeField]
    private SpriteRenderer _spriteRenderer;//행성 적용 색상

    //읽기 전용
    public Planet PlanetType => _planetType;
    public SpriteRenderer InstallBuildingImage => _installBuildingImage;
    public Building InstallBuilding => _installBuilding;
    public TilePosition TilePos => _tilePos;
    public int TilePower => _tilePower;
    public bool isUnion => _isUnion;

    private void Awake()
    {
        ApplyPlanetColor();
    }

    private void OnEnable()
    {
        Change_CubeCoordinateSystem();
        HideDistanceTile();
    }

    /// <summary>
    /// 큐브좌표계로 변환
    /// </summary>
    void Change_CubeCoordinateSystem()
    {
        _tilePos.x = (int)((TileSystem.root3 *transform.position.x-transform.position.y)*0.3333f);
        _tilePos.z = (int)(transform.position.y*0.6667f);
        _tilePos.y = -TilePos.x-TilePos.z;
    }

    /// <summary>
    /// 현재 건설한 건물로 이미지 및 파워 변경
    /// </summary>
    public void ChangeBuildingImageAndPower(Building buildingType)
    {
        //이미지 변경
        _installBuildingImage.sprite = BuildingManager.Instance.GetBuildingSprite(buildingType);

        //설치한 건물
        _installBuilding = buildingType;

        //파워값 변경
        switch (buildingType)
        {
            case Building.Mine:
                _tilePower = 1;
                break;
            case Building.TradingStation:
            case Building.ResearchLab:
                _tilePower = 2;
                break;
            case Building.Academy:
            case Building.PlanetaryInstitute:
                _tilePower = 3;
                break;
            default:
                _tilePower = 0;
                break;
        }

        //클릭표시 제거
        ShowClickedTile();
    }
    /// <summary>
    /// 파워 4로 증가
    /// </summary>
    public void TilePowerUP()
    {
        _tilePower = 4;
    }
    /// <summary>
    /// 행성 타입에 따라 이미지 색상 변경
    /// </summary>
    private void ApplyPlanetColor()
    {
        Array enumValues = Enum.GetValues(typeof(Planet));

        // 배열의 길이 내에서 무작위 인덱스를 생성합니다.
        // UnityEngine.Random.Range(min, max)에서 정수형은 max가 exclusive(포함되지 않음)입니다.
        //일정 수를 넘는 랜덤값은 모두 None으로
        int randomIndex = UnityEngine.Random.Range(-enumValues.Length/4, 2*enumValues.Length);
        randomIndex = randomIndex >= enumValues.Length-2 ? enumValues.Length-2 : randomIndex;
        randomIndex = randomIndex < 0 ? 0 : randomIndex;

        // 무작위 인덱스에 해당하는 값을 가져와 열거형 타입으로 캐스팅하여 반환합니다.
        Planet ranType = (Planet)enumValues.GetValue(randomIndex);

        //검은 행성 제외
        if (ranType == Planet.Black)
        {
            _spriteRenderer.sprite = null;
            _planetType = Planet.None;
            return;
        }

        PlanetData currentPlanetData = _planetList.allPlanets.Find(data => data.planetType == ranType);

        if (currentPlanetData != null)
        {
            if(currentPlanetData.planetType == Planet.None)
            {
                _spriteRenderer.sprite = null;
            }else _spriteRenderer.sprite = currentPlanetData.planetImage;
            _planetType = currentPlanetData.planetType;
        }
        TileManager.Instance.allTileList_MainBoard.Add(this);
    }

    #region 타일 표시 관련 로직
    /// <summary>
    /// 클릭한 타일 표시
    /// </summary>
    /// <returns></returns>
    public void ShowClickedTile()
    {
        if(_clickImage.activeSelf) _clickImage.SetActive(false);
        else _clickImage.SetActive(true);
    }
    /// <summary>
    /// 클릭한 타일 표시 지우기
    /// </summary>
    /// <returns></returns>
    public void HideClickedTile()
    {
        _clickImage.SetActive(false);
    }

    /// <summary>
    /// 사거리 타일 표시 로직
    /// </summary>
    public void ShowDistanceTile()
    {
        _distShowImage.SetActive(true);
    }
    /// <summary>
    /// 사거리 타일 표시 숨기기 로직
    /// </summary>
    public void HideDistanceTile()
    {
        _distShowImage.SetActive(false);
    }

    /// <summary>
    /// 연방 표시
    /// </summary>
    public void ShowUnion()
    {
        _isUnion = true;
        //빈타일이면 위성 설치
        if (_planetType == Planet.None) ChangeBuildingImageAndPower(Building.Satellite);
    }

    /// <summary>
    /// 검은 행성 표시
    /// </summary>
    public void ShowBlackPlanet()
    {
        PlanetData currentPlanetData = _planetList.allPlanets.Find(data => data.planetType == Planet.Black);

        if (currentPlanetData != null)
        {
            _spriteRenderer.sprite = currentPlanetData.planetImage;
            _planetType = currentPlanetData.planetType;
        }
    }
    #endregion
}
