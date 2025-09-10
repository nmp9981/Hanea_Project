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
/// Ÿ�� ��ġ
/// </summary>
public struct TilePosition
{
    public int areaNum;//���� ��ȣ
    public float x, y;//��ǥ
};


public class Tile : MonoBehaviour
{
    [SerializeField]
    private Planet _planetType;//�༺ Ÿ��
    private TilePosition _tilePos;//Ÿ�� ��ġ
    private int _tilePower = 0;//Ÿ�� �Ŀ�

    //�б� ����
    public Planet PlanetType => _planetType;
    public TilePosition TilePos => _tilePos;
    public int TilePower => _tilePower;
}
