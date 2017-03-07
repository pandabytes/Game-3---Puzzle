﻿using UnityEngine;
using System;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
	#region Member Variables

	/// <summary>
	/// Enemy's speed.
	/// </summary>
	public float speed;

	/// <summary>
	/// The game manager.
	/// </summary>
	public GameManager gameManager;

	/// <summary>
	/// The player object.
	/// </summary>
	protected GameObject player;

	/// <summary>
	/// The player health.
	/// </summary>
	protected PlayerHealth playerHealth;

	/// <summary>
	/// The start position of enemy.
	/// </summary>
	protected Vector3 startPosition;

	/// <summary>
	/// Flag indicate whether enemy is in motion (aka moving).
	/// </summary>
	protected bool isInMotion;

	/// <summary>
	/// The animation component of the enemy.
	/// </summary>
	private Animation anim;

	/// <summary>
	/// The enemy health.
	/// </summary>
	private EnemyHealth enemyHealth;

	#endregion 

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();
		playerHealth.PlayerDeath += new EventHandler (PlayerDeathHandler);

		startPosition = transform.position;
		isInMotion = false;
		anim = gameObject.GetComponent<Animation> ();
		enemyHealth = gameObject.GetComponent<EnemyHealth> ();

		InvokeRepeating ("EnableAttack", 2.0f, 6.0f);
	}

	// Update is called once per frame
	void Update () 
	{
		if (enemyHealth.CurrentHealth > 0.0f && !gameManager.isPlayerTurn)
		{
			Attack ();
		}
	}

	/// <summary>
	/// Enables enemy attack.
	/// </summary>
	protected void EnableAttack()
	{
		isInMotion = true;
	}

	/// <summary>
	/// Begin the attack process.
	/// </summary>
	protected void Attack()
	{
		MoveTowardToPlayer ();
		ResetToStartPosition ();	
	}

	/// <summary>
	/// Move toward to the player.
	/// </summary>
	protected virtual void MoveTowardToPlayer()
	{
		if (isInMotion)
		{
			anim.Play ("Walk");
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, player.transform.position, step);
		}
	}

	/// <summary>
	/// Reset the character to its orignal starting position after an attack.
	/// Move back to start position every frame.
	/// </summary>
	protected virtual void ResetToStartPosition()
	{
		if (!anim.IsPlaying("Attack") && !isInMotion && transform.position != startPosition)
		{
			float step = 30 * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, startPosition, step);
		}
	}

	/// <summary>
	/// The coroutine of physical attack.
	/// </summary>
	/// <returns>The attack coroutine.</returns>
	/// <param name="player">player.</param>
	protected virtual IEnumerator PhysicalAttackCoroutine()
	{
		isInMotion = false;
		anim.Stop ();
		anim.Play ("Attack");

		yield return new WaitForSeconds(0.3f);
		playerHealth.ReceiveDamage (Constants.PhysicalDamage);
	}

	/// <summary>
	/// Raises the collision enter event.
	/// Attack when collide the player.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && !gameManager.isPlayerTurn && isInMotion)
		{
			isInMotion = false;
			StartCoroutine (PhysicalAttackCoroutine ());
		}
		else if (other.gameObject.tag == "Shield" && !gameManager.isPlayerTurn)
		{
			isInMotion = false;
			anim.Stop ();
		}
	}

	/// <summary>
	/// Handle the player death event.
	/// Stop the enemy from attacking once player is dead.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void PlayerDeathHandler(object sender, EventArgs e)
	{
		CancelInvoke ();
	}

	#endregion
}

