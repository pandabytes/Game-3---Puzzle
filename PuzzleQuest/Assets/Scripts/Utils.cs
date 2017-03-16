using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using Prototype.NetworkLobby;

public class Utils : NetworkBehaviour
{
	/// <summary>
	/// The state of the game.
	/// </summary>
	public GameState gameState;

	public PlayerNetwork playerNetwork;

	void Start()
	{
		playerNetwork = GameObject.FindGameObjectWithTag ("Lobby Player").GetComponent<PlayerNetwork>();
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
		// Only server can start the game
		//playerNetwork = GameObject.FindGameObjectWithTag ("Lobby Player").GetComponent<PlayerNetwork>();

		if (isServer)
		{
			gameState.stage = StageEnum.FirstStage;
			LobbyManager lobbyManager = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();
			lobbyManager.ServerChangeScene ("Stage 3");
		}
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

