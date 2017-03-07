using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager2 : MonoBehaviour
{
    private int score;
    public MainUI mainUI;
    public int distanceFromOtherBoard;

    //a simple class to handle the coordinates
    public class XY2
    {
        public int X;
        public int Y;

        public XY2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    //the tile class. keeps track of the tile type, the GameObject, and its controller
    public class Tile2
    {
        public int TileType;
        public GameObject GO;
        public TileControl2 TileControl;

        public Tile2()
        {
            TileType = -1;
        }

        public Tile2(int tileType, GameObject go, TileControl2 tileControl)
        {
            TileType = tileType;
            GO = go;
            TileControl = tileControl;
        }
    }

    public GameObject[] TilePrefabs;

    public int GridWidth;
    public int GridHeight;
    public Tile2[,] Grid;

    private int movingTiles;

    void Awake()
    {
        CreateGrid();
        CheckMatches();
    }

    void CreateGrid()
    {
        Grid = new Tile2[GridWidth, GridHeight];

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                int randomTileType = Random.Range(0, TilePrefabs.Length);
                GameObject go = Instantiate(TilePrefabs[randomTileType], new Vector2(x+ distanceFromOtherBoard, y), Quaternion.identity) as GameObject;
                TileControl2 tileControl = go.GetComponent<TileControl2>();
                Grid[x, y] = new Tile2(randomTileType, go, tileControl);
                tileControl.GridManager2 = this;
                tileControl.MyXY2 = new XY2(x, y);
                go.name = x + "/" + y;
            }
        }
    }

    public void SwitchTiles(XY2 firstXY, XY2 secondXY)
    {
        Tile2 firstTile = new Tile2(Grid[firstXY.X, firstXY.Y].TileType, Grid[firstXY.X, firstXY.Y].GO, Grid[firstXY.X, firstXY.Y].TileControl);
        Tile2 secondTile = new Tile2(Grid[secondXY.X, secondXY.Y].TileType, Grid[secondXY.X, secondXY.Y].GO, Grid[secondXY.X, secondXY.Y].TileControl);

        Grid[firstXY.X, firstXY.Y] = secondTile;
        Grid[secondXY.X, secondXY.Y] = firstTile;
    }
    /*
        public bool SwitchBack()
        {
            List<XY> checkingTiles = new List<XY>(); //Tiles that are currently being considered for a match-3
            List<XY> tilesToDestroy = new List<XY>(); //Tiles that are confirmed match-3s and will be destroyed

            //vertical check
            for (int x = 0; x < GridWidth; x++)
            {
                int currentTileType = -1;
                int lastTileType = -1;

                if (checkingTiles.Count >= 3)
                    tilesToDestroy.AddRange(checkingTiles);

                checkingTiles.Clear();

                for (int y = 0; y < GridHeight; y++)
                {
                    currentTileType = Grid[x, y].TileType;

                    if (currentTileType != lastTileType)
                    {
                        if (checkingTiles.Count >= 3)
                            tilesToDestroy.AddRange(checkingTiles);

                        checkingTiles.Clear();
                    }

                    checkingTiles.Add(new XY(x, y));
                    lastTileType = currentTileType;
                }
            }

            if (checkingTiles.Count >= 3)
            {
                tilesToDestroy.AddRange(checkingTiles);
            }
            checkingTiles.Clear();

            //horizontal check
            for (int y = 0; y < GridHeight; y++)
            {
                int currentTileType = -1;
                int lastTileType = -1;

                if (checkingTiles.Count >= 3)
                {
                    for (int i = 0; i < checkingTiles.Count; i++)
                    {
                        if (!tilesToDestroy.Contains(checkingTiles[i]))
                            tilesToDestroy.Add(checkingTiles[i]);
                    }
                }
                checkingTiles.Clear();

                for (int x = 0; x < GridWidth; x++)
                {
                    currentTileType = Grid[x, y].TileType;

                    if (currentTileType != lastTileType)
                    {
                        if (checkingTiles.Count >= 3)
                        {
                            for (int i = 0; i < checkingTiles.Count; i++)
                            {
                                if (!tilesToDestroy.Contains(checkingTiles[i]))
                                    tilesToDestroy.Add(checkingTiles[i]);
                            }
                        }
                        checkingTiles.Clear();
                    }

                    checkingTiles.Add(new XY(x, y));
                    lastTileType = currentTileType;
                }
            }

            if (checkingTiles.Count >= 3)
            {
                for (int i = 0; i < checkingTiles.Count; i++)
                {
                    if (!tilesToDestroy.Contains(checkingTiles[i]))
                        tilesToDestroy.Add(checkingTiles[i]);
                }
            }
            Debug.Log(tilesToDestroy.Count);
            if (tilesToDestroy.Count == 0)
            {
                Debug.Log("should be switching back");
                return true;
            }
            return false;

        }
        */
    public void CheckMatches()
    {
        List<XY2> checkingTiles = new List<XY2>(); //Tiles that are currently being considered for a match-3
        List<XY2> tilesToDestroy = new List<XY2>(); //Tiles that are confirmed match-3s and will be destroyed

        //vertical check
        for (int x = 0; x < GridWidth; x++)
        {
            int currentTileType = -1;
            int lastTileType = -1;

            if (checkingTiles.Count >= 3)
                tilesToDestroy.AddRange(checkingTiles);

            checkingTiles.Clear();

            for (int y = 0; y < GridHeight; y++)
            {
                currentTileType = Grid[x, y].TileType;

                if (currentTileType != lastTileType)
                {
                    if (checkingTiles.Count >= 3)
                        tilesToDestroy.AddRange(checkingTiles);

                    checkingTiles.Clear();
                }

                checkingTiles.Add(new XY2(x, y));
                lastTileType = currentTileType;
            }
        }

        if (checkingTiles.Count >= 3)
        {
            tilesToDestroy.AddRange(checkingTiles);
        }
        checkingTiles.Clear();

        //horizontal check
        for (int y = 0; y < GridHeight; y++)
        {
            int currentTileType = -1;
            int lastTileType = -1;

            if (checkingTiles.Count >= 3)
            {
                for (int i = 0; i < checkingTiles.Count; i++)
                {
                    if (!tilesToDestroy.Contains(checkingTiles[i]))
                        tilesToDestroy.Add(checkingTiles[i]);
                }
            }
            checkingTiles.Clear();

            for (int x = 0; x < GridWidth; x++)
            {
                currentTileType = Grid[x, y].TileType;

                if (currentTileType != lastTileType)
                {
                    if (checkingTiles.Count >= 3)
                    {
                        for (int i = 0; i < checkingTiles.Count; i++)
                        {
                            if (!tilesToDestroy.Contains(checkingTiles[i]))
                                tilesToDestroy.Add(checkingTiles[i]);
                        }
                    }
                    checkingTiles.Clear();
                }

                checkingTiles.Add(new XY2(x, y));
                lastTileType = currentTileType;
            }
        }

        if (checkingTiles.Count >= 3)
        {
            for (int i = 0; i < checkingTiles.Count; i++)
            {
                if (!tilesToDestroy.Contains(checkingTiles[i]))
                    tilesToDestroy.Add(checkingTiles[i]);
            }
        }

        if (tilesToDestroy.Count != 0)
            DestroyMatches(tilesToDestroy);
        else
            ReplaceTiles();
    }

    void DestroyMatches(List<XY2> tilesToDestroy)
    {
        score = 0;
        for (int i = 0; i < tilesToDestroy.Count; i++)
        {
            Destroy(Grid[tilesToDestroy[i].X, tilesToDestroy[i].Y].GO);
            Grid[tilesToDestroy[i].X, tilesToDestroy[i].Y] = new Tile2();

            if (i <= 2)
                AddScore(1);
            else
                AddScore(2);
        }
        GravityCheck();
    }

    void AddScore(int amount)
    {
        score += amount;
        mainUI.SetScoreText(score);
    }

    void ReplaceTiles()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            int missingTileCount = 0;

            for (int y = 0; y < GridHeight; y++)
            {
                if (Grid[x, y].TileType == -1)
                    missingTileCount++;
            }

            for (int i = 0; i < missingTileCount; i++)
            {
                int tileY = GridHeight - missingTileCount + i;
                int randomTileType = Random.Range(0, TilePrefabs.Length);
                GameObject go = Instantiate(TilePrefabs[randomTileType], new Vector2(x+ distanceFromOtherBoard, GridHeight + i), Quaternion.identity) as GameObject;
                TileControl2 tileControl = go.GetComponent<TileControl2>();
                tileControl.GridManager2 = this;
                tileControl.Move2(new XY2(x, tileY));
                Grid[x, tileY] = new Tile2(randomTileType, go, tileControl);
                go.name = x + "/" + tileY;
            }
        }
        //CheckForLegalMoves();
    }

    void GravityCheck()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            int missingTileCount = 0;

            for (int y = 0; y < GridHeight; y++)
            {
                if (Grid[x, y].TileType == -1)
                    missingTileCount++;
                else
                {
                    if (missingTileCount >= 1)
                    {
                        Tile2 tile = new Tile2(Grid[x, y].TileType, Grid[x, y].GO, Grid[x, y].TileControl);
                        Grid[x, y].TileControl.Move2(new XY2(x, y - missingTileCount));
                        Grid[x, y - missingTileCount] = tile;
                        Grid[x, y] = new Tile2();
                    }
                }
            }
        }
        ReplaceTiles();
    }

    public void ReportTileMovement()
    {
        movingTiles++;
    }

    //if tiles have been moving, we'll check for matches once they are all done
    public void ReportTileStopped()
    {
        movingTiles--;

        if (movingTiles == 0)
            CheckMatches();
    }
    /*
        public void CheckForLegalMoves()
        {
            //vertical check
            for(int x = 0; x < GridWidth; x++)
            {
                int secondToLastType = -1;
                int lastType = -2;
                int currentType = -3;

                for(int y = 0; y < GridHeight; y++)
                {
                    currentType = Grid[x, y].TileType;
                    if(lastType == currentType)
                    {
                        if (CheckForTileType(x, y - 3, currentType))
                            return;
                        if (CheckForTileType(x + 1, y - 2, currentType))
                            return;
                        if (CheckForTileType(x - 1, y - 2, currentType))
                            return;
                        if (CheckForTileType(x, y + 2, currentType))
                            return;
                        if (CheckForTileType(x + 1, y + 1, currentType))
                            return;
                        if (CheckForTileType(x - 1, y + 1, currentType))
                            return;
                    }
                    else if(secondToLastType == currentType)
                    {
                        if (CheckForTileType(x + 1, y - 1, currentType))
                            return;
                        if (CheckForTileType(x - 1, y - 1, currentType))
                            return;
                    }
                    secondToLastType = lastType;
                    lastType = currentType;
                }
            }

            for(int y = 0; y < GridHeight; y++)
            {
                int secondToLastType = -1;
                int lastType = -2;
                int currentType = -3;

                for(int x = 0; x < GridWidth; x++)
                {
                    currentType = Grid[x, y].TileType;
                    if(lastType == currentType)
                    {
                        if (CheckForTileType(x - 3, y, currentType))
                            return;
                        if (CheckForTileType(x - 2, y + 1, currentType))
                            return;
                        if (CheckForTileType(x - 2, y - 1, currentType))
                            return;
                        if (CheckForTileType(x + 2, y, currentType))
                            return;
                        if (CheckForTileType(x + 1, y + 1, currentType))
                            return;
                        if (CheckForTileType(x + 1, y - 1, currentType))
                            return;
                    }
                    else if (secondToLastType == currentType)
                    {
                        if (CheckForTileType(x - 1, y + 1, currentType))
                            return;
                        if (CheckForTileType(x - 1, y - 1, currentType))
                            return;
                    }
                    secondToLastType = lastType;
                    lastType = currentType;
                }
            }

            ShuffleGrid();
        }
        */
    /*
    bool CheckForTileType(int x, int y, int tileType)
    {
        if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
            return Grid[x, y].TileType == tileType;
        else
            return false;
    }

    public TileControl GetTileControl(XY xy)
    {
        return Grid[xy.X, xy.Y].TileControl;
    }

    void ShuffleGrid()
    {
        List<XY> xyList = new List<XY>();

        for(int x = 0; x < GridWidth; x++)
        {
            for(int y = 0; y < GridWidth; y++)
            {
                xyList.Add(new XY(x, y));
            }
        }

        for(int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridWidth; y++)
            {
                System.Random rnd = new System.Random();
                int index = rnd.Next(xyList.Count);
                XY xy = xyList[index];
                Grid[x, y].TileControl.Move(xy);
                xyList.RemoveAt(index);
            }
        }
        
    }
    */
}

