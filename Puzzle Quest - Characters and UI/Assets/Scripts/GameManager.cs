using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
	#region Member Variables

	/// <summary>
	/// Indicate if enemy is dead.
	/// </summary>
	private bool isEnemyDead;

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
	/// The chest.
	/// </summary>
	public GameObject chest;

	/// <summary>
	/// The user interface manager.
	/// </summary>
	public UIManager uiManager;

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start ()
	{
		isEnemyDead = false;
		EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth> ();
		enemyHealth.EnemyDefeated += new EventHandler (EnemyDefeatedHandler);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isEnemyDead)
		{
			chest.SetActive (true);
		}
	}

	/// <summary>
	/// The coroutine when the enemy dies.
	/// </summary>
	/// <returns>.</returns>
	private IEnumerator EnemyDefeatedCoroutine()
	{
		isEnemyDead = true;
		chest.SetActive (true);

		yield return new WaitForSeconds (3.0f);
		uiManager.EnablePopUpWindow ();
	}

	/// <summary>
	/// Handler for enemy defeated event.
	/// Activate the portal when the enemy is defeated.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void EnemyDefeatedHandler(object sender, EventArgs e)
	{
		isEnemyDead = true;
		StartCoroutine (EnemyDefeatedCoroutine ());
	}

	#endregion
}