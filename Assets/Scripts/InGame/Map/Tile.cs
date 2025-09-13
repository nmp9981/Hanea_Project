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
/// Ÿ�� ��ġ
/// </summary>
public struct TilePosition
{
    public int areaNum;//���� ��ȣ
    public float x, y, z;//ť�� ��ǥ��
};


public class Tile : MonoBehaviour
{
    [SerializeField]
    private Planet _planetType;//�༺ Ÿ��
    private TilePosition _tilePos;//Ÿ�� ��ġ
    private int _tilePower = 0;//Ÿ�� �Ŀ�
    private bool _isUnion;//���� ����

    //�б� ����
    public Planet PlanetType => _planetType;
    public TilePosition TilePos => _tilePos;
    public int TilePower => _tilePower;
    public bool isUnion => _isUnion;

    private void Awake()
    {
        Change_CubeCoordinateSystem();
    }

    /// <summary>
    /// ť����ǥ��� ��ȯ
    /// </summary>
    void Change_CubeCoordinateSystem()
    {
        _tilePos.x = transform.position.x;
        _tilePos.z = transform.position.y;
        _tilePos.y = -TilePos.x-TilePos.z;
    }
}
