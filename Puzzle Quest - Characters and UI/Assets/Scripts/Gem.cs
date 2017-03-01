using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
	public List<Gem> Neighbors = new List<Gem>();
	public GameObject sphere;
	public GameObject selector;
	private string[] gemMaterials = { "Red", "Orange", "Yellow", "Purple", "Blue", "Green", "Indigo" };
	private string color;

	// Use this for initialization
	void Start () 
	{
		CreateGem ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// Creates the gem.
	/// </summary>
	public void CreateGem()
	{
		color = gemMaterials [Random.Range (0, 7)];
		Material m = Resources.Load ("Materials\\" + color) as Material;
		sphere.GetComponent<Renderer> ().material = m;
	}

	/// <summary>
	/// Adds the neighbor.
	/// </summary>
	/// <param name="g">The green component.</param>
	public void AddNeighbor(Gem g)
	{
		if (!Neighbors.Contains(g))
			Neighbors.Add (g);
	}

	/// <summary>
	/// Removes the neighbor.
	/// </summary>
	/// <param name="g">The green component.</param>
	public void RemoveNeighbor(Gem g)
	{
		Neighbors.Remove (g);	
	}
}
