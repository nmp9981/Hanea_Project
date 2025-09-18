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
    void ActivateBuilding(); // �ǹ��� Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
    void DeactivateBuilding();//�ǹ� ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
}
