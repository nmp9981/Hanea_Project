using UnityEngine;

public enum Building
{
    None,
    Mine,
    TradingStation,
    ResearchLab,
    Academy,
    PlanetaryInstitute,
    Satellite,
    Count
}

public interface IBuilding
{
    void ActivateBuilding(); // 건물이 활성화될 때 호출되는 메서드
    void DeactivateBuilding();//건물 비활성화될 때 호출되는 메서드
}
