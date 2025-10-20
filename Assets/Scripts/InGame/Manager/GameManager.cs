using System.Collections.Generic;
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
    GameObject gameoverObj;//게임오버 오브젝트

    [SerializeField]
    private List<RoundToken> roundTokenList = new();//전체 라운드 토큰
    private List<RoundToken> activeRoundToken = new();//이번 게임에서 사용하는 라운드 토큰
    [SerializeField]
    public List<BonusTrackTile> finalBonusList = new();//전체 보너스 타일

    public Dictionary<RoundEffect, bool> IsRoundEffectDic = new();//각 라운드 효과 활성화 여부

    private void Awake()
    {
        SingletonObjectLoad();
    }

    private void Start()
    {
        RoundTokenSetting();
        FinalBonusTileSetting();
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
    /// 최종 보너스 타일 세팅
    /// </summary>
    void FinalBonusTileSetting()
    {
        //리스트 순서 랜덤 지정
        HashSet<int> orderList = TileSystem.OrderNumberList(5, 2);

        foreach (BonusTrackTile gm in finalBonusList)
        {
            gm.gameObject.SetActive(false);
            gm.IsActive = false;
        }
        foreach (int idx in orderList)
        {
            finalBonusList[idx].gameObject.SetActive(true);
            finalBonusList[idx].IsActive = true;
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
            RoundToken token = roundTokenList[idx];//원본 프리팹

            //라운드 토큰 생성(복사본)
            GameObject tokenGM = Instantiate(token.gameObject);
            tokenGM.GetComponent<Transform>().parent = roundTokenArea;

            //복사된 객체에서 컴포넌트 가져오기
            RoundToken instantiatedToken = tokenGM.GetComponent<RoundToken>();

            activeRoundToken.Add(instantiatedToken);
            IsRoundEffectDic.Add(instantiatedToken.RoundEffect, false);
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
        if(currentRound>=2) activeRoundToken[currentRound - 2].button.interactable = false;
        activeRoundToken[currentRound - 1].ActiveRoundToken();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    private void GameOver()
    {
        int finalScore = CalFinalScore();//최종 점수 계산
        PlayerManager.Instance.GetScore(finalScore);
        gameoverObj.SetActive(true);
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
        score += CalFinalBonusScore();
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
            if (know.Value == 5) knowCount += 4;//레벨5는 4점 추가
        }
        return knowCount*4;
    }

    /// <summary>
    /// 최종 보너스 점수
    /// </summary>
    /// <returns></returns>
    private int CalFinalBonusScore()
    {
        int addScore = 0;
        foreach (var bonus in finalBonusList)
        {
            if (!bonus.IsActive) continue;//활성화 된것만 적용
            int finalCount = bonus.CompleteCount;

            switch (bonus.BonusType)
            {
                case BonusTrackType.GaiaDiemnsion://개당 3점
                    addScore = finalCount * 3;
                    break;
                case BonusTrackType.PlanetKind://개당 3점
                    addScore = finalCount * 3;
                    break;
                case BonusTrackType.Sattlate://2개당 3점
                    addScore = (finalCount/2) * 3;
                    break;
                case BonusTrackType.UnionBuilding://2개당 3점
                    addScore = (finalCount/2) * 3;
                    break;
                case BonusTrackType.BuildingCount://개당 2점
                    addScore = finalCount * 2;
                    break;
                default:
                    break;
            }
        }
        return addScore;
    }
}
