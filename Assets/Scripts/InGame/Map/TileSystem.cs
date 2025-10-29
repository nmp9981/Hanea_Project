using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class TileSystem
{
    public static float root3 = 1.732f;

    //6���� Ž��
    private static int[] dx = {0,1,1,0,-1,-1 };
    private static int[] dy = { -1, -1, 0, 1, 1, 0 };
    private static int[] dz = { 1, 0, -1, -1, 0, 1 };

    //Ÿ�� �湮�� ���� �ڷᱸ��
    class TileVisit//���� Ÿ��
    {
        public Tile tile;
        public bool isVisit;
    }

    /// <summary>
    /// Ŭ���� �༺���κ����� ��Ÿ� ǥ��
    /// </summary>
    public static void ShowNavigaitionDist_ClickTile(Tile clickTile)
    {
        //�� �༺ Ÿ��
        if (clickTile.PlanetType == Planet.None)
        {
            clickTile.HideDistanceTile();
            return;
        }
        if (clickTile.InstallBuilding != Building.None)
        {
            clickTile.HideDistanceTile();
            return;
        }

        //��Ÿ� �̳��� ���� �� �༺ Ÿ�� ǥ��
        foreach (var tile in TileManager.Instance.allTileList_MainBoard)
        {
            //�� �༺ Ÿ��
            if (tile.InstallBuilding != Building.None) continue;
            if (tile.PlanetType == Planet.None) continue;

            //��Ÿ� ��
            int dist = DistTileToTile(tile.TilePos, clickTile.TilePos);

            //��Ÿ����� ����
            if (dist <= PlayerManager.Instance.DistanceLimit)
            {
                tile.ShowDistanceTile();
            }
            else
            {
                tile.HideClickedTile();
                tile.HideDistanceTile();
            }
        }
    }

    /// <summary>
    /// ���� ����� ��Ÿ� ��ȯ
    /// </summary>
    public static int NearestNavigaitionDist(Tile clickTile)
    {
        //������ �ǹ��� �ƿ� ���� ��� ��Ÿ� �˻縦 ���� ����
        if (!IsBuidingInBoard()) return 0;

        //���� �ִ� �ǹ����� Ŭ���� Ÿ�ϰ� �˻�
        int nearestDist = int.MaxValue;
        foreach(var tile in TileManager.Instance.allTileList_MainBoard)
        {
            if (tile.InstallBuilding == Building.None) continue;

            //��Ÿ� ��
            int dist = DistTileToTile(tile.TilePos, clickTile.TilePos);

            //��Ÿ����� ����
            nearestDist = Mathf.Min(dist, nearestDist);
        }
        return nearestDist;
    }

    /// <summary>
    /// ��Ÿ� �߰� ���� ť�� : �Ÿ� 2�� 1��
    /// </summary>
    /// <param name="nearestDist"></param>
    /// <returns></returns>
    public static int AddQIC_Navigaition(int nearestDist)
    {
        //��Ÿ� ��
        if (nearestDist <= PlayerManager.Instance.DistanceLimit) return 0;

        //�Ÿ� 2�� 1��
        int addDist = nearestDist-PlayerManager.Instance.DistanceLimit;
        return (addDist+1)/2;
    }

    /// <summary>
    /// ���峻 �ǹ��� ������ �ִ°�?
    /// </summary>
    /// <returns></returns>
    public static bool IsBuidingInBoard()
    {
        int totalCount = 0;
        foreach(var build in PlayerManager.Instance._installBuidingCount)
        {
            totalCount += build.Value;
        }
        return totalCount > 0;
    }

    /// <summary>
    /// �� Ÿ�ϰ� �Ÿ�
    /// </summary>
    /// <returns></returns>
    public static int DistTileToTile(TilePosition tileA, TilePosition tileB)
    {
        float diffX = Mathf.Abs(tileA.x - tileB.x);
        float diffY = Mathf.Abs(tileA.y - tileB.y);
        float diffZ = Mathf.Abs(tileA.z - tileB.z);
        float dist = (diffX+diffY+diffZ)/2;
        return (int)Mathf.Round(dist);
    }

    /// <summary>
    /// �Ŀ��� ���� ���ϱ�
    /// </summary>
    /// <returns></returns>
    public static (int,bool) SumPower(HashSet<Tile> tileList)
    {
        int sumPower = 0;
        bool isMinPower = true;
        foreach (var tile in tileList)
        {
            if (tile.TilePower == 0) continue;//�� Ÿ��

            if (sumPower >= 7) isMinPower = false;//�̹� �� 7�̻��ε��� ����� �ǹ��� ��������

            //��ȸ ���ʽ� ����(���༺����)
            if (tile.PlanetType == Planet.Fire) sumPower += (tile.TilePower + PlayerManager.Instance.AddInstituteBonusPower);
            else sumPower += tile.TilePower;
        }
        return (sumPower, isMinPower);
    }

    /// <summary>
    /// ��� ����� Ÿ������?(bfs�� Ž��)
    /// </summary>
    /// <param name="tileList">Ŭ���� Ÿ�� ����Ʈ</param>
    /// <returns></returns>
    public static bool IsConnect_AllFactor(HashSet<Tile> tileList)
    {
        //1���� ����Ȱŷ� ����
        if(tileList.Count == 1) return true;

        //�湮 ������ ���� �ڷᱸ�� ����
        LinkedList<TileVisit> visitTileList = new LinkedList<TileVisit>();
        foreach (var tile in tileList)
        {
            TileVisit tileVisit = new TileVisit();
            tileVisit.tile = tile;
            tileVisit.isVisit = false;
            visitTileList.AddLast(tileVisit);
        }

        //���� Ÿ�� ����
        TileVisit startTile = visitTileList.First();
        startTile.isVisit = true;

        Queue<TileVisit> queue = new Queue<TileVisit>();
        queue.Enqueue(startTile);
    
        //BFS�� �˻�
        while (queue.Count > 0)
        {
            TileVisit curTile = queue.Dequeue();
     
            for (int i = 0; i < 6; i++)
            {
                //���� ����
                int nx = curTile.tile.TilePos.x + dx[i];
                int ny = curTile.tile.TilePos.y + dy[i];
                int nz = curTile.tile.TilePos.z + dz[i];

                //�ش� ������ �ִ� Ÿ�� ã��
                foreach(TileVisit nextTile in visitTileList)
                {
                    //���� Ÿ���� ã��
                    if (nextTile.tile.TilePos.x == nx
                        && nextTile.tile.TilePos.y == ny
                        && nextTile.tile.TilePos.z == nz)
                    {
                        //�湮���� ���� Ÿ�ϸ� �߰�
                        if (!nextTile.isVisit)
                        {
                            nextTile.isVisit = true;//���� Ÿ������ �ٲ����
                            queue.Enqueue(nextTile);
                        }
                        break;
                    }
                }
            }
        }
        //��� ����Ǿ����� �˻�
        foreach (TileVisit nextTile in visitTileList)
        {
            //�̹湮 ������ ������ �̴� ��� ����� Ÿ���� �ƴϴ�
            if (!nextTile.isVisit) return false;
        }
        return true;
    }

    /// <summary>
    /// �ּ� ������ ��Ģ�� ���� �ϴ���
    /// </summary>
    /// <param name="tileList">Ŭ���� Ÿ�� ����Ʈ</param>
    /// <param name="curSatelliteCount">���� ���� ����</param>
    /// <returns></returns>
    public static bool IsMin_Satellite_AllFactor(HashSet<Tile> tileList)
    {
        int curSatelliteCount = 0;

        return true;
    }

    /// <summary>
    /// �߰� ����
    /// </summary>
    /// <param name="planet">�༺</param>
    /// <returns></returns>
    public static (int ,int) RequireSabCount(Planet planet, int disCount)
    {
        if (planet == Planet.None) return (1000,100);//��ġ �Ұ�

        int sabCount = 0;
        //���༺���� ����
        switch (planet)
        {
            case Planet.Titanum:
            case Planet.Swamp:
                sabCount = 3-disCount;
                break;
            case Planet.Ice:
            case Planet.Desert:
                sabCount = 2-disCount;
                break;
            case Planet.Earth:
            case Planet.Volcano:
                sabCount = 1-disCount;
                break;
            default:
                break;
        }

        sabCount = Mathf.Max(0, sabCount);
        int sabCost = PlayerManager.Instance.AddOrePrice * sabCount;
        return (sabCost, sabCount);
    }

    /// <summary>
    /// ���� �༺ ���� ���� ����
    /// </summary>
    /// <returns></returns>
    public static int CountOccupyPlanet()
    {
        int cnt = 0;

        foreach(var planet in PlayerManager.Instance._planetOccupyDic)
        {
            if (planet.Value) cnt++;
        }
        return cnt;
    }

    /// <summary>
    /// 0~N-1 ���� ���� ����
    /// </summary>
    /// <param name="total">�� ����</param>
    /// <param name="select">�̴� ����</param>
    /// <returns></returns>
    public static HashSet<int> OrderNumberList(int total, int select)
    {
        HashSet<int> orderList = new HashSet<int>();
        while (orderList.Count < select)
        {
            int ran = UnityEngine.Random.Range(0, total);
            if (!orderList.Contains(ran))
            {
                orderList.Add(ran);
            }
        }
        return orderList;
    }
    /// <summary>
    /// ��ü Ÿ�� Ŭ��ǥ�� �����
    /// </summary>
    public static void AllHideClickedTile()
    {
        foreach (Tile tile in TileManager.Instance.allTileList_MainBoard)
        {
            tile.HideClickedTile();
        }
    }
    /// <summary>
    /// ���� ��ġ ȿ��
    /// </summary>
    /// <param name="tile">��ġ�� Ÿ��</param>
    public static void Effect_SabMine(Tile tile)
    {
        //���� ��ġ
        BuildingManager.Instance.InstallMine(tile, BuildingManager.Instance.sabDecreaseCount);
        //��ġ �� �ʱ�ȭ
        AllHideClickedTile();
        BuildingManager.Instance.sabDecreaseCount = 0;
    }
}
