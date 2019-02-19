using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviourScript : MonoBehaviour {
    internal int x;
    internal int y;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
    }

    internal void SetColor(Color color)
    {
        if(!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.color = color;
    }

    private void OnMouseOver()
    {
        var coord = new Vector2Int(x, y);
        if (Input.GetMouseButtonDown(0))
        {
            ChangeStartEndClick(coord);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            ChangeObstacleClick(coord);
        }
    }

    private void ChangeObstacleClick(Vector2Int coord)
    {
        ModelScript.AddRemoveObstacle(coord);
    }

    private void ChangeStartEndClick(Vector2Int coord)
    {
        ModelScript.SetStartEndPoint(coord);
    }
}
