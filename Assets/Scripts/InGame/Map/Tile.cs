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

    //읽기 전용
    public Planet PlanetType => _planetType;
}
