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
public struct TilePosition
{
    public int areaNum;//구역 번호
    public float x, y, z;//큐브 좌표계
};


public class Tile : MonoBehaviour
{
    [SerializeField]
    private PlanetList _planetList; // 모든 행성 데이터 리스트 참조
    [SerializeField]
    private Planet _planetType;//행성 타입
    [SerializeField]
    private SpriteRenderer _installBuildingImage;//설치한 건물 이미지
    private TilePosition _tilePos;//타일 위치
    private int _tilePower = 0;//타일 파워
    private bool _isUnion;//연방 여부
    [SerializeField]
    private SpriteRenderer _spriteRenderer;//행성 적용 색상

    //읽기 전용
    public Planet PlanetType => _planetType;
    public SpriteRenderer InstallBuildingImage => _installBuildingImage;
    public TilePosition TilePos => _tilePos;
    public int TilePower => _tilePower;
    public bool isUnion => _isUnion;

    private void Awake()
    {
        Change_CubeCoordinateSystem();
        ApplyPlanetColor();
    }

    /// <summary>
    /// 큐브좌표계로 변환
    /// </summary>
    void Change_CubeCoordinateSystem()
    {
        _tilePos.x = transform.position.x;
        _tilePos.z = transform.position.y;
        _tilePos.y = -TilePos.x-TilePos.z;
    }

    /// <summary>
    /// 현재 건설한 건물로 이미지 변경
    /// </summary>
    public void ChangeBuildingImage(Building buildingType)
    {
        //_installBuildingImage = BuildingManager._buildingSpriteList[(int)buildingType-1];
    }
    /// <summary>
    /// 행성 타입에 따라 이미지 색상 변경
    /// </summary>
    private void ApplyPlanetColor()
    {
        PlanetData currentPlanetData = _planetList.allPlanets.Find(data => data.planetType == _planetType);

        if (currentPlanetData != null)
        {
            _spriteRenderer.color = currentPlanetData.planetColor;
        }
    }
}
