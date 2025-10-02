using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTile : MonoBehaviour, TileInterface
{
    // Ÿ�� ���� �� ȣ��� �̺�Ʈ
    public event System.Action OnChanged;

    //�Ӽ� ����
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet = false;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();
    [SerializeField] private Button _button;
   

    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public Button Button => GetComponent<Button>();
    
    void Awake()
    {
        InitTile();
    }

    /// <summary>
    /// Ÿ�� �ʱ�ȭ
    /// </summary>
    public void InitTile()
    {
        _button = this.Button;
        _button.onClick.AddListener(GetReward);
    }

    /// <summary>
    /// Ÿ�� Ȱ��ȭ
    /// </summary>
    public void TileActive()
    {
        _isGet = false;
        _button.interactable = true;//��ư Ȱ��ȭ
    }


    /// <summary>
    /// ���� : ȹ��� �ѹ� ����
    /// </summary>
    public void GetReward()
    {
        //�� ������ ���� ���� ȹ��
        if (_rewardResourceList.Count == 0) return;
        foreach (var reward in _rewardResourceList)
        {
            switch (reward.RewardResourcesType)
            {
                case RewardResourcesType.Import://���� ����
                    ResourcesManager.Instance.ImportResourceAmount_UpDown(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.SingleUse://�ڿ� ȹ��
                    ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                    break;
                case RewardResourcesType.Score://���� ȹ��
                    if(reward.ResourceName == "Gaia")//���̾� �༺
                    {
                        PlayerManager.Instance.GetScore(3);
                    }
                    else//7��
                    {
                        PlayerManager.Instance.GetScore(reward.RewardAmount);
                    }
                    break;
                case RewardResourcesType.Etc://��Ÿ ȿ��
                    
                    break;
                default:
                    break;
            }
        }

        //�� ����
        if (_isGet == true)
        {
            OnChanged?.Invoke();
        }
    }
}
