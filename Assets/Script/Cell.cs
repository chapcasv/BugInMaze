using System.Collections.Generic;
using UnityEngine;

public class Cell
{   
    private bool isVisited;
    private List<GameObject> allWall;

    public GameObject North;
    public GameObject East;
    public GameObject West;
    public GameObject South;
    public Vector3 WorldPos;

    public List<GameObject> AllWall { get => allWall; }
    public bool IsVisited { get => isVisited; set => isVisited = value; }

    public Cell()
    {
        IsVisited = false;   
    }

    public void SetWall()
    {
        allWall = new List<GameObject>() { East, North, West, South };
    }

    public void RemoveWall(GameObject wall)
    {
        allWall.Remove(wall);
    }
}
