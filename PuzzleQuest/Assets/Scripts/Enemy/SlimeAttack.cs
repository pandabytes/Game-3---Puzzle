using UnityEngine;
using System;
using System.Collections;

public class SlimeAttack : EnemyAttack
{
	#region Member Variables

	/// <summary>
	/// The slime health.
	/// </summary>
	private SlimeHealth slimeHealth;

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start ()
	{
		playerHealth = player.GetComponent<PlayerHealth> ();

		startPosition = transform.position;
		isInMotion = false;
		slimeHealth = gameObject.GetComponent<SlimeHealth> ();

		timer.EventTimesUp += TimesUpHandler;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (slimeHealth.CurrentHealth > 0.0f && !gameManager.isPlayerTurn && isInMotion)
		{
			MoveTowardToPlayer ();
		}
		ResetToStartPosition ();
	}

	/// <summary>
	/// Move toward to the player.
	/// </summary>
	protected override void MoveTowardToPlayer()
	{
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, step);
	}

	/// <summary>
	/// Reset the enemy to its orignal starting position after an attack.
	/// Move back to start position every frame.
	/// </summary>
	protected override void ResetToStartPosition()
	{
		if (!isInMotion && transform.position != startPosition)
		{
			float step = 30 * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, startPosition, step);

			// Only server can send a notifcation to indicate that the enemy's turn has completed.
			// Enemy doesn't need a countdown 
			if (transform.position == startPosition)
			{
				timer.Second = Constants.TimeLimit;
				timer.StopTimer = false;
				timer.SetTimerText ();

				if (isServer)
				{
					isInMotion = true;
					timer.OnTimesUp (!gameManager.isPlayerTurn);
				}
				else
				{
					isInMotion = false;
				}

			}
		}
	}

	/// <summary>
	/// The coroutine of physical attack.
	/// </summary>
	/// <returns>The attack coroutine.</returns>
	/// <param name="player">player.</param>
	protected override IEnumerator PhysicalAttackCoroutine()
	{
		isInMotion = false;

		yield return new WaitForSeconds(0.03f);
		playerHealth.ReceiveDamage (Constants.SlimeDamage);
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
	}

	#endregion
}

