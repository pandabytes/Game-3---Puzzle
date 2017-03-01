using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightBox : MonoBehaviour 
{
	Rigidbody rb;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey (KeyCode.A))
		{
			rb.MovePosition(transform.position + Vector3.left);
		}
		else if (Input.GetKey (KeyCode.S))
		{
			rb.MovePosition(transform.position + Vector3.down);
		}
		else if (Input.GetKey (KeyCode.D))
		{
			rb.MovePosition(transform.position + Vector3.right);
		}
		else if (Input.GetKey (KeyCode.W))
		{
			rb.MovePosition(transform.position + Vector3.up);
		}
	}
}
