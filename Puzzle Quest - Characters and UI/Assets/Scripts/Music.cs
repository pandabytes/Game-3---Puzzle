using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
	public enum SceneEnum
	{
		StartMenu = 0, GameOver = 1
	};

	public SceneEnum scene;
	private AudioSource music;

	// Use this for initialization
	void Start ()
	{
		if (scene == SceneEnum.GameOver)
		{
			if (GameObject.FindGameObjectWithTag ("Music") == null)
			{
				music = gameObject.GetComponent<AudioSource> ();
				music.Play ();
			}
		}
		else
		{
			DontDestroyOnLoad (gameObject);
		}
	}

}

