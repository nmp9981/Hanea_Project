using System.Collections.Generic;
using UnityEngine;

//자원 인터페이스 정의
public interface IResource
{
    string Name { get; }//이름
    int CurCount { get; }//현재 보유 개수
    int MaxCount { get; }//최대 보유 개수
    int ImportAmount { get; }//수입량

    void Gain(int amount);//자원 획득 
    void Consume(int amount);//자원 소비
    void Import();//자원 수입
}

// 2. 자원 클래스 구현
// 각 자원을 클래스로 만들고 인터페이스를 구현합니다.
[System.Serializable]
public class Resource : IResource
{
    // 자원 변경 시 호출될 이벤트
    public event System.Action OnChanged;

    //속성 정의
    [SerializeField] private string _name;
    [SerializeField] private int _curCount;
    [SerializeField] private int _maxCount;
    [SerializeField] private int _importAmount;

    //읽기 전용
    public string Name => _name;
    public int CurCount => _curCount;
    public int MaxCount => _maxCount;
    public int ImportAmount => _importAmount;

    //자원 획득
    public void Gain(int amount)
    {
        int prevCount = _curCount;
        _curCount = Mathf.Min(_curCount + amount, _maxCount);

        //값 변경
        if (_curCount != prevCount)
        {
            OnChanged?.Invoke();
        }
    }

    //자원 소비
    public void Consume(int amount)
    {
        int prevCount = _curCount;
        _curCount = Mathf.Max(_curCount - amount, 0);

        //값 변경
        if (_curCount != prevCount)
        {
            OnChanged?.Invoke();
        }
    }
    //자원 수입
    public void Import()
    {
        int prevCount = _curCount;
        Gain(_importAmount);

        //값 변경
        if (_curCount != prevCount)
        {
            OnChanged?.Invoke();
        }
    }
    //자원 수입량 증감
    public void ImportAmount_Change(int amount)
    {
        int prevCount = _importAmount;
        //수입량이 0미만으로 떨어지지 않음
        _importAmount = Mathf.Max(_importAmount + amount, 0);

        //값 변경
        if (_importAmount != prevCount)
        {
            OnChanged?.Invoke();
        }
    }
}

/// <summary>
/// 자원 관리
/// </summary>
public class ResourcesManager : MonoBehaviour
{
    // 인터페이스 타입 리스트로 자원을 관리하여 유연성을 확보합니다.
    [SerializeField] public List<Resource> resources = new List<Resource>();
   
    /// <summary>
    /// 초기 자원 설정
    /// </summary>
    public void InitResourcesSet()
    {
        // 초기 설정 로직 (예: 인스펙터에서 설정된 값 사용)
        // 또는 특정 자원만 초기화
        //돈
        Resource money = resources.Find(r => r.Name == "Money");
        if (money != null) money.Gain(8);
        //광석
        Resource ore = resources.Find(r => r.Name == "Ore");
        if (ore != null) { ore.Gain(4); }
    }

    //자원 수입
    public void ImportAllResources()
    {
        foreach (IResource resource in resources)
        {
            resource.Import();
        }
    }

    //자원 수입량 증감
    public void ImportResourceAmount_UpDown(string resourceName, int increaseAmount)
    {
        Resource res = resources.Find(r=>r.Name == resourceName);
        if (res != null)
        {
            res.ImportAmount_Change(increaseAmount);
        }
    }
}
