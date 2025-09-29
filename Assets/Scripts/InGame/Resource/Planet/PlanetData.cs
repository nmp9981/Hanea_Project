using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Scriptable Objects/PlanetData")]
public class PlanetData : ScriptableObject
{
    public Planet planetType;//행성 타입
    public Sprite planetImage;//행성 이미지
}

[CreateAssetMenu(fileName = "PlanetList", menuName = "Game Data/Planet List")]
public class PlanetList : ScriptableObject
{
    public List<PlanetData> allPlanets = new();
}