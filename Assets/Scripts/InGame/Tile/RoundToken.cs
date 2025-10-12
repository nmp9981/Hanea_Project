using System.Linq;
using UnityEngine;

public enum RoundEffect
{
    Mine,
    TradingI,
    TragindII,
    Power3,
    GaiaMineI,
    GaiaMineII,
    Union,
    Terafoming,
    Knowledge,
    Count
}

public class RoundToken : MonoBehaviour
{
    [SerializeField]
    private RoundEffect roundEffect;

    //읽기 전용
    public RoundEffect RoundEffect => roundEffect;

    /// <summary>
    /// 라운드 토큰 활성화
    /// </summary>
    public void ActiveRoundToken()
    {
        // 1. 모든 키의 값을 false로 초기화 (또는 원하는 기본값으로)
        var keys = GameManager.Instance.IsRoundEffectDic.Keys.ToList(); // 키 목록을 복사
        foreach (var key in keys)
        {
            GameManager.Instance.IsRoundEffectDic[key] = false;
        }

        // 2. 원하는 특정 키의 값만 true로 설정
        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(roundEffect))
        {
            GameManager.Instance.IsRoundEffectDic[roundEffect] = true; // ✅ 루프 없이 특정 값만 설정
        }
    }
}
