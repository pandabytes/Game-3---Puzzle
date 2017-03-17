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

	void Start()
	{
	}

	#region Public Methods

	/// <summary>
	/// Replay the game in whichever scene the players lose.
	/// </summary>
	public void Replay()
	{
		if (!isServer)
			return;
		
		LobbyManager lobbyManager = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();
		if (gameState.stage == StageEnum.FirstStage)
		{
			if (GameObject.FindGameObjectWithTag ("Music") != null)
			{
				Destroy (GameObject.FindGameObjectWithTag ("Music"));
			}
			SceneManager.LoadScene ("Stage 1");
			lobbyManager.ServerChangeScene ("Stage 1");
		}
		else if (gameState.stage == StageEnum.SecondStage)
		{
			SceneManager.LoadScene ("Stage 2");
			lobbyManager.ServerChangeScene ("Stage 2");
		}
		else if (gameState.stage == StageEnum.ThirdStage)
		{
			SceneManager.LoadScene ("Stage 3");
			lobbyManager.ServerChangeScene ("Stage 3");
		}
		else if (gameState.stage == StageEnum.FourthStage)
		{
			SceneManager.LoadScene ("Stage 4");
			lobbyManager.ServerChangeScene ("Stage 4");
		}
	}

	/// <summary>
	/// Play the game from the beginning.
	/// </summary>
	public void PlayFromBeginning()
	{
		// Only server can start the game
		if (isServer)
		{
			gameState.stage = StageEnum.FirstStage;
			if (GameObject.FindGameObjectWithTag ("Music") != null)
			{
				Destroy (GameObject.FindGameObjectWithTag ("Music"));
			}

			LobbyManager lobbyManager = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();
			lobbyManager.ServerChangeScene ("Stage 1");
		}
	}

	/// <summary>
	/// Quit this instance.
	/// </summary>
	public void Quit()
	{
		if (isServer)
		{
			gameState.stage = StageEnum.FirstStage;
			Application.Quit ();
		}
	}

	#endregion
}

