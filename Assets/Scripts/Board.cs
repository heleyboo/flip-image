using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class Board : MonoBehaviour
{
    public int width;

    public int height;
    
    public static Board instance;     // 1

    public GameObject tilePrefab;
    public BackgroundTile[,] allTiles;
    
    public List<Sprite> characters = new List<Sprite>();
    
    public bool IsShifting { get; set; }     // 6
    
    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<Board>();     // 7
        allTiles = new BackgroundTile[width, height];
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 tempPosition = new Vector3(i, j, 0);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = string.Format("({0}, {1})", i, j);
                Sprite newSprite = characters[Random.Range(0, characters.Count)]; // 2
                backgroundTile.GetComponent<SpriteRenderer>().sprite = newSprite; // 3
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
