using UnityEngine;
using DigitalRuby.PyroParticles;
using System.Collections;

public class MeteorsAttack : MonoBehaviour
{
	private GameObject meteorsObject;
	private FireBaseScript meteorsScript;
	public GameObject meteorsPrefab;

	private void Start()
	{}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.M))
			SummonMeteors ();
	}

	/// <summary>
	/// Summons the meteors.
	/// </summary>
	public void SummonMeteors()
	{
		StopFiring ();
		BeginFiring ();
	}

	/// <summary>
	/// Begins the firing.
	/// </summary>
	private void BeginFiring()
	{
		Vector3 pos;
		float yRot = transform.rotation.eulerAngles.y;
		Vector3 forwardY = Quaternion.Euler (0.0f, yRot, 0.0f) * Vector3.forward;
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;
		Vector3 up = transform.up;
		Quaternion rotation = Quaternion.identity;

		meteorsObject = GameObject.Instantiate (meteorsPrefab);
		meteorsScript = meteorsObject.GetComponent<FireBaseScript> ();

		// temporary effect, like a meteors
		if (meteorsScript.IsProjectile)
		{
			rotation = transform.rotation;
			pos = transform.position + (4*forward) + (3*up);
		}
		else
		{
			pos = transform.position + (forwardY * 10.0f);
		}

		FireProjectileScript projectileScript = meteorsObject.GetComponentInChildren<FireProjectileScript> ();

		if (projectileScript != null)
		{
			// make sure we don't collide with other friendly layers
			projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer ("FriendlyLayer"));
		}

		meteorsObject.transform.position = pos;
		meteorsObject.transform.rotation = rotation;
	}

	/// <summary>
	/// Stops the firing.
	/// </summary>
	private void StopFiring()
	{
		// if we are running a constant effect like wall of fire, stop it now
		if (meteorsScript != null && meteorsScript.Duration > 10000)
		{
			meteorsScript.Stop();
		}
		meteorsObject = null;
		meteorsScript = null;
	}
}

