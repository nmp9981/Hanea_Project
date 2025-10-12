using NUnit.Framework;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    //����
    public int currentRound = 0;
    [SerializeField]
    TextMeshProUGUI roundText;

    [SerializeField]
    private List<RoundToken> roundTokenList = new();//��ü ���� ��ū
    private List<RoundToken> activeRoundToken = new();//�̹� ���ӿ��� ����ϴ� ���� ��ū

    public Dictionary<RoundEffect, bool> IsRoundEffectDic = new();//�� ���� ȿ�� Ȱ��ȭ ����

    private void Awake()
    {
        SingletonObjectLoad();
        RoundTokenSetting();
        ShowRoundText();
    }

    /// <summary>
    /// �̱���
    /// </summary>
    void SingletonObjectLoad()
    {
        if (Instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            Instance = this; //���ڽ��� instance�� �־��ݴϴ�.
            DontDestroyOnLoad(gameObject); //OnLoad(���� �ε� �Ǿ�����) �ڽ��� �ı����� �ʰ� ����
        }
        else
        {
            if (Instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }

    /// <summary>
    /// ���� ��ū ����
    /// </summary>
    void RoundTokenSetting()
    {
        //����Ʈ ���� ���� ����
        HashSet<int> orderList = TileSystem.OrderNumberList(9, 6);

        //���� ��ū ����
        Transform roundTokenArea = GameObject.Find("RoundTokenArea").GetComponent<Transform>();
        foreach (int idx in orderList) { 
            RoundToken token = roundTokenList[idx];
            activeRoundToken.Add(token);
            IsRoundEffectDic.Add(token.RoundEffect,false);

            //���� ��ū ����
            GameObject tokenGM = Instantiate(token.gameObject);
            tokenGM.GetComponent<Transform>().parent = roundTokenArea;
        }
    }

    /// <summary>
    /// ���� ���� ��ȯ
    /// </summary>
    public void ShowRoundText()
    {
        if (currentRound == 6)
        {
            GameOver();
            return;
        }

        currentRound += 1;
        roundText.text = $"{currentRound}";
        //���� ��ū �ʱ�ȭ �� Ȱ��ȭ
        activeRoundToken[currentRound - 1].ActiveRoundToken();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private void GameOver()
    {
        int finalScore = CalFinalScore();//���� ���� ���
    }

    /// <summary>
    /// ���� ���� ���
    /// </summary>
    private int CalFinalScore()
    {
        int score = 0;
        //���� Ÿ�� ����
        score += CalKnowledgeTileScore();
        //���� ��ǥ ���
        //�ڿ� ���� ���
        score += CalResourcesScore();
        return score;
    }

    /// <summary>
    /// ���� �ڿ� ���� ���
    /// </summary>
    /// <returns></returns>
    private int CalResourcesScore()
    {
        int restResourceCount = 0;
        foreach(Resource resource in ResourcesManager.Instance.resources)
        {
            if(resource.Name == "Money" || resource.Name == "Ore" || resource.Name == "Knowledge")
            {
                restResourceCount += resource.CurCount;
            }
        }
        return restResourceCount/3;
    }

    /// <summary>
    /// ���� Ÿ�� ���� ���
    /// </summary>
    /// <returns></returns>
    private int CalKnowledgeTileScore()
    {
        int knowCount = 0;
        foreach(var know in KnowledgeBoard_Manager.Instance.playerKnowledgeLevel)
        {
            knowCount += Mathf.Max(0, know.Value-2);
        }
        return knowCount*4;
    }

    /// <summary>
    /// ���� ���ʽ� ���� �߰�
    /// </summary>
    public void AddRoundBonusScore()
    {

        PlayerManager.Instance.GetScore(5);
    }
}
