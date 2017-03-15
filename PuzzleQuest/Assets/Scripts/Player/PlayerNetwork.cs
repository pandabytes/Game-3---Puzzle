using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetwork : NetworkBehaviour
{
	public Player2Input player2;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!isLocalPlayer)
			return;
		
		if (Input.GetKeyDown (KeyCode.S))
		{
			Debug.Log ("Local Player: " + isLocalPlayer + ", Server: " + isServer);
		}
	}

	/// <summary>
	/// Update the client's move changes to the server
	/// </summary>
	/// <param name="activeTileNetworkID">Active tile network ID.</param>
	/// <param name="tileToSwapNetworkID">Tile to swap network ID.</param>
	[Command]
	public void CmdAttempMove(NetworkIdentity activeTileNetworkID, NetworkIdentity tileToSwapNetworkID)
	{
		object t = this;
		player2.AttemptMove (activeTileNetworkID, tileToSwapNetworkID);
	}

	/// <summary>
	/// Determines whether this instance is server and local.
	/// </summary>
	/// <returns><c>true</c> if this instance is server and local; otherwise, <c>false</c>.</returns>
	public bool IsServerAndLocal()
	{
		return isLocalPlayer && isServer;
	}
}

