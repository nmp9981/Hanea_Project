using System.Collections.Generic;
using UnityEngine;

public class ExchangeResources
{
    // �ڿ� ��ȯ�� �ʿ��� �ڿ����� ���� (���� ����)
    private readonly List<Resource> _allResources;

    // �����ڸ� ���� ������ �ڿ� ����Ʈ�� �޽��ϴ�.
    public ExchangeResources(List<Resource> allResources)
    {
        _allResources = allResources;
    }

    /// <summary>
    /// �ڿ� A�� B�� ��ȯ�ϴ� ����
    /// <param name="fromResourceName">��ȯ �� �ڿ� ��</param>
    /// <param name="fromAmount">��ȯ �� �ڿ� ����</param>
    /// <param name="toResourceName">��ȯ �� �ڿ� ��</param>
    /// <param name="toAmount">��ȯ �� �ڿ� ����</param>
    /// </summary>
    public bool Exchange_AToB(string fromResourceName, int fromAmount, string toResourceName, int toAmount)
    {
        // 1. �ʿ��� �ڿ��� ������� Ȯ��
        Resource fromResource = _allResources.Find(r => r.Name == fromResourceName);
        if (fromResource == null || fromResource.CurCount < fromAmount)
        {
            Debug.LogError("��ȯ�� �ʿ��� �ڿ��� �����մϴ�.");
            return false;
        }

        // 2. ��ȯ�� �ڿ� ��ü ã��
        Resource toResource = _allResources.Find(r => r.Name == toResourceName);
        if (toResource == null)
        {
            Debug.LogError("��ȯ�� ��� �ڿ��� �������� �ʽ��ϴ�.");
            return false;
        }

        // 3. �ڿ� �Һ� �� ȹ��
        fromResource.Consume(fromAmount);
        toResource.Gain(toAmount);

        return true;
    }
}
