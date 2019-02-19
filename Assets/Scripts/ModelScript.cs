using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelScript : MonoBehaviour {

    internal const int MAX_ROWS = 50;
    internal const int MAX_COLS = 50;
    private const int NEIGHBOUR_DISTANCE = 1;

    //for really bad cases and mistakes
    private static int MAX_ITERATIONS = MAX_COLS*MAX_COLS;
    private static int steps;

    private static Vector2Int startPoint;
    private static Vector2Int endPoint;

    private static List<Vector2Int> obstacleList = new List<Vector2Int>();

    private static System.Random rand;

    // Use this for initialization
    void Start () {
        rand = new System.Random();

        startPoint = new Vector2Int(0, 0);
        endPoint = new Vector2Int(49, 49);
        obstacleList.Add(new Vector2Int(1, 1));

        Randomize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private static void RefreshField()
    {
        ViewScript.Instance.RefreshField(startPoint, endPoint, obstacleList);
    }

    internal static void AddRemoveObstacle(Vector2Int coord)
    {
        var badPoint = new Vector2Int(-1, -1);
        if (coord == startPoint)
            startPoint = badPoint;
        if (coord == endPoint)
            endPoint = badPoint;
        if (obstacleList.Contains(coord))
            obstacleList.Remove(coord);
        else
            obstacleList.Add(coord);
        RefreshField();
    }

    internal static void SetStartEndPoint(Vector2Int coord)
    {
        var badPoint = new Vector2Int(-1, -1);
        if(startPoint == badPoint)
        {
            startPoint = coord;
        }
        else if (endPoint == badPoint)
        {
            badPoint = coord;
        }
        else if (coord == startPoint)
        {
            endPoint = coord;
            startPoint = badPoint;
        }
        else if(coord == endPoint)
        {
            endPoint = badPoint;
        }
        else if (obstacleList.Contains(coord))
        {
            obstacleList.Remove(coord);
            startPoint = coord;
        }
        else
        {
            startPoint = coord;
        }
        RefreshField();
    }

    internal static void Randomize()
    {
        int obstacleNumber = rand.Next(MAX_COLS*MAX_ROWS/2);
        obstacleList = new List<Vector2Int>();
        do
        {
            int x = rand.Next(MAX_COLS);
            int y = rand.Next(MAX_ROWS);
            Vector2Int vect = new Vector2Int(x, y);
            if (!obstacleList.Contains(vect))
                obstacleList.Add(vect);
        }
        while (obstacleList.Count < obstacleNumber);

        startPoint = new Vector2Int(-1,-1);
        endPoint = new Vector2Int(-1, -1);

        do
        {
            int x = rand.Next(MAX_COLS);
            int y = rand.Next(MAX_ROWS);
            Vector2Int vect = new Vector2Int(x, y);
            if (!obstacleList.Contains(vect))
                startPoint = vect;
        } while (startPoint.x == -1);

        do
        {
            int x = rand.Next(MAX_COLS);
            int y = rand.Next(MAX_ROWS);
            Vector2Int vect = new Vector2Int(x, y);
            if (!obstacleList.Contains(vect))
                endPoint = vect;
        } while (endPoint.x == -1);

        RefreshField();
    }

    public static void AStarClick()
    {
        if (IsBadPoints())
        {
            ViewScript.Instance.ShowStartEndError();
            return;
        }


        var path = AStar();
        
        if (path != null)
        {
            ViewScript.Instance.DrawPath(path);
        }
        ViewScript.Instance.ShowResult(path!=null, steps);
    }

    private static bool IsBadPoints()
    {
        return startPoint.x < 0 || startPoint.y < 0 || endPoint.x < 0 || endPoint.y < 0;
    }

    private static List<Vector2Int> AStar()
    {
        float startTime = Time.time;
        var opened = new List<Cell>();
        var closed = new List<Cell>();

        var startCell = new Cell
        {
            coords = startPoint,
            previousCell = null,
            distanceFromStart = 0,
            approximateToEnd = DistanceBetweenPoints(startPoint, endPoint)
        };
        opened.Add(startCell);

        steps = 0;

        while (opened.Count > 0 && steps < MAX_ITERATIONS)
        {
            steps++;
            
            Cell currentCell = GetWithMinApprox(opened);
            if(currentCell.coords == endPoint)
            {
                return PathForCell(currentCell);
            }
            opened.Remove(currentCell);
            closed.Add(currentCell);
            var neighbours = GetNeighbours(currentCell);
            foreach(var neighbour in neighbours)
            {
                bool isClosed = false;
                foreach (var close in closed)
                {
                    if (close.coords == neighbour.coords)
                    {
                        isClosed = true;
                        break;
                    }
                }
                if(isClosed)
                    continue;
                Cell openCell = null;
                foreach(var open in opened)
                {
                    if (open.coords == neighbour.coords)
                    {
                        openCell = open;
                        break;
                    }
                }
                if (openCell == null)
                    opened.Add(neighbour);
                else if (openCell.distanceFromStart > neighbour.distanceFromStart)
                {
                    openCell.previousCell = currentCell;
                    openCell.distanceFromStart = neighbour.distanceFromStart;
                }
            }
        }
        return null;
    }

    private static Cell GetWithMinApprox(List<Cell> opened)
    {
        Cell answer = opened[0];
        foreach (var cell in opened)
        {
            if (answer.approximateToEnd > cell.approximateToEnd)
                answer = cell;
        }
        return answer;
    }

    //go away, Pythagoras, we don't have diagonal connection here
    private static int DistanceBetweenPoints(Vector2Int a, Vector2Int b)
    {
        return Math.Abs((a - b).x) + Math.Abs((a - b).y);
    }

    public static List<Vector2Int> PathForCell(Cell currentCell)
    {
        var answer = new List<Vector2Int>();
        var iterCell = currentCell;
        while (iterCell != null)
        {
            answer.Add(iterCell.coords);
            iterCell = iterCell.previousCell;
        }
        answer.Reverse();
        return answer;
    }

    private static List<Cell> GetNeighbours(Cell cell)
    {
        var neighbours = new List<Cell>();

        Vector2Int[] points = new Vector2Int[4];
        points[0] = new Vector2Int(cell.coords.x, cell.coords.y + 1);
        points[1] = new Vector2Int(cell.coords.x + 1, cell.coords.y);
        points[2] = new Vector2Int(cell.coords.x, cell.coords.y - 1);
        points[3] = new Vector2Int(cell.coords.x - 1, cell.coords.y);

        for(int i = 0; i<4; i++)
        {
            var point = points[i];
            if (point.x < 0 || point.y < 0 || point.x >= MAX_COLS || point.y >= MAX_ROWS)
                continue;
            if (obstacleList.Contains(point))
                continue;
            var neighbour = new Cell
            {
                coords = point,
                previousCell = cell,
                distanceFromStart = cell.distanceFromStart + NEIGHBOUR_DISTANCE,
                approximateToEnd = DistanceBetweenPoints(point, endPoint)
            };
            neighbours.Add(neighbour);
        }
        return neighbours;
    }
}
