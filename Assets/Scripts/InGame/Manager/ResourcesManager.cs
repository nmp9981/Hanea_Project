using System.Collections.Generic;
using UnityEngine;

//�ڿ� �������̽� ����
public interface IResource
{
    string Name { get; }//�̸�
    int CurCount { get; }//���� ���� ����
    int MaxCount { get; }//�ִ� ���� ����
    int ImportAmount { get; }//���Է�

    void Gain(int amount);//�ڿ� ȹ�� 
    void Consume(int amount);//�ڿ� �Һ�
    void Import();//�ڿ� ����
}

// 2. �ڿ� Ŭ���� ����
// �� �ڿ��� Ŭ������ ����� �������̽��� �����մϴ�.
[System.Serializable]
public class Resource : IResource
{
    //�Ӽ� ����
    [SerializeField] private string _name;
    [SerializeField] private int _curCount;
    [SerializeField] private int _maxCount;
    [SerializeField] private int _importAmount;

    //�б� ����
    public string Name => _name;
    public int CurCount => _curCount;
    public int MaxCount => _maxCount;
    public int ImportAmount => _importAmount;

    //�ڿ� ȹ��
    public void Gain(int amount)
    {
        _curCount = Mathf.Min(_curCount + amount, _maxCount);
    }

    //�ڿ� �Һ�
    public void Consume(int amount)
    {
        _curCount = Mathf.Max(_curCount - amount, 0);
    }
    //�ڿ� ����
    public void Import()
    {
        Gain(_importAmount);
    }
}

/// <summary>
/// �ڿ� ����
/// </summary>
public class ResourcesManager : MonoBehaviour
{
    // �������̽� Ÿ�� ����Ʈ�� �ڿ��� �����Ͽ� �������� Ȯ���մϴ�.
    [SerializeField] private List<Resource> resources = new List<Resource>();

    /// <summary>
    /// �ʱ� �ڿ� ����
    /// </summary>
    public void InitResourcesSet()
    {
        // �ʱ� ���� ���� (��: �ν����Ϳ��� ������ �� ���)
        // �Ǵ� Ư�� �ڿ��� �ʱ�ȭ
        //��
        Resource money = resources.Find(r => r.Name == "Money");
        if (money != null) money.Gain(8);
        //����
        Resource ore = resources.Find(r => r.Name == "Ore");
        if (ore != null) { ore.Gain(4); }
    }

    //�ڿ� ����
    public void ImportAllResources()
    {
        foreach (IResource resource in resources)
        {
            resource.Import();
        }
    }
}
