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
    private Planet _planetType;//행성 타입
    private TilePosition _tilePos;//타일 위치
    private int _tilePower = 0;//타일 파워
    private bool _isUnion;//연방 여부

    //읽기 전용
    public Planet PlanetType => _planetType;
    public TilePosition TilePos => _tilePos;
    public int TilePower => _tilePower;
    public bool isUnion => _isUnion;

    private void Awake()
    {
        Change_CubeCoordinateSystem();
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
}
