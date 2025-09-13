using UnityEngine;

public class Mine : IBuilding
{
    //속성 정의
    [SerializeField] private Building _type = Building.Mine;
    [SerializeField] private int _powerValue = 1;
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
