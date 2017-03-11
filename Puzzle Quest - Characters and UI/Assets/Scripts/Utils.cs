using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Utils : MonoBehaviour
{
	/// <summary>
	/// The state of the game.
	/// </summary>
	public GameState gameState;

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
		gameState.stage = StageEnum.FirstStage;
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

