using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetwork : NetworkBehaviour
{
	public GameObject stageOnePrefab;

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
	/// Determines whether this instance is server and local.
	/// </summary>
	/// <returns><c>true</c> if this instance is server and local; otherwise, <c>false</c>.</returns>
	public bool IsServerAndLocal()
	{
		return isLocalPlayer && isServer;
	}
}

