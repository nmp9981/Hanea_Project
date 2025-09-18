using System.Collections.Generic;
using UnityEngine;

public static class TileSystem
{
    /// <summary>
    /// 두 타일간 거리
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
    /// 파워값 총합 구하기
    /// </summary>
    /// <returns></returns>
    public static (int,bool) SumPower(HashSet<Tile> tileList)
    {
        int sumPower = 0;
        bool isMinPower = true;
        foreach (var tile in tileList)
        {
            if (sumPower >= 7) isMinPower = false;//이미 합 7이상인데도 계산할 건물이 남아있음
            sumPower += tile.TilePower;
        }
        return (sumPower, isMinPower);
    }

}
