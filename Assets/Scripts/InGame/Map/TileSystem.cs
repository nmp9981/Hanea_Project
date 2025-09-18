using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public static class TileSystem
{
    public static float root3 = 1.732f;

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
            if (sumPower >= 7) isMinPower = false;//�̹� �� 7�̻��ε��� ����� �ǹ��� ��������
            sumPower += tile.TilePower;
        }
        return (sumPower, isMinPower);
    }

    /// <summary>
    /// ��� ����� Ÿ������?
    /// </summary>
    /// <param name="tileList">Ŭ���� Ÿ�� ����Ʈ</param>
    /// <returns></returns>
    public static bool IsConnect_AllFactor(HashSet<Tile> tileList)
    {
        return false;
    }

    /// <summary>
    /// �ּ� ������ ��Ģ�� ���� �ϴ���
    /// </summary>
    /// <param name="tileList">Ŭ���� Ÿ�� ����Ʈ</param>
    /// <param name="curSatelliteCount">���� ���� ����</param>
    /// <returns></returns>
    public static bool IsMin_Satellite_AllFactor(HashSet<Tile> tileList, int curSatelliteCount)
    {

        return false;
    }
}
