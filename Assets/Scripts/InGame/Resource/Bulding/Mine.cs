using UnityEngine;

public class Mine : IBuilding
{
    //�Ӽ� ����
    [SerializeField] private Building _type = Building.Mine;
    [SerializeField] private int _powerValue = 1;
    [SerializeField] private Sprite _buildingImage;

    //�б� ����
    public Building BuildingType => _type;
    public int PowerValue => _powerValue;
    public Sprite BuildingImage => _buildingImage;

    /// <summary>
    /// �ǹ� Ȱ��ȭ
    /// </summary>
    public void ActivateBuilding()
    {

    }
}
