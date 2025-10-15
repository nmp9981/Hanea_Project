using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public enum BonusTrackType
{
    GaiaDiemnsion,
    PlanetKind,
    Sattlate,
    UnionBuilding,
    BuildingCount,
    Count
}

public class BonusTrackTile : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _countText;
    [SerializeField]
    private BonusTrackType _bonusType;
    private int _completeCount;
    private bool _isActive;
    public BonusTrackType BonusType { get { return _bonusType; } set { _bonusType = value; } }
    public int CompleteCount { get { return _completeCount; }set { _completeCount = value; } }
    public bool IsActive { get { return _isActive; } set { _isActive = value; } }

    private void OnEnable()
    {
        InitCount();
    }

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    void InitCount()
    {
        _completeCount = 0;
        _countText.text = $"{_completeCount}";
    }

    /// <summary>
    /// �Ϸ� ���� ����
    /// </summary>
    public void CountUP()
    {
        //Ȱ��ȭ ���� �˻�
        if (!_isActive) return;

        _completeCount += 1;
        _countText.text = $"{_completeCount}";
    }
}
