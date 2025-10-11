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

    private void Awake()
    {
        SingletonObjectLoad();
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
