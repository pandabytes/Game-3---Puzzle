using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player2Input : NetworkBehaviour
{
    private GridManager2 gridManager2;
    public LayerMask Tiles;
    private GameObject activeTile;
    public GameObject indicator;
	public GameManager gameManager;
	public Timer timer;
	public PlayerNetwork playerNetwork1;
	public PlayerNetwork playerNetwork2;
    private GameObject go;

    void Awake()
    {
		// PlayerNetwork1 has the local authority, PlayerNetwork2 doesn't
		playerNetwork1 = GameObject.FindGameObjectsWithTag ("Lobby Player") [0].GetComponent<PlayerNetwork>();
		if (!playerNetwork1.hasAuthority)
		{
			playerNetwork1 = GameObject.FindGameObjectsWithTag ("Lobby Player") [1].GetComponent<PlayerNetwork> ();
			playerNetwork2 = GameObject.FindGameObjectsWithTag ("Lobby Player") [0].GetComponent<PlayerNetwork> ();
		}
		else
		{
			playerNetwork2 = GameObject.FindGameObjectsWithTag ("Lobby Player") [1].GetComponent<PlayerNetwork> ();
		}
			
		playerNetwork1.player2 = this;
		playerNetwork2.player2 = this;
        gridManager2 = GetComponent<GridManager2>();
		timer.TimesUp += EventTimesUpHandler;
    }

    // Update is called once per frame
    void Update()
    {
		if (playerNetwork1.IsServerAndLocal())
			return;
		
		if (Input.GetKeyDown(KeyCode.Mouse1) && timer.Second >= 0.8f &&
			!gameManager.coverImage1.IsActive () && !gameManager.uiManager.backgroundImage.IsActive())
        {
			if (activeTile == null)
			{
				SelectTile ();
			}
			else
			{
				GameObject tileToSwap = null;
				NetworkIdentity swapTileNetworkID = null;
				NetworkIdentity activeTileNetworkID = activeTile.GetComponent<NetworkIdentity> ();
				Ray ray = GameObject.Find("Player2Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, Tiles);

				// Get the tile to swap network ID
				if (hit)
				{
					tileToSwap = hit.collider.gameObject;
					swapTileNetworkID = tileToSwap.GetComponent<NetworkIdentity> ();

				}

				// Send the IDs to the server
				if (swapTileNetworkID != null && activeTileNetworkID != null)
				{
					playerNetwork1.CmdAttempMove (activeTileNetworkID, swapTileNetworkID);
					activeTile = null;
					Destroy (go);
				}
			}
        }
    }

    // Tries to select a tile if the players left-clicks and no other tile is selected.
    void SelectTile()
    {		
        Ray ray = GameObject.Find("Player2Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, Tiles);
        if (hit)
        {
            activeTile = hit.collider.gameObject;
            go = Instantiate(indicator, new Vector2(activeTile.transform.position.x, activeTile.transform.position.y), Quaternion.identity) as GameObject;
        }
    }

    //tries to select and move a tile if the player right-clicks and another tile has already been selected
	public void AttemptMove(NetworkIdentity activeTileNetworkID, NetworkIdentity tileToSwapNetworkID)
    {
		GameObject activeTileServer = NetworkServer.FindLocalObject (activeTileNetworkID.netId);
		GameObject tileToSwapServer = NetworkServer.FindLocalObject (tileToSwapNetworkID.netId);
		if (Vector2.Distance (activeTileServer.transform.position, tileToSwapServer.transform.position) <= 1.25f)
		{
			TileControl2 activeControl = activeTileServer.GetComponent<TileControl2> ();
			TileControl2 hitControl = tileToSwapServer.GetComponent<TileControl2> ();

			GridManager2.XY2 activeXY = activeControl.MyXY2;
			GridManager2.XY2 hitXY = hitControl.MyXY2;

			activeControl.Move2 (hitXY);
			hitControl.Move2 (activeXY);

			gridManager2.SwitchTiles (hitXY, activeXY);
		}
    }

	/// <summary>
	/// Handler for when the time expires.
	/// Deselect a tile once the players run out of time.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void EventTimesUpHandler(object isPlayerTurn, EventArgs e)
	{
		if (activeTile != null)
		{
			activeTile = null;
			Destroy (go);
		}
	}
}
