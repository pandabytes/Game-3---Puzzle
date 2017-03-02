using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
	public void LoadFirstStageScene()
	{
		SceneManager.LoadScene ("FirstStage");
	}

	/// <summary>
	/// Quit this instance.
	/// </summary>
	public void Quit()
	{
		Application.Quit ();
	}
}

