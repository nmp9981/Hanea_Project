using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTile : MonoBehaviour, TileInterface
{
    // Ÿ�� ���� �� ȣ��� �̺�Ʈ
    public event System.Action OnChanged;

    //�Ӽ� ����
    [SerializeField] private TileType _type;
    [SerializeField] private bool _isGet;
    [SerializeField] private List<RewardResource> _rewardResourceList = new();
    [SerializeField] private Button _button;
    [SerializeField] private int _costAmount;
    private string _costResource;
    
    //�б� ����
    public TileType Type => _type;
    public bool IsGet => _isGet;
    public List<RewardResource> RewardResourcesList => _rewardResourceList;
    public Button Button => GetComponent<Button>();
    public int CostAmount => _costAmount;

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
        TileActive();//Ÿ�� Ȱ��ȭ
        SetCostResource();//���� �ڿ� ����

        _button.onClick.AddListener(ClickKnowLedgeTile);
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
    /// ���� �ڿ� ����
    /// </summary>
    void SetCostResource()
    {
        switch (_type)
        {
            case TileType.Energy:
                _costResource = "Energy";
                break;
            case TileType.Information:
                _costResource = "Quantum Intelligence Cube";
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// ����
    /// </summary>
    public void GetReward()
    {
        //����ť�� �׼����� ������ �׼������� ���� ����
        if (_type == TileType.Energy)
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
                        if(reward.ResourceName == "Energy")//�������� 1�� ����
                        {
                            PlayerManager.Instance.GetScore(-1);
                        }
                        ResourcesManager.Instance.GainResource(reward.ResourceName, reward.RewardAmount);
                        break;
                    case RewardResourcesType.Etc://��Ÿ ȿ��
                                                 //���� �� �ִ°� ǥ��
                        break;
                    default:
                        break;
                }
            }
        }
        else if (_type == TileType.Information)//����ť�� �׼�
        {
            switch(_costAmount)
            {
                case 4://Ÿ�� ��������
                    KnowledgeBoard_Manager.Instance.Activate_AllSkillTile();
                    break;
                case 3://���� ȿ�� ����
                    break;
                case 2://����
                    int addScore = 3 + TileSystem.CountOccupyPlanet();
                    PlayerManager.Instance.GetScore(addScore);
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

    /// <summary>
    /// ���� Ÿ���� ����
    /// </summary>
    public void ClickKnowLedgeTile()
    {
        //�ڿ��� �� �� �ִ°�?
        if (ResourcesManager.Instance.HasEnoughResources(_costResource, _costAmount))
        {
            ResourcesManager.Instance.ConsumeResource(_costResource, _costAmount);
        }else return;
        
        //���� �ڿ� ȹ��
        GetReward();

        //��ư ��Ȱ��ȭ (�� ���� ���� 1ȸ��)
        _button.interactable = false;
        _isGet = true;
    }
}
