using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
	public void LoadPlayScene()
	{
		SceneManager.LoadScene ("Main");
	}

	/// <summary>
	/// Quit this instance.
	/// </summary>
	public void Quit()
	{
		Application.Quit ();
	}
}

