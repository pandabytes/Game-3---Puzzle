using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SY = System;

public class GridManager2 : MonoBehaviour
{
	public int score;
    public int distanceFromOtherBoard;
	public Text scoreText;
	public Timer timer;
	public PlayerNetwork playerNetwork;

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
		// Client creates 2nd grid
//		playerNetwork = GameObject.FindGameObjectWithTag ("Lobby Player").GetComponent<PlayerNetwork>();
//		if (playerNetwork.IsServerAndLocal())
//			return;
		
        CreateGrid();
        CheckMatches();
		timer.TimesUp += new SY.EventHandler (TimesUpHandler);
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
		{
			AddScore (tilesToDestroy.Count);	
			DestroyMatches (tilesToDestroy);
		}
        else
            ReplaceTiles();
    }

	/// <summary>
	/// Occurs when player has finished making his move.
	/// </summary>
	public event SY.EventHandler Heal;

	/// <summary>
	/// Raises the heal event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void OnHeal(object sender, SY.EventArgs e)
	{
		if (Heal != null)
		{
			Heal (sender, e);
		}
	}

	/// <summary>
	/// Reset the score when the time expires.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void TimesUpHandler(object sender, SY.EventArgs e)
	{
		if (score > 0)
		{
			ScoreEventArgs scoreEvent = new ScoreEventArgs ((float)score);
			OnHeal (this, scoreEvent);

			score = 0;
			scoreText.text = score.ToString();
		}
	}

    void DestroyMatches(List<XY2> tilesToDestroy)
    {
        for (int i = 0; i < tilesToDestroy.Count; i++)
        {
            Destroy(Grid[tilesToDestroy[i].X, tilesToDestroy[i].Y].GO);
            Grid[tilesToDestroy[i].X, tilesToDestroy[i].Y] = new Tile2();
        }
        GravityCheck();
    }

    void AddScore(int amount)
    {
        score += amount;
		scoreText.text = score.ToString();
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
}