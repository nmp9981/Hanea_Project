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
    private PlanetList _planetList; // ��� �༺ ������ ����Ʈ ����
    [SerializeField]
    private Planet _planetType;//�༺ Ÿ��
    [SerializeField]
    private SpriteRenderer _installBuildingImage;//��ġ�� �ǹ� �̹���
    private TilePosition _tilePos;//Ÿ�� ��ġ
    private int _tilePower = 0;//Ÿ�� �Ŀ�
    private bool _isUnion;//���� ����
    [SerializeField]
    private SpriteRenderer _spriteRenderer;//�༺ ���� ����

    //�б� ����
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
    /// ť����ǥ��� ��ȯ
    /// </summary>
    void Change_CubeCoordinateSystem()
    {
        _tilePos.x = transform.position.x;
        _tilePos.z = transform.position.y;
        _tilePos.y = -TilePos.x-TilePos.z;
    }

    /// <summary>
    /// ���� �Ǽ��� �ǹ��� �̹��� ����
    /// </summary>
    public void ChangeBuildingImage(Building buildingType)
    {
        //_installBuildingImage = BuildingManager._buildingSpriteList[(int)buildingType-1];
    }
    /// <summary>
    /// �༺ Ÿ�Կ� ���� �̹��� ���� ����
    /// </summary>
    private void ApplyPlanetColor()
    {
        Array enumValues = Enum.GetValues(typeof(Planet));

        // �迭�� ���� ������ ������ �ε����� �����մϴ�.
        // UnityEngine.Random.Range(min, max)���� �������� max�� exclusive(���Ե��� ����)�Դϴ�.
        int randomIndex = UnityEngine.Random.Range(0, enumValues.Length-1);

        // ������ �ε����� �ش��ϴ� ���� ������ ������ Ÿ������ ĳ�����Ͽ� ��ȯ�մϴ�.
        Planet ranType = (Planet)enumValues.GetValue(randomIndex);

        PlanetData currentPlanetData = _planetList.allPlanets.Find(data => data.planetType == ranType);

        if (currentPlanetData != null)
        {
            _spriteRenderer.color = currentPlanetData.planetColor;
        }
    }
}
