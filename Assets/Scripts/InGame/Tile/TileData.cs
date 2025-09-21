using System;
using System.Collections.Generic;
using UnityEngine;

// �ڿ� Ÿ�԰� ���� ���� ����ü/Ŭ����
[Serializable]
public struct ResourceAmount
{
    public Resource type; // ���� ��ȭ���� ������ enum
    public int amount;
}

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public TileType tileType;
    public ResearchType researchType;
    public List<ResourceAmount> costs;
}
