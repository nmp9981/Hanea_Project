using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    [Header("�� ���� ����")]
    public int startXPos;
    public int startYPos;
    [Header("�� ũ��")]
    public int width;
    public int height;

    void Awake()
    {
        GenerateHexGrid();
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    void GenerateHexGrid()
    {
        float hexWidth = 0.866f; // sqrt(3)
        float hexHeight = 1.0f;

        for (int y = startYPos; y < height+startYPos; y++)
        {
            for (int x = startXPos; x < width+startXPos; x++)
            {
                float xPos = x * hexWidth;
                if (y % 2 != 0) // Ȧ�� ���� ������
                {
                    xPos += hexWidth / 2f;
                }
                float yPos = y * 0.75f * hexHeight;

                Instantiate(hexTilePrefab, new Vector3(xPos, yPos, 0), Quaternion.identity, transform);
            }
        }
    }
}
