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
        Sprite[] previousLeft = new Sprite[height];
        Sprite previousBelow = null;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 tempPosition = new Vector3(i, j, 0);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = string.Format("({0}, {1})", i, j);
                
                List<Sprite> possibleCharacters = new List<Sprite>(); // 1
                possibleCharacters.AddRange(characters); // 2

                possibleCharacters.Remove(previousLeft[j]); // 3
                possibleCharacters.Remove(previousBelow);
                
                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                backgroundTile.GetComponent<SpriteRenderer>().sprite = newSprite; // 3
                previousLeft[j] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
    
    public IEnumerator FindNullTiles() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++)
            {
                SpriteRenderer render = allTiles[x, y].GetComponent<SpriteRenderer>();
                if (render.sprite == null) {
                    Debug.Log("Found null");
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }
        
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                allTiles[x, y].GetComponent<BackgroundTile>().ClearAllMatches();
            }
        }
    }
    
    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) {
        IsShifting = true;
        List<SpriteRenderer>  renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < width; y++) {  // 1
            SpriteRenderer render = allTiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null) { // 2
                nullCount++;
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++) { // 3
            yield return new WaitForSeconds(shiftDelay);// 4
            for (int k = 0; k < renders.Count - 1; k++) { // 5
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, height - 1);
            }
        }
        IsShifting = false;
    }
    
    private Sprite GetNewSprite(int x, int y) {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);

        if (x > 0) {
            possibleCharacters.Remove(allTiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < width - 1) {
            possibleCharacters.Remove(allTiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0) {
            possibleCharacters.Remove(allTiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
