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
[System.Serializable]
public struct TilePosition
{
    public int x, y, z;//ť�� ��ǥ��
};


public class Tile : MonoBehaviour
{
    [SerializeField]
    private PlanetList _planetList; // ��� �༺ ������ ����Ʈ ����
    [SerializeField]
    private Planet _planetType;//�༺ Ÿ��
    [SerializeField]
    private SpriteRenderer _installBuildingImage;//��ġ�� �ǹ� �̹���
    [SerializeField]
    private GameObject _clickImage;//Ÿ�� Ŭ�� �̹���
    [SerializeField]
    private Building _installBuilding = Building.None;//��ġ�� �ǹ�
    [SerializeField]    
    private TilePosition _tilePos;//Ÿ�� ��ġ
    private int _tilePower = 0;//Ÿ�� �Ŀ�
    private bool _isUnion = false;//���� ����
    [SerializeField]
    private SpriteRenderer _spriteRenderer;//�༺ ���� ����

    //�б� ����
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
    }

    /// <summary>
    /// ť����ǥ��� ��ȯ
    /// </summary>
    void Change_CubeCoordinateSystem()
    {
        _tilePos.x = (int)((TileSystem.root3 *transform.position.x-transform.position.y)*0.3333f);
        _tilePos.z = (int)(transform.position.y*0.6667f);
        _tilePos.y = -TilePos.x-TilePos.z;
    }

    /// <summary>
    /// ���� �Ǽ��� �ǹ��� �̹��� �� �Ŀ� ����
    /// </summary>
    public void ChangeBuildingImageAndPower(Building buildingType)
    {
        //�̹��� ����
        _installBuildingImage.sprite = BuildingManager.Instance.GetBuildingSprite(buildingType);

        //��ġ�� �ǹ�
        _installBuilding = buildingType;

        //�Ŀ��� ����
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

        //Ŭ��ǥ�� ����
        ShowClickedTile();
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

    /// <summary>
    /// Ŭ���� Ÿ�� ǥ��
    /// </summary>
    /// <returns></returns>
    public void ShowClickedTile()
    {
        if(_clickImage.activeSelf) _clickImage.SetActive(false);
        else _clickImage.SetActive(true);
    }
    /// <summary>
    /// Ŭ���� Ÿ�� ǥ�� �����
    /// </summary>
    /// <returns></returns>
    public void HideClickedTile()
    {
        _clickImage.SetActive(false);
    }

    /// <summary>
    /// ���� ǥ��
    /// </summary>
    public void ShowUnion()
    {
        _isUnion = true;
        //��Ÿ���̸� ���� ��ġ
        if (_planetType == Planet.None) ChangeBuildingImageAndPower(Building.Satellite);
    }
}
