using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BuildingManager Instance { get; private set; }

    [SerializeField]
    private List<Sprite> _buildingSpriteList;

    //건물 데이터
    [SerializeField]
    public List<BuildingData> buildingDataList = new();

    //건물 UI
    [SerializeField]
    private List<Image> mineImage_UIList = new();
    public Stack<Image> mineImage_UIStack = new();
    public Image last_mineImage;
    [SerializeField]
    private List<Image> tradingStation_UIList = new();
    public Stack<Image> tradingStation_UIStack = new();
    public Image last_tradingStationImage;
    [SerializeField]
    private List<Image> researchLab_UIList = new();
    public Stack<Image> researchLab_UIStack = new();
    public Image researchLabImage;
    [SerializeField]
    private List<Image> academy_UIList = new();
    public Stack<Image> academy_UIStack = new();
    [SerializeField]
    private List<Image> institute_UIList = new();
    public Stack<Image> institute_UIStack = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요시 주석 해제
        }
        else
        {
            Destroy(gameObject);
        }

        EnrollBuldingImage();//건물 이미지 등록
    }
   
    /// <summary>
    /// 건물 이미지 등록
    /// </summary>
    private void EnrollBuldingImage()
    {
        foreach(var image in mineImage_UIList)
        {
            mineImage_UIStack.Push(image);
        }
        foreach (var image in tradingStation_UIList)
        {
            tradingStation_UIStack.Push(image);
        }
        foreach (var image in researchLab_UIList)
        {
            researchLab_UIStack.Push(image);
        }
        foreach (var image in academy_UIList)
        {
            academy_UIStack.Push(image);
        }
        foreach (var image in institute_UIList)
        {
            institute_UIStack.Push(image);
        }
    }
    /// <summary>
    /// 건물 짓기
    /// 어느 타일에 해당 건물을 짓는다.
    /// </summary>
    public void InstallBuiling(Tile tile, Building buildingType)
    {

    }
    /// <summary>
    /// 건물 스프라이트 리스트 반환
    /// </summary>
    public Sprite GetBuildingSprite(Building buildingType)
    {
        // buildingType에 해당하는 스프라이트 반환
        return _buildingSpriteList[(int)buildingType];
    }
}
