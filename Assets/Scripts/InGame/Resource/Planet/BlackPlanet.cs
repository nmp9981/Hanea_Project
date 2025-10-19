using UnityEngine;

public class BlackPlanet
{
    /// <summary>
    /// ��ġ ������ ���� �༺
    /// ��ü���� 1���� �ߵ��ǹǷ� getcomponent�� �ᵵ ����
    /// </summary>
    public static void AbleInstallBlackPlanet()
    {
        GameObject mainBoardObj = GameObject.Find("MainBoard");
        foreach(Tile tile in mainBoardObj.GetComponentsInChildren<Tile>())
        {
            //None Ÿ�Ը� Ȱ��ȭ
            if (tile.PlanetType != Planet.None) continue;

            tile.ShowClickedTile();
        }
    }

    /// <summary>
    /// ���� �༺ ȿ��
    /// </summary>
    /// <param name="tile">��ġ�� Ÿ��</param>
    public static void Effect_BlackPlanetTile(Tile tile)
    {
        tile.ShowBlackPlanet();
        tile.ChangeBuildingImageAndPower(Building.Mine);
        TileSystem.AllHideClickedTile();
    }
}
