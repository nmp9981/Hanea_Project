using UnityEngine;
using UnityEngine.UIElements;

public class BlackPlanet
{
    /// <summary>
    /// 설치 가능한 검은 행성
    /// 전체에서 1번만 발동되므로 getcomponent를 써도 무관
    /// </summary>
    public static void AbleInstallBlackPlanet()
    {
        foreach(Tile tile in TileManager.Instance.allTileList_MainBoard)
        {
            //None 타입만 활성화
            if (tile.PlanetType != Planet.None) continue;

            tile.ShowClickedTile();
        }
    }

    /// <summary>
    /// 검은 행성 효과
    /// </summary>
    /// <param name="tile">설치된 타일</param>
    public static void Effect_BlackPlanetTile(Tile tile)
    {
        tile.ShowBlackPlanet();
        tile.ChangeBuildingImageAndPower(Building.Mine);
        TileSystem.AllHideClickedTile();

        //점령행성 종류 추가
        if (PlayerManager.Instance._planetOccupyDic[Planet.Black] == false)
        {
            PlayerManager.Instance._planetOccupyDic[Planet.Black] = true;
            BuildingManager.Instance.planetInstallMineDic[Planet.Black].enabled = true;
            GameManager.Instance.finalBonusList[1].CountUP();
        }
    }
}
