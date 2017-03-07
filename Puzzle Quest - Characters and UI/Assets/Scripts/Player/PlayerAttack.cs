using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour 
{
	#region Member variables

	/// <summary>
	/// The game manager.
	/// </summary>
	public GameManager gameManager;

	/// <summary>
	/// The player input.
	/// </summary>
	public GridManager gridManager;

	/// <summary>
	/// This demtermines how much damage to deal to the enemy.
	/// </summary>
	private float score;

	/// <summary>
	/// The animation component of the player.
	/// </summary>
	private Animation anim;

	/// <summary>
	/// The player movement.
	/// </summary>
	private PlayerMovement playerMovement;

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		score = 0.0f;
		anim = GetComponent<Animation> ();
		playerMovement = gameObject.GetComponent<PlayerMovement> ();
		gridManager.Attack += new EventHandler (AttackHandler);
	}

	/// <summary>
	/// THe coroutine of physical attack.
	/// </summary>
	/// <returns>The attack coroutine.</returns>
	/// <param name="enemy">Enemy.</param>
	IEnumerator PhysicalAttackCoroutine(GameObject enemy)
	{
		anim.Stop ();
		anim.Play ("Attack");

		yield return new WaitForSeconds(0.2f);

		EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//		SlimeHealth slimeHealth = enemyHealth as SlimeHealth;

//		if (slimeHealth != null)
//			slimeHealth.ReceiveDamage (Constants.PhysicalDamage);
//		else
			enemyHealth.ReceiveDamage (score * Constants.PhysicalDamage);
	}

	/// <summary>
	/// Raises the collision enter event.
	/// Attack when collide the enemy.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Enemy" && gameManager.isPlayerTurn && playerMovement.IsInMotion)
		{		
			playerMovement.IsInMotion = false;
			StartCoroutine (PhysicalAttackCoroutine (other.gameObject));
		}
	}

	/// <summary>
	/// Handle the attack event sent from the grid.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void AttackHandler(object sender, EventArgs e)
	{
		playerMovement.IsInMotion = true;
		ScoreEventArgs scoreEvent = e as ScoreEventArgs;
		score = scoreEvent.Score;
		Debug.Log ("Score received: " + scoreEvent.Score.ToString ());
	}

	#endregion
}
