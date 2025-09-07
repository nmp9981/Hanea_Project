using System.Collections.Generic;
using UnityEngine;

public class ExchangeResources
{
    // 자원 교환에 필요한 자원들을 참조 (구성 관계)
    private readonly List<Resource> _allResources;

    // 생성자를 통해 관리할 자원 리스트를 받습니다.
    public ExchangeResources(List<Resource> allResources)
    {
        _allResources = allResources;
    }

    /// <summary>
    /// 자원 A를 B로 교환하는 로직
    /// <param name="fromResourceName">변환 전 자원 명</param>
    /// <param name="fromAmount">변환 전 자원 개수</param>
    /// <param name="toResourceName">변환 후 자원 명</param>
    /// <param name="toAmount">변환 후 자원 개수</param>
    /// </summary>
    public bool Exchange_AToB(string fromResourceName, int fromAmount, string toResourceName, int toAmount)
    {
        // 1. 필요한 자원이 충분한지 확인
        Resource fromResource = _allResources.Find(r => r.Name == fromResourceName);
        if (fromResource == null || fromResource.CurCount < fromAmount)
        {
            Debug.LogError("교환에 필요한 자원이 부족합니다.");
            return false;
        }

        // 2. 교환할 자원 객체 찾기
        Resource toResource = _allResources.Find(r => r.Name == toResourceName);
        if (toResource == null)
        {
            Debug.LogError("교환할 대상 자원이 존재하지 않습니다.");
            return false;
        }

        // 3. 자원 소비 및 획득
        fromResource.Consume(fromAmount);
        toResource.Gain(toAmount);

        return true;
    }
}
