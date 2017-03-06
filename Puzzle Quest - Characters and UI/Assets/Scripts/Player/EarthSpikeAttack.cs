using UnityEngine;
using System.Collections;

public class EarthSpikeAttack : MonoBehaviour
{
	public GameObject rock_1;
	public GameObject rock_2;
	public GameObject rock_3;
	public GameObject rock_4;

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

	private IEnumerator RaiseRocks()
	{
		Animation anim_1 = rock_1.GetComponent<Animation> ();
		Animation anim_2 = rock_2.GetComponent<Animation> ();

		anim_1.Play ("Rise_1");
		yield return new WaitForSeconds (1.0f);
		anim_1.Play ("Drop_1");

		yield return new WaitForSeconds (0.5f);

		anim_2.Play ("Rise_2");
		yield return new WaitForSeconds (1.0f);
		anim_2.Play ("Drop_2");
	}

	private IEnumerator RaiseRocksCoroutine()
	{
		Rise (rock_1);
		yield return new WaitForSeconds (0.5f);

		Drop (rock_1);
		Rise (rock_2);
		yield return new WaitForSeconds (0.5f);

		Drop (rock_2);
		Rise (rock_3);
		yield return new WaitForSeconds (0.5f);

		Drop (rock_3);
		Rise (rock_4);
		yield return new WaitForSeconds (0.5f);

		Drop (rock_4);
	}

	private void Rise(GameObject rock)
	{
		rock.transform.Translate (5.5f * Vector3.up);
	}

	private void Drop(GameObject rock)
	{
		rock.transform.Translate (5.5f * Vector3.down);
	}
}

