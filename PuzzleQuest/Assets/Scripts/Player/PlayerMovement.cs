﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
	#region Member Variables

	/// <summary>
	/// The player health.
	/// </summary>
	private PlayerHealth playerHealth;

	/// <summary>
	/// The animation component.
	/// </summary>
	private Animation anim;

	/// <summary>
	/// The start position of player.
	/// </summary>
	private Vector3 startPosition;

	/// <summary>
	/// The enemy health.
	/// </summary>
	private EnemyHealth enemyHealth;

	/// <summary>
	/// Flag indicate whether player is in motion (aka moving).
	/// </summary>
	private bool isInMotion;

	/// <summary>
	/// Indicate if the enemy is dead.
	/// </summary>
	private bool isEnemyDead;

	/// <summary>
	/// The enemy object.
	/// </summary>
	public GameObject enemy;

	/// <summary>
	/// Player's speed.
	/// </summary>
	public float speed;

	/// <summary>
	/// The game manager.
	/// </summary>
	public GameManager gameManager;

	/// <summary>
	/// The chest.
	/// </summary>
	public GameObject chest;

	/// <summary>
	/// The timer.
	/// </summary>
	public Timer timer;

	#endregion

	#region Public Getters and Setters

	/// <summary>
	/// Gets or sets a value indicating whether this instance is in motion.
	/// </summary>
	/// <value><c>true</c> if this instance is in motion; otherwise, <c>false</c>.</value>
	public bool IsInMotion
	{
		get { return isInMotion; }
		set { isInMotion = value; }
	}

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start ()
	{
		playerHealth = gameObject.GetComponent<PlayerHealth> ();
		anim = gameObject.GetComponent<Animation> ();
		startPosition = transform.position;

		isInMotion = false;
		isEnemyDead = false;
		enemyHealth = enemy.GetComponent<EnemyHealth> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isEnemyDead)
		{
			anim.Play ("Wait");
			return;
		}

		// Play the "wait" anination here
		if (playerHealth.CurrentHealth > 0.0f &&
		    !(anim.IsPlaying ("Attack") || anim.IsPlaying ("Damage") || anim.IsPlaying ("Walk")))
		{
			anim.Play ("Wait");
		}

		// Once it's the player's turn, move the character toward the enemy
		if (isInMotion && gameManager.isPlayerTurn && playerHealth.CurrentHealth > 0.0f && enemyHealth.CurrentHealth > 0.0f)
		{
			CmdMoveTowardToEnemy ();
		}
		CmdResetToStartPosition ();
	}

	/// <summary>
	/// Move toward to the enemy.
	/// </summary>

	private void CmdMoveTowardToEnemy()
	{
		anim.Play ("Walk");
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, enemy.transform.position, step);
	}

	/// <summary>
	/// Reset the character to its orignal starting position after an attack.
	/// Move back to start position every frame.
	/// </summary>

	private void CmdResetToStartPosition()
	{
		if (!anim.IsPlaying("Attack") && !isInMotion && transform.position != startPosition)
		{
			float step = 30 * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, startPosition, step);
		}

		if (transform.position == startPosition)
		{
			isInMotion = false;
		}
	}

	#endregion
}
