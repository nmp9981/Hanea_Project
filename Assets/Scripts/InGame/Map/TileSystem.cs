using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
    /// 클릭한 행성으로부터의 사거리 표시
    /// </summary>
    public static void ShowNavigaitionDist_ClickTile(Tile clickTile)
    {
        //빈 행성 타일
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

        //사거리 이내에 들어가는 빈 행성 타일 표시
        foreach (var tile in TileManager.Instance.allTileList_MainBoard)
        {
            //빈 행성 타일
            if (tile.InstallBuilding != Building.None) continue;
            if (tile.PlanetType == Planet.None) continue;

            //사거리 비교
            int dist = DistTileToTile(tile.TilePos, clickTile.TilePos);

            //사거리내에 있음
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
    /// 가장 가까운 사거리 반환
    /// </summary>
    public static int NearestNavigaitionDist(Tile clickTile)
    {
        //지어진 건물이 아예 없을 경우 사거리 검사를 하지 않음
        if (!IsBuidingInBoard()) return 0;

        //현재 있는 건물들은 클릭한 타일과 검사
        int nearestDist = int.MaxValue;
        foreach(var tile in TileManager.Instance.allTileList_MainBoard)
        {
            if (tile.InstallBuilding == Building.None) continue;

            //사거리 비교
            int dist = DistTileToTile(tile.TilePos, clickTile.TilePos);

            //사거리내에 있음
            nearestDist = Mathf.Min(dist, nearestDist);
        }
        return nearestDist;
    }

    /// <summary>
    /// 사거리 추가 정보 큐브 : 거리 2당 1개
    /// </summary>
    /// <param name="nearestDist"></param>
    /// <returns></returns>
    public static int AddQIC_Navigaition(int nearestDist)
    {
        //사거리 내
        if (nearestDist <= PlayerManager.Instance.DistanceLimit) return 0;

        //거리 2당 1개
        int addDist = nearestDist-PlayerManager.Instance.DistanceLimit;
        return (addDist+1)/2;
    }

    /// <summary>
    /// 보드내 건물이 지어져 있는가?
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

            //의회 보너스 적용(모행성에만)
            if (tile.PlanetType == Planet.Fire) sumPower += (tile.TilePower + PlayerManager.Instance.AddInstituteBonusPower);
            else sumPower += tile.TilePower;
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
    public static (int ,int) RequireSabCount(Planet planet, int disCount)
    {
        if (planet == Planet.None) return (1000,100);//설치 불가

        int sabCount = 0;
        //모행성과의 관계
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

    /// <summary>
    /// 0~N-1 랜덤 숫자 정렬
    /// </summary>
    /// <param name="total">총 개수</param>
    /// <param name="select">뽑는 개수</param>
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
    /// 전체 타일 클릭표시 숨기기
    /// </summary>
    public static void AllHideClickedTile()
    {
        foreach (Tile tile in TileManager.Instance.allTileList_MainBoard)
        {
            tile.HideClickedTile();
        }
    }
    /// <summary>
    /// 광산 설치 효과
    /// </summary>
    /// <param name="tile">설치된 타일</param>
    public static void Effect_SabMine(Tile tile)
    {
        //광산 설치
        BuildingManager.Instance.InstallMine(tile, BuildingManager.Instance.sabDecreaseCount);
        //설치 후 초기화
        AllHideClickedTile();
        BuildingManager.Instance.sabDecreaseCount = 0;
    }
}
