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

    private void Awake()
    {
        SingletonObjectLoad();
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
