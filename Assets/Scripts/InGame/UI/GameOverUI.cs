using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        exitButton.onClick.AddListener(GameExit);
    }

    private void OnEnable()
    {
        finalScoreText.text = $"{PlayerManager.Instance.FinalScore()}";
    }
   
    /// <summary>
    /// 게임 나가기
    /// </summary>
    void GameExit()
    {
#if UNITY_EDITOR //에디터에서
        UnityEditor.EditorApplication.isPlaying = false;
#else //나머지
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
