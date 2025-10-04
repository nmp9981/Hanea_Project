using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

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
            sumPower += tile.TilePower;
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
    public static int RequireSabCount(Planet planet)
    {
        if (planet == Planet.None) return 1000;//��ġ �Ұ�

        int sabCount = 0;
        //���༺���� ����
        switch (planet)
        {
            case Planet.Titanum:
            case Planet.Swamp:
                sabCount = 3;
                break;
            case Planet.Ice:
            case Planet.Desert:
                sabCount = 2;
                break;
            case Planet.Earth:
            case Planet.Volcano:
                sabCount = 1;
                break;
            default:
                break;
        }

        int sabCost = PlayerManager.Instance.AddOrePrice * sabCount;
        return sabCost;
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
}
