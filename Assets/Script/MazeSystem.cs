using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeSystem : MonoBehaviour
{
    #region Properties
    [SerializeField] Text stageInfomation;

    [Header("Maze Generate")]
    [SerializeField] GameObject Wall;
    [SerializeField] Transform Maze;
    [SerializeField] Transform bugPrefab;
    [SerializeField] Transform gatePrefab;
    public static bool renderWithEffect = true;


    private Dictionary<Cell, Cell> previous = new Dictionary<Cell, Cell>();
    private Dictionary<Cell, float> distances = new Dictionary<Cell, float>();
    private List<Cell> Path = new List<Cell>();
    private List<Cell> openList = new List<Cell>();

    private Vector2 initialPos;
    private Vector2 wallPos;
    private Transform Bug;
    private Cell bugCell;
    private Transform Gate;
    private Cell gateCell;
    private Cell currentCell;
    private Queue cellStack;
    private GameObject[] allWall;
    private List<Cell> allCell;
    private List<Line> allLine;
    private List<Line> way;

    private int VisitedCells;
    private int TotalCells;
    private float wallLength = 1;
    private float width = 10;
    private float height = 13;

    #endregion

    void Start()
    {
        StartGenerate();
        BugSpawn();
        GateSpawn();
        SetWayDontHaveWall();
        LoadStageInfomation();
    }

    #region Methods

    #region Maze Generate

    private void StartGenerate()
    {
        GenerateWall();
        InitializeCell();
        InitializeLine();
        GenerateMaze();
    }

    private void GenerateWall()
    {   
        ///Start from top-left
        initialPos = new Vector2((-width / 2 + wallLength / 2), (height / 2));

        for (int j = 0; j < width; j++)
        {   
            ///Generate + 1 wall for outline
            for(int i = 0; i <= height; i++) 
            {
                wallPos = new Vector2(initialPos.x + (j * wallLength),
                                      initialPos.y - (i * wallLength));
                Instantiate(Wall, wallPos, Quaternion.identity, Maze);
            }
        }

        ///Generate + 1 wall for outline
        for (int j = 0; j <= width; j++)
        {
            for (int i = 0; i < height; i++)
            {
                wallPos = new Vector2(initialPos.x + (j * wallLength) - (wallLength / 2),
                                      initialPos.y - (i * wallLength) - (wallLength / 2));
                Instantiate(Wall, wallPos, Quaternion.Euler(0, 0, 90), Maze);
            }
        }

        allWall = new GameObject[Maze.childCount];
        for (int i = 0; i < allWall.Length; i++)
        {
            allWall[i] = Maze.GetChild(i).gameObject;
        }
    }

    private void InitializeCell()
    {
        allCell = new List<Cell>((int)(width*height));

        ///First cell start from top-left (1,1)
        int offsetChangeColumn = 0;
        int rows = 0;
        int offsetOutline = 1;

        for (int wallIndex = 0; wallIndex < allCell.Capacity; wallIndex++)
        {
            Cell newCell = new Cell();
            newCell.West = allWall[(wallIndex + ((int)height + offsetOutline) * (int)width)];
            newCell.East = allWall[(wallIndex + ((int)height + offsetOutline) * (int)width) + (int)height];

            if(rows == height)
            {
                offsetChangeColumn += 1;
                rows = 0;
            }
            rows++;

            newCell.South = allWall[wallIndex + offsetChangeColumn + offsetOutline];
            newCell.North = allWall[wallIndex + offsetChangeColumn];
            newCell.SetWall();
            SetCellWorldPos(newCell);
            allCell.Add(newCell);
        }
    }
    
    private void InitializeLine()
    {   
        if (allCell == null) return;

        allLine = new List<Line>();
        foreach (Cell from in allCell)
        {
            foreach (Cell to in allCell)
            {
                if(Vector2.Distance(from.WorldPos,to.WorldPos) == wallLength)
                {
                    allLine.Add(new Line(from, to));
                }
            }
        }
    }

    private void GenerateMaze()
    {
        TotalCells = allCell.Count;
        currentCell = allCell[Random.Range(0,TotalCells)];
        currentCell.IsVisited = true;
        VisitedCells = 1;
        cellStack = new Queue();

        while (VisitedCells < TotalCells)
        {
            List<Cell> neighbors = GetNeighbors(currentCell);
            
            if (neighbors.Count > 0)
            {
                int randomCell = Random.Range(0, neighbors.Count);

                Cell theCell = neighbors[randomCell];
                theCell.IsVisited = true;

                BreakWall(currentCell, theCell);

                cellStack.Enqueue(currentCell);
                currentCell = theCell; 
                VisitedCells++; 
            }
            else 
            {
                currentCell = (Cell)cellStack.Dequeue();
            }
        }
    }

    private void BreakWall(Cell currentCell, Cell adjacentCell)
    {
        foreach (var wallCurrent in currentCell.AllWall)
        {
            foreach (var wallAdjacent in adjacentCell.AllWall)
            {
                if(wallCurrent.transform.position == wallAdjacent.transform.position)
                {
                    currentCell.RemoveWall(wallCurrent);
                    adjacentCell.RemoveWall(wallAdjacent);
                    Destroy(wallAdjacent);
                    return;
                }
            }
        }
    }

    private List<Cell> GetNeighbors(Cell from)
    {
        List<Cell> adjacentCells = new List<Cell>();

        foreach (Line line in allLine)
        {
            if (line.From == from && !line.To.IsVisited)
                adjacentCells.Add(line.To);
        }
        return adjacentCells;
    }

    private void SetCellWorldPos(Cell cell)
    {
        float x = cell.West.transform.position.x + wallLength / 2;
        float y = cell.West.transform.position.y;
        cell.WorldPos = new Vector2(x, y);
    }

    #endregion

    #region Finding Path

    ///Return list neighbor dont have wall bettween
    private List<Cell> GetNeighborForPath(Cell from)
    {
        List<Cell> neighbor = new List<Cell>();

        foreach(Line line in way)
        {
            if (line.From == from)
                neighbor.Add(line.To);
        }
        return neighbor;
    }

    private void SetWayDontHaveWall()
    {
        way = new List<Line>();
        foreach (Line l in allLine)
        {
            if (!haveWallBetween(l.From, l.To)) 
                way.Add(l);
        }
    }

    private bool haveWallBetween(Cell from, Cell to)
    {
        foreach (GameObject wallFrom in from.AllWall)
        {
            foreach (GameObject wallTo in to.AllWall)
            {
                if (wallFrom == wallTo)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private List<Cell> GetPath(Cell start, Cell end)
    {
        List<Cell> path = new List<Cell>();

        if (start == end)
        {
            path.Add(start);
            return path;
        }

        InitializeOpenList();

        distances[start] = 0;

        while (openList.Count != 0)
        {
            Cell current = PopCell_Distance_Smallest();

            if(current == end)
            {
                path = ConstructPath(current);
                break;
            }

            foreach (Cell neighbor in GetNeighborForPath(current))
            {
                
                float lengthA = Vector3.Distance(current.WorldPos, neighbor.WorldPos);

                // The distance from start to neighbor of current
                float lengthB = distances[current] + lengthA;

                // short path - from start to neighbor of current
                if (lengthB < distances[neighbor])
                {
                    distances[neighbor] = lengthB;
                    previous[neighbor] = current;
                }
            }  
        }
        return path;
    }

    private Cell PopCell_Distance_Smallest()
    {
        openList = openList.OrderBy(cell => distances[cell]).ToList();
        Cell smallest = openList[0];
        openList.Remove(smallest);
        return smallest;
    }

    private List<Cell> ConstructPath(Cell current)
    {
        List<Cell> path = new List<Cell>();
        while (previous.ContainsKey(current))
        {
            path.Insert(0, current);
            current = previous[current];
        }
        path.Insert(0, current);
        return path;
    }

    ///Set all distances to infinity
    private void InitializeOpenList()
    {
        for (int i = 0; i < allCell.Count; i++)
        {
            openList.Add(allCell[i]);
            distances.Add(allCell[i], int.MaxValue);
        }
    }

    #endregion 

    #region Gameplay
    private void GateSpawn()
    {
        Gate = Instantiate(gatePrefab);
        int randomIndex = Random.Range(1, allCell.Count);
        gateCell = allCell[randomIndex];
        Gate.position = gateCell.WorldPos;
    }

    private void BugSpawn()
    {
        Bug = Instantiate(bugPrefab);
        bugCell = allCell[0];
        Bug.position = bugCell.WorldPos;
    }

    public void FindWay()
    {
        if (!WayEffect.Rendering)
        {
            WayEffect wayeffect = GetComponent<WayEffect>();
            if (Path.Count == 0)
            {
                Cell start = bugCell;
                Cell end = gateCell;
                Path = GetPath(start, end);
                wayeffect.RenderBy(Path);
            }
            else
            {
                wayeffect.RenderBy(Path);
            }

        }
    }

    public void AutoMove()
    {
        if (!Player.inAutoMode)
        {
            Player.AutoMoveBy(Path);
        }   
    }

    private void LoadStageInfomation()
    {
        stageInfomation.text = "Stage - " + StageManager.StageSelected.ToString();
    }

    public void BackMenuStage()
    {
        SceneManager.LoadScene(0);
    }

    #endregion 

    #endregion
}
