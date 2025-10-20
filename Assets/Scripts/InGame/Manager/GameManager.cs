using System.Collections.Generic;
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
    GameObject gameoverObj;//���ӿ��� ������Ʈ

    [SerializeField]
    private List<RoundToken> roundTokenList = new();//��ü ���� ��ū
    private List<RoundToken> activeRoundToken = new();//�̹� ���ӿ��� ����ϴ� ���� ��ū
    [SerializeField]
    public List<BonusTrackTile> finalBonusList = new();//��ü ���ʽ� Ÿ��

    public Dictionary<RoundEffect, bool> IsRoundEffectDic = new();//�� ���� ȿ�� Ȱ��ȭ ����

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
    /// ���� ���ʽ� Ÿ�� ����
    /// </summary>
    void FinalBonusTileSetting()
    {
        //����Ʈ ���� ���� ����
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
    /// ���� ��ū ����
    /// </summary>
    void RoundTokenSetting()
    {
        //����Ʈ ���� ���� ����
        HashSet<int> orderList = TileSystem.OrderNumberList(9, 6);

        //���� ��ū ����
        Transform roundTokenArea = GameObject.Find("RoundTokenArea").GetComponent<Transform>();
        foreach (int idx in orderList) { 
            RoundToken token = roundTokenList[idx];//���� ������

            //���� ��ū ����(���纻)
            GameObject tokenGM = Instantiate(token.gameObject);
            tokenGM.GetComponent<Transform>().parent = roundTokenArea;

            //����� ��ü���� ������Ʈ ��������
            RoundToken instantiatedToken = tokenGM.GetComponent<RoundToken>();

            activeRoundToken.Add(instantiatedToken);
            IsRoundEffectDic.Add(instantiatedToken.RoundEffect, false);
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
        if(currentRound>=2) activeRoundToken[currentRound - 2].button.interactable = false;
        activeRoundToken[currentRound - 1].ActiveRoundToken();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private void GameOver()
    {
        int finalScore = CalFinalScore();//���� ���� ���
        PlayerManager.Instance.GetScore(finalScore);
        gameoverObj.SetActive(true);
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
        score += CalFinalBonusScore();
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
            if (know.Value == 5) knowCount += 4;//����5�� 4�� �߰�
        }
        return knowCount*4;
    }

    /// <summary>
    /// ���� ���ʽ� ����
    /// </summary>
    /// <returns></returns>
    private int CalFinalBonusScore()
    {
        int addScore = 0;
        foreach (var bonus in finalBonusList)
        {
            if (!bonus.IsActive) continue;//Ȱ��ȭ �Ȱ͸� ����
            int finalCount = bonus.CompleteCount;

            switch (bonus.BonusType)
            {
                case BonusTrackType.GaiaDiemnsion://���� 3��
                    addScore = finalCount * 3;
                    break;
                case BonusTrackType.PlanetKind://���� 3��
                    addScore = finalCount * 3;
                    break;
                case BonusTrackType.Sattlate://2���� 3��
                    addScore = (finalCount/2) * 3;
                    break;
                case BonusTrackType.UnionBuilding://2���� 3��
                    addScore = (finalCount/2) * 3;
                    break;
                case BonusTrackType.BuildingCount://���� 2��
                    addScore = finalCount * 2;
                    break;
                default:
                    break;
            }
        }
        return addScore;
    }
}
