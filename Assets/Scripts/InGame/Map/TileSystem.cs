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
    public static int SumPower(List<Tile> tileList)
    {
        int sum = 0;
        foreach (Tile tile in tileList)
        {
            sum += tile.TilePower;
        }
        return sum;
    }

}
