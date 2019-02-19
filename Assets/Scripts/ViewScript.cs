using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewScript:ScriptableObject{
    private float UNITS_TO_PIXELS = 0.15f;

    //empty
    internal static Color COLOR_UNUSED = Color.white;
    internal static Color COLOR_START = Color.green;
    internal static Color COLOR_END = Color.blue;
    internal static Color COLOR_OBSTACLE = Color.red;

    private static ViewScript instance;

    private CellBehaviourScript[,] cellArray = new CellBehaviourScript[ModelScript.MAX_ROWS, ModelScript.MAX_COLS];

    private GameObject anchor;

    internal static ViewScript Instance
    {
        get
        {
            if(instance == null)
            {
                instance = ScriptableObject.CreateInstance<ViewScript>();
            }
            return instance;
        }
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    private void GenerateField()
    {
        anchor = GameObject.Find("Anchor");
        float shiftX = anchor.transform.position.x;
        float shiftY = anchor.transform.position.y;
        for (int x = 0; x < ModelScript.MAX_COLS; x++)
        {
            for (int y = 0; y < ModelScript.MAX_ROWS; y++)
            {
                var cell = (GameObject)Instantiate(Resources.Load("TileSprite"));        
                cell.transform.position = new Vector2(shiftX + x * UNITS_TO_PIXELS, shiftY + y * UNITS_TO_PIXELS);
                var cellBehaviour = cell.GetComponent<CellBehaviourScript>();
                cellBehaviour.x = x;
                cellBehaviour.y = y;
                cellArray[x, y] = cellBehaviour;
            }
        }
    }

    internal void RefreshField(Vector2Int startPoint, Vector2Int endPoint, List<Vector2Int> obstacleList)
    {
        if (!anchor)
        {
            GenerateField();
        }
        for (int x = 0; x < ModelScript.MAX_COLS; x++)
        {
            for (int y = 0; y < ModelScript.MAX_ROWS; y++)
            {
                cellArray[x, y].SetColor(COLOR_UNUSED);
            }
        }
        for (int i = 0; i < obstacleList.Count; i++)
        {
            cellArray[obstacleList[i].x, obstacleList[i].y].SetColor(COLOR_OBSTACLE);
        }
        if(startPoint.x>0 && startPoint.y > 0)
            cellArray[startPoint.x, startPoint.y].SetColor(COLOR_START);
        if(endPoint.x>0 && endPoint.y > 0)
            cellArray[endPoint.x, endPoint.y].SetColor(COLOR_END);
    }

    internal void DrawPath(List<Vector2Int> path)
    {
        for(int i=0; i<path.Count; i++)
        {
            cellArray[path[i].x, path[i].y].SetColor(Color.Lerp(COLOR_START, COLOR_END, ((float)i+1)/((float)path.Count+1)));
        }
    }

    internal void ShowResult(bool isPath, int steps)
    {
        GameObject textField = GameObject.Find("Result Text");
        String resultText;
        if (isPath)
            resultText = "Path finded!";
        else
            resultText = "No path.";
        resultText = resultText + "\nCells checked: " + steps;
        textField.GetComponentInChildren<Text>().text = resultText;
    }

    internal void ShowStartEndError()
    {
        GameObject textField = GameObject.Find("Result Text");
        textField.GetComponentInChildren<Text>().text = "Check start/end points or chose Randomize first!";
    }
}
