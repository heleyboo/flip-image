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
        var x = BoardController.instance;
        // 1
        // if (render.sprite == null || BoardController.instance.IsShifting) {
        //     return;
        // }
        //
        // if (isSelected) { // 2 Is it already selected?
        //     Deselect();
        // } else {
        //     if (previousSelected == null) { // 3 Is it the first tile selected?
        //         Select();
        //     } else {
        //         previousSelected.Deselect(); // 4
        //     }
        // }
    }
}
