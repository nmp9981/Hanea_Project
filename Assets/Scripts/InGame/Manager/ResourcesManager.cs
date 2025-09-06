using UnityEngine;

//�ڿ�
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
    //���� �ڿ� ����
    public int[] curResourcesCount = new int[(int)Resources.Count];
    //�ִ� ���� �ڿ� ����
    public int[] maxHaveResourcesCount = new int[(int)Resources.Count];
    //���� �ڿ� ����
    public int[] importResourcesCount = new int[(int)Resources.Count];

    private void Awake()
    {
        InitResoucesSet();
    }

    /// <summary>
    /// �ʱ� �ڿ� ����
    /// </summary>
    private void InitResoucesSet()
    {
        curResourcesCount[(int)Resources.Money] = 8;
    }


    /// <summary>
    /// �ڿ� ����
    /// �ִ� ������ ���� �ʴ¼����� ����
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
