using UnityEngine;
using DigitalRuby.PyroParticles;
using System.Collections;

public class FireBallAttack : MonoBehaviour
{
	private GameObject fireBallObject;
	private FireBaseScript fireBallScript;
	public GameObject fireBallPrefab;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void ShootFireBall()
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

		fireBallObject = GameObject.Instantiate (fireBallPrefab);
		fireBallScript = fireBallObject.GetComponent<FireBaseScript> ();

		// temporary effect, like a fireball
		if (fireBallScript.IsProjectile)
		{
			rotation = transform.rotation;
			pos = transform.position + (4*forward) + (3*up);
		}
		else
		{
			pos = transform.position + (forwardY * 10.0f);
		}

		FireProjectileScript projectileScript = fireBallObject.GetComponentInChildren<FireProjectileScript> ();

		if (projectileScript != null)
		{
			// make sure we don't collide with other friendly layers
			projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer ("FriendlyLayer"));
		}

		fireBallObject.transform.position = pos;
		fireBallObject.transform.rotation = rotation;
	}

	/// <summary>
	/// Stops the firing.
	/// </summary>
	private void StopFiring()
	{
		// if we are running a constant effect like wall of fire, stop it now
		if (fireBallScript != null && fireBallScript.Duration > 10000)
		{
			fireBallScript.Stop();
		}
		fireBallObject = null;
		fireBallScript = null;
	}
}

