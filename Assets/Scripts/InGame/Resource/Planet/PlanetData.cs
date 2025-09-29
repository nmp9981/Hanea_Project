using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Scriptable Objects/PlanetData")]
public class PlanetData : ScriptableObject
{
    public Planet planetType;//�༺ Ÿ��
    public Sprite planetImage;//�༺ �̹���
}

[CreateAssetMenu(fileName = "PlanetList", menuName = "Game Data/Planet List")]
public class PlanetList : ScriptableObject
{
    public List<PlanetData> allPlanets = new();
}