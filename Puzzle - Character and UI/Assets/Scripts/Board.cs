using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour 
{
	public List<Gem> gems = new List<Gem>();
	public int GridWidth;
	public int GridHeight;
	public GameObject gemPrefab;

	Vector3 boxPosHiLeftWorld = new Vector3(2, 2, 0);
	Vector3 boxPosLowRightWorld = new Vector3(1.5f, 0, 0);

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < GridHeight; i++)
		{
			for (int j = 0; j < GridWidth; j++)
			{
				GameObject g = Instantiate (gemPrefab, new Vector3(j, i, 0.0f), Quaternion.identity) as GameObject;
				g.transform.parent = transform;
				gems.Add (g.GetComponent<Gem>());
			}
		}
		//transform.Translate(new Vector3(-2.59f, -1.77f, 0.92f));
		transform.Translate(new Vector3(-5.25f, -1.77f, 0.92f));
	}


	private void OnGUI()
	{
		if (Input.GetKeyDown (KeyCode.A))
		{
			boxPosHiLeftWorld += Vector3.left;
			boxPosLowRightWorld += Vector3.left;
		}
		else if (Input.GetKeyDown (KeyCode.D))
		{
			boxPosHiLeftWorld += Vector3.right;
			boxPosLowRightWorld += Vector3.right;
		}
		if (Input.GetKeyDown (KeyCode.W))
		{
			boxPosHiLeftWorld += Vector3.up;
			boxPosLowRightWorld += Vector3.up;
		}
		else if (Input.GetKeyDown (KeyCode.S))
		{
			boxPosHiLeftWorld += Vector3.down;
			boxPosLowRightWorld += Vector3.down;
		}

		Vector3 boxPosHiLeftCamera = Camera.main.WorldToScreenPoint(boxPosHiLeftWorld);
		Vector3 boxPosLowRightCamera = Camera.main.WorldToScreenPoint(boxPosLowRightWorld);

		float width = boxPosHiLeftCamera.x - boxPosLowRightCamera.x;
		float height = boxPosHiLeftCamera.y - boxPosLowRightCamera.y;

		//GUI.Box(new Rect(boxPosHiLeftCamera.x, Screen.height - boxPosHiLeftCamera.y, width, height));
		GUI.Box(new Rect(0, 5, Screen.width, Screen.height),"Boxy");
	}

	private void Update()
	{
	}
}
