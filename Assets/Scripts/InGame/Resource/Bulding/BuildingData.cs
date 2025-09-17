using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    //�ǹ� ����
    public Building type;
    //���
    public List<ResourceCost> costs;
    //�Ӽ�
    public int powerValue;
    public Sprite buildingImage;
}

/// <summary>
/// ��� ����
/// </summary>
[System.Serializable]
public class ResourceCost
{
    public string resourceName;
    public int amount;
}
