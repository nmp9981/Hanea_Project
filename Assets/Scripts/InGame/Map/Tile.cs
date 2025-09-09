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

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Planet _planetType;

    //�б� ����
    public Planet PlanetType => _planetType;
}
