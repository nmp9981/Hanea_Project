using UnityEngine;

public class BuildingComponent : MonoBehaviour, IBuilding
{
    // ✅ 건물 데이터 에셋을 참조
    public BuildingData buildingData;

    // IBuilding 인터페이스 구현
    public void ActivateBuilding()
    {
        if (buildingData != null)
        {
            // buildingData.powerValue, buildingData.type 등을 활용하여 로직 실행
            Debug.Log($"{buildingData.type} 건물을 활성화합니다. 파워: {buildingData.powerValue}");
        }
    }
    public void DeactivateBuilding()
    {
        if (buildingData != null)
        {
            // buildingData.powerValue, buildingData.type 등을 활용하여 로직 실행
            Debug.Log($"{buildingData.type} 건물을 활성화합니다. 파워: {buildingData.powerValue}");
        }
    }
}
