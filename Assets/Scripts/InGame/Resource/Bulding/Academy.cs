using UnityEngine;

public class Academy : IBuilding
{
    //속성 정의
    [SerializeField] private Building _type = Building.Academy;
    [SerializeField] private int _powerValue = 3;
    [SerializeField] private Sprite _buildingImage;

    //읽기 전용
    public Building BuildingType => _type;
    public int PowerValue => _powerValue;
    public Sprite BuildingImage => _buildingImage;

    /// <summary>
    /// 건물 활성화
    /// </summary>
    public void ActivateBuilding()
    {

    }
}
