using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;     // 1
    public List<Sprite> characters = new List<Sprite>();     // 2
    public GameObject tile;      // 3
    public int xSize, ySize;     // 4

    private GameObject[,] tiles;      // 5

    public bool IsShifting { get; set; }     // 6

    void Start () {
        instance = GetComponent<BoardController>();     // 7

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(100, 100);     // 8
    }

    private void CreateBoard (float xOffset, float yOffset) {
        tiles = new GameObject[xSize, ySize];     // 9

        float startX = transform.position.x;     // 10
        float startY = transform.position.y;

        for (int x = 0; x < xSize; x++) {      // 11
            for (int y = 0; y < ySize; y++) {
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
                tiles[x, y] = newTile;
            }
        }
    }
}
