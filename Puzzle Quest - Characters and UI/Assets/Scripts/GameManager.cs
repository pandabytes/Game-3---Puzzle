using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
	#region Member Variables

	/// <summary>
	/// Indicate if it's the player's turn
	/// </summary>
	public bool isPlayerTurn;

	/// <summary>
	/// The player game object.
	/// </summary>
	public GameObject player;

	/// <summary>
	/// The enemy game object.
	/// </summary>
	public GameObject enemy;

	/// <summary>
	/// The portal game object.
	/// </summary>
	public GameObject portal;

	#endregion

	// Use this for initialization
	void Start ()
	{
		EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth> ();
		enemyHealth.EnemyDefeated += new EventHandler (EnemyDefeatedHandler);
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	/// <summary>
	/// Handler for enemy defeated event.
	/// Activate the portal when the enemy is defeated.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void EnemyDefeatedHandler(object sender, EventArgs e)
	{

	}
}

