using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeler : MonoBehaviour
{
	public Gem owner;

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Gem")
			owner.AddNeighbor (other.GetComponent<Gem> ()); 
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Gem")
			owner.AddNeighbor (other.GetComponent<Gem> ()); 
	}
}
