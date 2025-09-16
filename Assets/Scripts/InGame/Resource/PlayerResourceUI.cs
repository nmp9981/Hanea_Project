using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ���� �ؽ�Ʈ ����
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
                // ���� ���� �� �̺�Ʈ �߻�
                OnScoreChanged?.Invoke(_score);
            }
        }
    }
}

public class PlayerResourceUI : MonoBehaviour
{
    // �÷��̾�� �ڿ� �����ڸ� ������ �ִ� (has-a)
    public ResourcesManager resourcesManager;

    [SerializeField] private TMP_Text[] resourceTexts;
    [SerializeField] private TMP_Text[] resourceImportTexts;

    private Dictionary<string, TMP_Text> resourceTextMap = new Dictionary<string, TMP_Text>();
    private Dictionary<string, TMP_Text> importTextMap = new Dictionary<string, TMP_Text>();

    private void Awake()
    {
        // ��ųʸ� �ʱ�ȭ
        for (int i = 0; i < resourcesManager.resources.Count; i++)
        {
            string resName = resourcesManager.resources[i].Name;
            resourceTextMap.Add(resName, resourceTexts[i]);
            importTextMap.Add(resName, resourceImportTexts[i]);
        }
    }

    private void OnEnable()
    {
        // �� �ڿ��� �̺�Ʈ�� �Լ� ����
        foreach (var res in resourcesManager.resources)
        {
            res.OnChanged += () => ShowPlayerCurResource(res);
            res.OnChanged += () => ShowPlayerCurImportResource(res);
        }
    }

    private void OnDisable()
    {
        // �̺�Ʈ ���� ����
        foreach (var res in resourcesManager.resources)
        {
            res.OnChanged -= () => ShowPlayerCurResource(res);
            res.OnChanged -= () => ShowPlayerCurImportResource(res);
        }
    }

    /// <summary>
    /// �÷��̾ ���� �ڿ� ��Ȳ ���̱�
    /// </summary>
    public void ShowPlayerCurResource(Resource res)
    {
        if (resourceTextMap.ContainsKey(res.Name))
        {
            resourceTextMap[res.Name].text = res.CurCount.ToString();
        }
    }
    /// <summary>
    /// �÷��̾ ���� ���� �ڿ� ��Ȳ ���̱�
    /// </summary>
    public void ShowPlayerCurImportResource(Resource res)
    {
        if (importTextMap.ContainsKey(res.Name))
        {
            importTextMap[res.Name].text = "+"+res.ImportAmount.ToString();
        }
    }
}
