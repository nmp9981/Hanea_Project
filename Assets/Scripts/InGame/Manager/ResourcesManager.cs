using UnityEngine;

//자원
public enum Resources
{
    Money,
    Energy,
    Stone,
    Knowledge,
    Plants,
    Count
}

public class ResourcesManager : MonoBehaviour
{
    //보유 자원 개수
    public int[] curResourcesCount = new int[(int)Resources.Count];
    //최대 보유 자원 개수
    public int[] maxHaveResourcesCount = new int[(int)Resources.Count];
    //수입 자원 개수
    public int[] importResourcesCount = new int[(int)Resources.Count];

    private void Awake()
    {
        InitResoucesSet();
    }

    /// <summary>
    /// 초기 자원 설정
    /// </summary>
    private void InitResoucesSet()
    {
        curResourcesCount[(int)Resources.Money] = 8;
    }


    /// <summary>
    /// 자원 수입
    /// 최대 개수를 넘지 않는선에서 수입
    /// </summary>
    public void ImportResource()
    {
        int resourceKindCount = (int)Resources.Count;
        for (int i = 0; i < resourceKindCount; i++)
        {
            curResourcesCount[i] += importResourcesCount[i];
            curResourcesCount[i] = Mathf.Min(curResourcesCount[i], maxHaveResourcesCount[i]);
        }
    }
}
