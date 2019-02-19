using UnityEngine;

//Basically, just struct. Cannot be struct because of cycle reference to another Cell.
public class Cell
{
    public Vector2Int coords;
    public int distanceFromStart;
    public int approximateToEnd;
    public Cell previousCell;   
}
