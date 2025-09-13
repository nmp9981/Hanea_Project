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
    Building BuildingType { get; }//�ǹ� Ÿ��
    int PowerValue { get; }//�Ŀ� ��
    Sprite BuildingImage { get; }//�ǹ� �̹���
    void ActivateBuilding(); // �ǹ��� Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
}
