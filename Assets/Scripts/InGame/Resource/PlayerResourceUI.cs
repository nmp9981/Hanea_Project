using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 점수 텍스트 관리
/// </summary>
public class ScoreTextManager : MonoBehaviour
{
    public static event Action<int> OnScoreChanged;

    private int _score;
    public int Score
    {
        get { return _score; }
        set
        {
            if (_score != value)
            {
                _score = value;
                // 점수 변경 시 이벤트 발생
                OnScoreChanged?.Invoke(_score);
            }
        }
    }
}

public class PlayerResourceUI : MonoBehaviour
{
    // 플레이어는 자원 관리자를 가지고 있다 (has-a)
    public ResourcesManager resourcesManager;

    [SerializeField] private TMP_Text[] resourceTexts;
    [SerializeField] private TMP_Text[] resourceImportTexts;

    private Dictionary<string, TMP_Text> resourceTextMap = new Dictionary<string, TMP_Text>();
    private Dictionary<string, TMP_Text> importTextMap = new Dictionary<string, TMP_Text>();

    private void Awake()
    {
        // 딕셔너리 초기화
        for (int i = 0; i < resourcesManager.resources.Count; i++)
        {
            string resName = resourcesManager.resources[i].Name;
            resourceTextMap.Add(resName, resourceTexts[i]);
            importTextMap.Add(resName, resourceImportTexts[i]);
        }
    }

    private void OnEnable()
    {
        // 각 자원의 이벤트에 함수 구독
        foreach (var res in resourcesManager.resources)
        {
            res.OnChanged += () => ShowPlayerCurResource(res);
            res.OnChanged += () => ShowPlayerCurImportResource(res);
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        foreach (var res in resourcesManager.resources)
        {
            res.OnChanged -= () => ShowPlayerCurResource(res);
            res.OnChanged -= () => ShowPlayerCurImportResource(res);
        }
    }

    /// <summary>
    /// 플레이어가 가진 자원 현황 보이기
    /// </summary>
    public void ShowPlayerCurResource(Resource res)
    {
        if (resourceTextMap.ContainsKey(res.Name))
        {
            resourceTextMap[res.Name].text = res.CurCount.ToString();
        }
    }
    /// <summary>
    /// 플레이어가 가진 수입 자원 현황 보이기
    /// </summary>
    public void ShowPlayerCurImportResource(Resource res)
    {
        if (importTextMap.ContainsKey(res.Name))
        {
            importTextMap[res.Name].text = "+"+res.ImportAmount.ToString();
        }
    }
}
