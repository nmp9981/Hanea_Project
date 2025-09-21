using System;
using System.Collections.Generic;
using UnityEngine;

// 자원 타입과 양을 묶는 구조체/클래스
[Serializable]
public struct ResourceAmount
{
    public Resource type; // 이전 대화에서 제안한 enum
    public int amount;
}

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public TileType tileType;
    public ResearchType researchType;
    public List<ResourceAmount> costs;
}
