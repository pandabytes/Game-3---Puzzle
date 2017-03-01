using UnityEngine;
using System.Collections;

public class PlayerDefense : MonoBehaviour
{
	/// <summary>
	/// The shield.
	/// </summary>
	public GameObject shield;

	/// <summary>
	/// Indicate if the shield is currently active or not.
	/// </summary>
	private bool isShieldActive;

	// Use this for initialization
	void Start ()
	{
		isShieldActive = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.S))
		{
			isShieldActive = !isShieldActive;
			shield.SetActive (isShieldActive);
		}
	}
}

