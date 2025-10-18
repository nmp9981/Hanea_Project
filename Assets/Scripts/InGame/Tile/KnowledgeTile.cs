using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ÿ�� Ÿ��
/// </summary>
public enum TileType
{
    Knowledge,
    Skill,
    Energy,
    Information,
    Count
}


/// <summary>
/// Ÿ�� �������̽� ����
/// </summary>
public interface TileInterface
{
    //����
    TileType Type { get; }//Ÿ�� ����

    bool IsGet { get; }//���� ����

    List<RewardResource> RewardResourcesList { get; }//�ڿ� ����

    //����
    void GetReward();
}

[System.Serializable]
public class RewardResource
{
    public RewardResourcesType RewardResourcesType;//�ڿ� ���� ����
    public string ResourceName;//���� �ڿ���
    public int RewardAmount;//����
}

public class KnowledgeTile : MonoBehaviour, TileInterface
{
    // Ÿ�� ���� �� ȣ��� �̺�Ʈ
    public event System.Action OnChanged;

    //�Ӽ� ����
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();

    [SerializeField] private int _level;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _button;
    [SerializeField] private TileData _tileData;

    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public int Level => _level;
    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Button Button => GetComponent<Button>();
    public TileData TileData => _tileData;

    /// <summary>
    /// Ÿ�� �ʱ�ȭ
    /// </summary>
    public void InitTile()
    {
        _isGet = false;

        if(_level==0) _isGet = true;

        _rectTransform = this.RectTransform;
        _button = this.Button;
        _button.interactable = false;//��ư ��Ȱ��ȭ

        _button.onClick.AddListener(ClickKnowLedgeTile);
    }

    /// <summary>
    /// ����
    /// </summary>
    public void GetReward()
    {
        //2->3 �Ŀ� ����
        if (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] == 3)
        {
            ResourcesManager.Instance.GainResource("Energy", 3);
        }

        //�� ������ ���� ���� ȹ��
        if (_rewardResourceList.Count != 0)
        {
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
                    case RewardResourcesType.Etc://��Ÿ ȿ��
                        EtcRewardEffect(_tileData.researchType);
                        break;
                    default:
                        break;
                }
            }
        }
        
        //���� 5 ���ܻ��� : ����5 ���� �� ���԰���
        if (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] == 5)
        {
            PlayerManager.Instance.RestUnionCount -= 1;//���� ��ū �Һ�
            //�׶����� : ���� ȹ��
            if (this.TileData.researchType == ResearchType.Terraforming)
            {
                KnowledgeBoard_Manager.Instance._lv5_UnionTile.GetReward();
                PlayerManager.Instance.RestUnionCount += 1;//���� ���� ���� 1 ����
            }
            //��Ÿ� : ���� �༺ ��ġ
            if (this.TileData.researchType == ResearchType.Navigation)
            {
                BlackPlanet.AbleInstallBlackPlanet();
            }
            //����
            if (this.TileData.researchType == ResearchType.Economy)
            {
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Ore", -2);
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Money", -4);
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Energy", -4);
            }
            //����
            if (this.TileData.researchType == ResearchType.Science)
            {
                ResourcesManager.Instance.ImportResourceAmount_UpDown("Knowledge", -4);
            }
        }
        //���� ���ʽ�
        if (GameManager.Instance.IsRoundEffectDic.ContainsKey(RoundEffect.Knowledge))
        {
            if (GameManager.Instance.IsRoundEffectDic[RoundEffect.Knowledge] == true)
            {
                PlayerManager.Instance.GetScore(2);
            }
        }

        //�� ����
        if (_isGet==true)
        {
            OnChanged?.Invoke();
        }
    }

    /// <summary>
    /// ��Ÿ ���� ȿ��
    /// </summary>
    private void EtcRewardEffect(ResearchType tileType)
    {
        switch (tileType)
        {
            case ResearchType.Terraforming:
                PlayerManager.Instance.AddOrePrice -= 1;
                break;
            case ResearchType.Navigation:
                PlayerManager.Instance.DistanceLimit += 1;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ���� Ÿ���� ����
    /// </summary>
    public void ClickKnowLedgeTile()
    {
        //���� 4�Ҹ� : ���� ��ư�� ���� ��츸
        if(KnowledgeBoard_Manager.Instance.IsPushButton)
            ResourcesManager.Instance.ConsumeResource("Knowledge", 4);

        //���� 5�� ���� ���ؼ��� ���� ���� ��ū 1���̻� �ʿ�
        if (KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] == 4)
        {
            if (PlayerManager.Instance.RestUnionCount < 1) return;
        }

        //���� Ʈ�� �̵�
        KnowledgeBoard_Manager.Instance.stateObjDic[this.TileData.researchType].GetComponent<RectTransform>().position 
            = this._rectTransform.position+ this.RectTransform.sizeDelta.y * Vector3.up;
        
        //�÷��̾��� ���� ���� ��ȭ
        KnowledgeBoard_Manager.Instance.playerKnowledgeLevel[this.TileData.researchType] += 1;

        //���� �ڿ� ȹ��
        GetReward();
        
        //2�� ȹ��
        PlayerManager.Instance.GetScore(2);
        //�ٽ� �������
        KnowledgeBoard_Manager.Instance.UnActivateKnowledgeTile();
        KnowledgeBoard_Manager.Instance.IsPushButton = false;
    }
}
