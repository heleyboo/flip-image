using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BackgroundTile : MonoBehaviour
{
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static BackgroundTile previousSelected = null;

    private SpriteRenderer render;
    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool matchFound = false;

    void Awake() {
        Debug.Log("11111");
        render = GetComponent<SpriteRenderer>();
    }

    private void Select() {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<BackgroundTile>();
    }

    private void Deselect() {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }
        
    void OnMouseDown() {
        Debug.Log("324343434");
        if (render.sprite == null || Board.instance.IsShifting) {
            return;
        }
        
        if (isSelected) { // 2 Is it already selected?
            Deselect();
        } else {
            if (previousSelected == null) { // 3 Is it the first tile selected?
                Select();
            } else {
                List<GameObject> allAdjacentTiles = GetAllAdjacentTiles();
                if (allAdjacentTiles.Contains(previousSelected.gameObject)) { // 1
                    SwapSprite(previousSelected.render); // 2
                    previousSelected.ClearAllMatches();
                    previousSelected.Deselect();
                    ClearAllMatches();
                } else { // 3
                    previousSelected.GetComponent<BackgroundTile>().Deselect();
                    Select();
                }
            }
        }
    }
    
    public void SwapSprite(SpriteRenderer render2) { // 1
        if (render.sprite == render2.sprite) { // 2
            return;
        }

        Sprite tempSprite = render2.sprite; // 3
        render2.sprite = render.sprite; // 4
        render.sprite = tempSprite; // 5
    }
    
    private GameObject GetAdjacent(Vector2 castDir) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null) {
            return hit.collider.gameObject;
        }
        return null;
    }
    
    private List<GameObject> GetAllAdjacentTiles() {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++) {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }
    
    private List<GameObject> FindMatch(Vector2 castDir) { // 1
        List<GameObject> matchingTiles = new List<GameObject>(); // 2
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // 3
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) { // 4
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles; // 5
    }
    
    private void ClearMatch(Vector2[] paths) // 1
    {
        List<GameObject> matchingTiles = new List<GameObject>(); // 2
        for (int i = 0; i < paths.Length; i++) // 3
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2) // 4
        {
            for (int i = 0; i < matchingTiles.Count; i++) // 5
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true; // 6
        }
    }
    
    public void ClearAllMatches() {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (matchFound) {
            render.sprite = null;
            matchFound = false;
            StopCoroutine(Board.instance.FindNullTiles());
            StartCoroutine(Board.instance.FindNullTiles());
        }
    }



}
