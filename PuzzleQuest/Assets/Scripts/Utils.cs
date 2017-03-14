using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using Prototype.NetworkLobby;

public class Utils : MonoBehaviour
{
	/// <summary>
	/// The state of the game.
	/// </summary>
	public GameState gameState;

	public PlayerNetwork playerNetwork;

	void Start()
	{
		//playerNetwork = GameObject.FindGameObjectWithTag ("Lobby Player").GetComponent<PlayerNetwork>();
	}

	void Update()
	{

	}

	/// <summary>
	/// Replay the game in whichever scene the players lose.
	/// </summary>
	public void Replay()
	{
		if (gameState.stage == StageEnum.FirstStage)
		{
			SceneManager.LoadScene ("Stage 1");	
		}
		else if (gameState.stage == StageEnum.SecondStage)
		{
			SceneManager.LoadScene ("Stage 2");	
		}
		else if (gameState.stage == StageEnum.ThirdStage)
		{
			SceneManager.LoadScene ("Stage 3");	
		}
		else if (gameState.stage == StageEnum.FourthStage)
		{
			SceneManager.LoadScene ("Stage 4");	
		}
	}

	/// <summary>
	/// Play the game from the beginning.
	/// </summary>
	public void PlayFromBeginning()
	{
//		if (GameObject.FindGameObjectWithTag("Lobby Player") == null)
//		{
//			Debug.Log ("Lobby player is NULL");
//			return;
//		}
//
//		// Only server can start the game
//		playerNetwork = GameObject.FindGameObjectWithTag ("Lobby Player").GetComponent<PlayerNetwork>();
//		if (playerNetwork.IsServerAndLocal())
//		{
//			gameState.stage = StageEnum.FirstStage;
//			LobbyManager lobbyManager = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();
//			lobbyManager.ServerChangeScene ("Stage 1");
//		}

		SceneManager.LoadScene ("Stage 1");
	}

	/// <summary>
	/// Quit this instance.
	/// </summary>
	public void Quit()
	{
		gameState.stage = StageEnum.FirstStage;
		Application.Quit ();
	}
}

