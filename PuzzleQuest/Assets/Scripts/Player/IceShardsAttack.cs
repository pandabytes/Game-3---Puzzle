using UnityEngine;
using System.Collections;

public class IceShardsAttack : MonoBehaviour
{
	public GameObject iceShardsPrefab;
	public AudioSource iceShatterSound;

	// Use this for initialization
	void Start ()
	{
	}

	/// <summary>
	/// Unleashs ice shards.
	/// </summary>
	public void UnleashIce()
	{
		iceShatterSound.Play ();

		float yRot = transform.rotation.eulerAngles.y;
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;
		Vector3 up = transform.up;

		Vector3 pos = transform.position + (4*forward) + (3*up);
		Quaternion rotation = Quaternion.identity;

		GameObject iceShards = Instantiate (iceShardsPrefab) as GameObject;
		iceShards.transform.position = pos;
		iceShards.transform.rotation = rotation;

		Rigidbody rigidBody = iceShards.GetComponent<Rigidbody> ();
		rigidBody.velocity = Vector3.right * 10.0f;
	}
}

