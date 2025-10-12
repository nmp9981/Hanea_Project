using NUnit.Framework;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    //라운드
    public int currentRound = 0;
    [SerializeField]
    TextMeshProUGUI roundText;

    [SerializeField]
    private List<RoundToken> roundTokenList = new();//전체 라운드 토큰
    private List<RoundToken> activeRoundToken = new();//이번 게임에서 사용하는 라운드 토큰

    public Dictionary<RoundEffect, bool> IsRoundEffectDic = new();//각 라운드 효과 활성화 여부

    private void Awake()
    {
        SingletonObjectLoad();
        RoundTokenSetting();
        ShowRoundText();
    }

    /// <summary>
    /// 싱글톤
    /// </summary>
    void SingletonObjectLoad()
    {
        if (Instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            Instance = this; //내자신을 instance로 넣어줍니다.
            DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            if (Instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }

    /// <summary>
    /// 라운드 토큰 세팅
    /// </summary>
    void RoundTokenSetting()
    {
        //리스트 순서 랜덤 지정
        HashSet<int> orderList = TileSystem.OrderNumberList(9, 6);

        //라운드 토큰 세팅
        Transform roundTokenArea = GameObject.Find("RoundTokenArea").GetComponent<Transform>();
        foreach (int idx in orderList) { 
            RoundToken token = roundTokenList[idx];
            activeRoundToken.Add(token);
            IsRoundEffectDic.Add(token.RoundEffect,false);

            //라운드 토큰 생성
            GameObject tokenGM = Instantiate(token.gameObject);
            tokenGM.GetComponent<Transform>().parent = roundTokenArea;
        }
    }

    /// <summary>
    /// 다음 라운드 전환
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
        //라운드 토큰 초기화 및 활성화
        activeRoundToken[currentRound - 1].ActiveRoundToken();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    private void GameOver()
    {
        int finalScore = CalFinalScore();//최종 점수 계산
    }

    /// <summary>
    /// 최종 점수 계산
    /// </summary>
    private int CalFinalScore()
    {
        int score = 0;
        //지식 타일 점수
        score += CalKnowledgeTileScore();
        //최종 목표 계산
        //자원 점수 계산
        score += CalResourcesScore();
        return score;
    }

    /// <summary>
    /// 남은 자원 점수 계산
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
    /// 지식 타일 점수 계산
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
    /// 라운드 보너스 점수 추가
    /// </summary>
    public void AddRoundBonusScore()
    {

        PlayerManager.Instance.GetScore(5);
    }
}
