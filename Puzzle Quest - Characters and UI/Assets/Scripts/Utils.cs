using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Utils : MonoBehaviour
{

	/// <summary>
	/// Loads the first stage scene.
	/// </summary>
	public void LoadFirstStageScene()
	{
		SceneManager.LoadScene ("Stage 1");
	}

	/// <summary>
	/// Loads the second stage scene.
	/// </summary>
	public void LoadSecondStageScene()
	{
		SceneManager.LoadScene ("Stage 2");
	}

	/// <summary>
	/// Quit this instance.
	/// </summary>
	public void Quit()
	{
		Application.Quit ();
	}
}

