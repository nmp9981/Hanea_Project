using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public static class TileSystem
{
    public static float root3 = 1.732f;

    //6방향 탐색
    private static int[] dx = {0,1,1,0,-1,-1 };
    private static int[] dy = { -1, -1, 0, 1, 1, 0 };
    private static int[] dz = { 1, 0, -1, -1, 0, 1 };

    //타일 방문을 위한 자료구조
    class TileVisit//참조 타입
    {
        public Tile tile;
        public bool isVisit;
    }

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
            if (tile.TilePower == 0) continue;//빈 타일

            if (sumPower >= 7) isMinPower = false;//이미 합 7이상인데도 계산할 건물이 남아있음
            sumPower += tile.TilePower;
        }
        return (sumPower, isMinPower);
    }

    /// <summary>
    /// 모두 연결된 타일인지?(bfs로 탐색)
    /// </summary>
    /// <param name="tileList">클릭한 타일 리스트</param>
    /// <returns></returns>
    public static bool IsConnect_AllFactor(HashSet<Tile> tileList)
    {
        //1개면 연결된거로 간주
        if(tileList.Count == 1) return true;

        //방문 구조를 위한 자료구조 설정
        LinkedList<TileVisit> visitTileList = new LinkedList<TileVisit>();
        foreach (var tile in tileList)
        {
            TileVisit tileVisit = new TileVisit();
            tileVisit.tile = tile;
            tileVisit.isVisit = false;
            visitTileList.AddLast(tileVisit);
        }

        //시작 타일 지정
        TileVisit startTile = visitTileList.First();
        startTile.isVisit = true;

        Queue<TileVisit> queue = new Queue<TileVisit>();
        queue.Enqueue(startTile);
    
        //BFS로 검사
        while (queue.Count > 0)
        {
            TileVisit curTile = queue.Dequeue();
     
            for (int i = 0; i < 6; i++)
            {
                //다음 지점
                int nx = curTile.tile.TilePos.x + dx[i];
                int ny = curTile.tile.TilePos.y + dy[i];
                int nz = curTile.tile.TilePos.z + dz[i];

                //해당 지점에 있는 타일 찾기
                foreach(TileVisit nextTile in visitTileList)
                {
                    //다음 타일을 찾음
                    if (nextTile.tile.TilePos.x == nx
                        && nextTile.tile.TilePos.y == ny
                        && nextTile.tile.TilePos.z == nz)
                    {
                        //방문하지 않은 타일만 추가
                        if (!nextTile.isVisit)
                        {
                            nextTile.isVisit = true;//참조 타입으로 바꿔야함
                            queue.Enqueue(nextTile);
                        }
                        break;
                    }
                }
            }
        }
        //모두 연결되었는지 검사
        foreach (TileVisit nextTile in visitTileList)
        {
            //미방문 지역이 있으면 이는 모두 연결된 타일이 아니다
            if (!nextTile.isVisit) return false;
        }
        return true;
    }

    /// <summary>
    /// 최소 위성수 규칙을 만족 하는지
    /// </summary>
    /// <param name="tileList">클릭한 타일 리스트</param>
    /// <param name="curSatelliteCount">현재 위성 개수</param>
    /// <returns></returns>
    public static bool IsMin_Satellite_AllFactor(HashSet<Tile> tileList)
    {
        int curSatelliteCount = 0;

        return true;
    }

    /// <summary>
    /// 추가 삽비용
    /// </summary>
    /// <param name="planet">행성</param>
    /// <returns></returns>
    public static int RequireSabCount(Planet planet)
    {
        if (planet == Planet.None) return 1000;//설치 불가

        int sabCount = 0;
        //모행성과의 관계
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
    /// 점령 행성 종류 개수 세기
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
