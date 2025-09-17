using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    //건물 종류
    public Building type;
    //비용
    public List<ResourceCost> costs;
    //속성
    public int powerValue;
    public Sprite buildingImage;
}

/// <summary>
/// 비용 정의
/// </summary>
[System.Serializable]
public class ResourceCost
{
    public string resourceName;
    public int amount;
}
