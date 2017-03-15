using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerInput : NetworkBehaviour {
	private GridManager gridManager;
	public LayerMask Tiles;
	private GameObject activeTile;
    public GameObject indicator;
	public GameManager gameManager;
	public Timer timer;
	public PlayerNetwork playerNetwork;
    private GameObject go;

	void Awake()
	{
		playerNetwork = GameObject.FindGameObjectWithTag ("Lobby Player").GetComponent<PlayerNetwork>();
		gridManager = GetComponent<GridManager> ();
		timer.TimesUp += EventTimesUpHandler;
	}

	// Update is called once per frame
	void Update ()
	{
		if (!playerNetwork.IsServerAndLocal ())
			return;
		
		// Make it unclickable when cover images are active
		if (Input.GetKeyDown (KeyCode.Mouse0) && timer.Second >= 0.8f && 
			!gameManager.coverImage1.IsActive () && !gameManager.uiManager.backgroundImage.IsActive())
		{
			if (activeTile == null)
				SelectTile ();
			else
				AttemptMove ();
		}
	}

	// Tries to select a tile if the players left-clicks and no other tile is selected.
	void SelectTile()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, Tiles);
        if (hit)
        {
            activeTile = hit.collider.gameObject;
            go = Instantiate(indicator, new Vector2(activeTile.transform.position.x, activeTile.transform.position.y), Quaternion.identity) as GameObject;
        }
	}

	//tries to select and move a tile if the player left-clicks and another tile has already been selected
	void AttemptMove()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 50f, Tiles);
		if(hit)
		{
            if (Vector2.Distance(activeTile.transform.position, hit.collider.gameObject.transform.position) <= 1.25f)
            {
                TileControl activeControl = activeTile.GetComponent<TileControl>();
                TileControl hitControl = hit.collider.gameObject.GetComponent<TileControl>();

                GridManager.XY activeXY = activeControl.MyXY;
                GridManager.XY hitXY = hitControl.MyXY;

                activeControl.Move(hitXY);
                hitControl.Move(activeXY);

                gridManager.SwitchTiles(hitXY, activeXY);
            }
                activeTile = null;
                Destroy(go);
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
