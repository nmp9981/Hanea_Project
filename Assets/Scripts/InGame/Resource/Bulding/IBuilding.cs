using UnityEngine;

public enum Building
{
    None,
    Mine,
    TradingStation,
    ResearchLab,
    Academy,
    PlanetaryInstitute,
    Count
}

public interface IBuilding
{
    Building BuildingType { get; }//건물 타입
    int PowerValue { get; }//파워 값
    Sprite BuildingImage { get; }//건물 이미지
    void ActivateBuilding(); // 건물이 활성화될 때 호출되는 메서드
}
