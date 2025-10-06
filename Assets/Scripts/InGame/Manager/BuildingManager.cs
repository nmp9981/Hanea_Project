using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static BuildingManager Instance { get; private set; }

    [SerializeField]
    private List<Sprite> _buildingSpriteList;

    //�ǹ� ������
    [SerializeField]
    public List<BuildingData> buildingDataList = new();

    //�ǹ� UI
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
            // DontDestroyOnLoad(gameObject); // �ʿ�� �ּ� ����
        }
        else
        {
            Destroy(gameObject);
        }

        EnrollBuldingImage();//�ǹ� �̹��� ���
    }
   
    /// <summary>
    /// �ǹ� �̹��� ���
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
    /// �ǹ� ����
    /// ��� Ÿ�Ͽ� �ش� �ǹ��� ���´�.
    /// </summary>
    public void InstallBuiling(Tile tile, Building buildingType)
    {

    }
    /// <summary>
    /// �ǹ� ��������Ʈ ����Ʈ ��ȯ
    /// </summary>
    public Sprite GetBuildingSprite(Building buildingType)
    {
        // buildingType�� �ش��ϴ� ��������Ʈ ��ȯ
        return _buildingSpriteList[(int)buildingType];
    }
}
