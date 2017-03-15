using SY = System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class GridManager : NetworkBehaviour
{
	public int score;
	public Text scoreText;
	public Timer timer;
	public PlayerNetwork playerNetwork;

	//a simple class to handle the coordinates
	public class XY
	{
		public int X;
		public int Y;

		public XY(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	//the tile class. keeps track of the tile type, the GameObject, and its controller
	public class Tile
	{
		public int TileType;
		public GameObject GO;
		public TileControl TileControl;

		public Tile()
		{
			TileType = -1;
		}

		public Tile(int tileType, GameObject go, TileControl tileControl)
		{
			TileType = tileType;
			GO = go;
			TileControl = tileControl;
		}
	}

	public GameObject[] TilePrefabs;

	public int GridWidth;
	public int GridHeight;
	public Tile[,] Grid;

	private int movingTiles;

	void Awake()
	{
		// Server creates 1st grid
		playerNetwork = GameObject.FindGameObjectWithTag ("Lobby Player").GetComponent<PlayerNetwork>();
		if (!playerNetwork.IsServerAndLocal())
			return;
		
		CreateGrid ();
		CheckMatches ();
		timer.TimesUp += new SY.EventHandler (TimesUpHandler);
	}
		
	void CreateGrid()
	{
		Grid = new Tile[GridWidth, GridHeight];

		for(int x = 0; x < GridWidth; x++)
		{
			for(int y = 0; y < GridHeight; y++)
			{
				int randomTileType = Random.Range(0, TilePrefabs.Length);
				GameObject go = Instantiate(TilePrefabs[randomTileType], new Vector2(x, y), Quaternion.identity) as GameObject;
				TileControl tileControl = go.GetComponent<TileControl>();
				Grid [x, y] = new Tile (randomTileType, go, tileControl);
				tileControl.GridManager = this;
				tileControl.MyXY = new XY(x,y);
				go.name = x + "/" + y;

				NetworkServer.Spawn(go);
			}
		}
	}

	public void SwitchTiles(XY firstXY, XY secondXY)
	{
		Tile firstTile = new Tile (Grid [firstXY.X, firstXY.Y].TileType, Grid [firstXY.X, firstXY.Y].GO, Grid [firstXY.X, firstXY.Y].TileControl);
		Tile secondTile = new Tile (Grid [secondXY.X, secondXY.Y].TileType, Grid [secondXY.X, secondXY.Y].GO, Grid [secondXY.X, secondXY.Y].TileControl);

		Grid [firstXY.X, firstXY.Y] = secondTile;
		Grid [secondXY.X, secondXY.Y] = firstTile;
	}
		
	public void CheckMatches()
	{
		List<XY> checkingTiles = new List<XY> (); //Tiles that are currently being considered for a match-3
		List<XY> tilesToDestroy = new List<XY> (); //Tiles that are confirmed match-3s and will be destroyed

		//vertical check
		for (int x = 0; x < GridWidth; x++) {
			int currentTileType = -1;
			int lastTileType = -1;

			if (checkingTiles.Count >= 3)
				tilesToDestroy.AddRange (checkingTiles);

			checkingTiles.Clear ();

			for (int y = 0; y < GridHeight; y++) {
				currentTileType = Grid [x, y].TileType;

				if (currentTileType != lastTileType) {
					if (checkingTiles.Count >= 3)
						tilesToDestroy.AddRange (checkingTiles);

					checkingTiles.Clear ();
				}

				checkingTiles.Add (new XY (x, y));
				lastTileType = currentTileType;
			}
		}

        if (checkingTiles.Count >= 3)
        {
            tilesToDestroy.AddRange(checkingTiles);
        }
        checkingTiles.Clear ();

		//horizontal check
		for (int y = 0; y < GridHeight; y++) 
		{
			int currentTileType = -1;
			int lastTileType = -1;

			if (checkingTiles.Count >= 3) 
			{
				for (int i = 0; i < checkingTiles.Count; i++) 
				{
					if (!tilesToDestroy.Contains (checkingTiles [i]))
						tilesToDestroy.Add (checkingTiles [i]);
				}
			}
			checkingTiles.Clear ();

			for (int x = 0; x < GridWidth; x++) 
			{
				currentTileType = Grid [x, y].TileType;

				if (currentTileType != lastTileType) 
				{
					if (checkingTiles.Count >= 3) {
						for (int i = 0; i < checkingTiles.Count; i++) 
						{
							if (!tilesToDestroy.Contains (checkingTiles [i]))
								tilesToDestroy.Add (checkingTiles [i]);
						}
					}
					checkingTiles.Clear ();
				}

				checkingTiles.Add (new XY (x, y));
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
		{
			ReplaceTiles ();
		}
	}

	/// <summary>
	/// Occurs when player has finished making his move.
	/// </summary>
	public event SY.EventHandler Attack;

	/// <summary>
	/// Raises the attack event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void OnAttack(object sender, SY.EventArgs e)
	{
		if (Attack != null)
		{
			Attack (sender, e);
		}
	}

	/// <summary>
	/// Reset the score when the time expires.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void TimesUpHandler(object sender, SY.EventArgs e)
	{
		// Do not deal damage is score is <= 0
		if (score > 0)
		{
			ScoreEventArgs scoreEvent = new ScoreEventArgs ((float) score);
			OnAttack (this, scoreEvent);

			score = 0;
			scoreText.text = score.ToString();
		}
	}

	void DestroyMatches(List<XY> tilesToDestroy)
	{
		for (int i = 0; i < tilesToDestroy.Count; i++) 
		{
			Destroy (Grid[tilesToDestroy [i].X, tilesToDestroy [i].Y].GO);
			Grid [tilesToDestroy [i].X, tilesToDestroy [i].Y] = new Tile ();
		}
		GravityCheck ();
	}

	void AddScore (int amount)
	{
		score += amount;
		scoreText.text = score.ToString();
	}

	void ReplaceTiles()
	{
		for(int x = 0; x < GridWidth; x++)
		{
			int missingTileCount = 0;

			for(int y = 0; y < GridHeight; y++)
			{
				if (Grid [x, y].TileType == -1)
					missingTileCount++;
			}

			for(int i = 0; i < missingTileCount; i++)
			{
				int tileY = GridHeight - missingTileCount + i;
				int randomTileType = Random.Range (0, TilePrefabs.Length);
				GameObject go = Instantiate (TilePrefabs [randomTileType], new Vector2 (x, GridHeight + i), Quaternion.identity) as GameObject;
				TileControl tileControl = go.GetComponent<TileControl> ();
				tileControl.GridManager = this;
				tileControl.Move(new XY(x, tileY));
				Grid[x, tileY] = new Tile(randomTileType, go, tileControl);
				go.name = x + "/" + tileY;

				NetworkServer.Spawn(go);
			}
		}
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
						Tile tile = new Tile (Grid [x, y].TileType, Grid [x, y].GO, Grid[x, y].TileControl);
						Grid [x, y].TileControl.Move (new XY (x, y - missingTileCount));
						Grid [x, y - missingTileCount] = tile;
						Grid [x, y] = new Tile ();
					}
				}
			}
		}
		ReplaceTiles ();
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
           CheckMatches ();
	}
}
