using UnityEngine;

public class BlackPlanet
{
    /// <summary>
    /// 설치 가능한 검은 행성
    /// 전체에서 1번만 발동되므로 getcomponent를 써도 무관
    /// </summary>
    public static void AbleInstallBlackPlanet()
    {
        GameObject mainBoardObj = GameObject.Find("MainBoard");
        foreach(Tile tile in mainBoardObj.GetComponentsInChildren<Tile>())
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
    }
}
