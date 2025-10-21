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
    /// ���� ������
    /// </summary>
    void GameExit()
    {
#if UNITY_EDITOR //�����Ϳ���
        UnityEditor.EditorApplication.isPlaying = false;
#else //������
        Application.Quit(); // ���ø����̼� ����
#endif
    }
}
