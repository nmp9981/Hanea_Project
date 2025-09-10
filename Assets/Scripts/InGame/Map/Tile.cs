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
    Count
}

/// <summary>
/// 타일 위치
/// </summary>
public struct TilePosition
{
    public int areaNum;//구역 번호
    public float x, y;//좌표
};


public class Tile : MonoBehaviour
{
    [SerializeField]
    private Planet _planetType;//행성 타입
    private TilePosition _tilePos;//타일 위치
    private int _tilePower = 0;//타일 파워

    //읽기 전용
    public Planet PlanetType => _planetType;
    public TilePosition TilePos => _tilePos;
    public int TilePower => _tilePower;
}
