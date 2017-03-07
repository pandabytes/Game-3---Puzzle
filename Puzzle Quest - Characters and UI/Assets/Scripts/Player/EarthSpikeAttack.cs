using UnityEngine;
using System.Collections;

public class EarthSpikeAttack : MonoBehaviour
{
	public GameObject rock_1;
	public GameObject rock_2;
	public GameObject rock_3;
	public GameObject rock_4;
	public AudioSource earthRumbleSound;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R))
		{
			StartCoroutine (RaiseRocks ());
		}
	}

	/// <summary>
	/// Raises the rocks.
	/// </summary>
	/// <returns>The rocks.</returns>
	private IEnumerator RaiseRocks()
	{
		Animation anim_1 = rock_1.GetComponent<Animation> ();
		Animation anim_2 = rock_2.GetComponent<Animation> ();
		Animation anim_3 = rock_3.GetComponent<Animation> ();
		Animation anim_4 = rock_4.GetComponent<Animation> ();

		// Play the sound effect
		earthRumbleSound.Play ();
		earthRumbleSound.volume = 1.0f;

		anim_1.Play ("Rise_1");
		yield return new WaitForSeconds (0.4f);
		anim_1.Play ("Drop_1");

		yield return new WaitForSeconds (0.1f);

		anim_2.Play ("Rise_2");
		yield return new WaitForSeconds (0.4f);
		anim_2.Play ("Drop_2");

		yield return new WaitForSeconds (0.1f);

		anim_3.Play ("Rise_3");
		yield return new WaitForSeconds (0.4f);
		anim_3.Play ("Drop_3");

		yield return new WaitForSeconds (0.1f);

		anim_4.Play ("Rise_4");
		yield return new WaitForSeconds (0.4f);
		anim_4.Play ("Drop_4");

		// Fade out the sound effect
		yield return new WaitForSeconds (0.3f);
		earthRumbleSound.volume = 0.7f;
		yield return new WaitForSeconds (0.3f);
		earthRumbleSound.volume = 0.4f;
		yield return new WaitForSeconds (0.3f);
		earthRumbleSound.volume = 0.1f;
		yield return new WaitForSeconds (0.3f);
		earthRumbleSound.volume = 0.0f;

		earthRumbleSound.Stop ();
	}
}

