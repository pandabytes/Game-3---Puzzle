using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetwork : NetworkBehaviour
{
	public Player2Input player2;
	public PlayerAttack playerAttack;
	public GolemAttack golemAttack;

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
			Debug.Log ("Local Player: " + isLocalPlayer + ", Server: " + isServer + ", Ready: ");
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
		player2.AttemptMove (activeTileNetworkID, tileToSwapNetworkID);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="attackName">Attack name.</param>
	/// <param name="playerNetworkID">Player network ID.</param>
	[ClientRpc]
	public void RpcPlayerAttack(string attackName, NetworkIdentity playerNetworkID)
	{
		playerAttack.ClientAttack (attackName, playerNetworkID);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="usePhysicalAttack">If set to <c>true</c> use physical attack.</param>
	/// <param name="golemNetworkID">Golem network ID.</param>
	[ClientRpc]
	public void RpcGolemAttack(bool usePhysicalAttack, NetworkIdentity golemNetworkID)
	{
		//golemAttack.ClientAttack (usePhysicalAttack, golemNetworkID);
	}

	[ClientRpc]
	public void RpcUnfreezeTime(float timeScale)
	{
		Time.timeScale = timeScale;
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

