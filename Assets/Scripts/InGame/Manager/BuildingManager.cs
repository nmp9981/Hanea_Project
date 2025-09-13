using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField]
    public static List<Sprite> _buildingSpriteList = new();

    /// <summary>
    /// 건물 짓기
    /// 어느 타일에 해당 건물을 짓는다.
    /// </summary>
    public void InstallBuiling(Tile tile, Building buildingType)
    {

    }
}
