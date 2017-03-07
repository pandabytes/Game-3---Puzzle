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
		player = GameObject.FindGameObjectWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();
		playerHealth.PlayerDeath += new EventHandler (PlayerDeathHandler);

		startPosition = transform.position;
		isInMotion = false;
		slimeHealth = gameObject.GetComponent<SlimeHealth> ();

		InvokeRepeating ("EnableAttack", 2.0f, 6.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (slimeHealth.CurrentHealth > 0.0f && !gameManager.isPlayerTurn)
		{
			Attack ();
		}
	}

	/// <summary>
	/// Move toward to the player.
	/// </summary>
	protected override void MoveTowardToPlayer()
	{
		if (isInMotion)
		{
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, player.transform.position, step);
		}
	}

	/// <summary>
	/// Reset the character to its orignal starting position after an attack.
	/// Move back to start position every frame.
	/// </summary>
	protected override void ResetToStartPosition()
	{
		if (!isInMotion && transform.position != startPosition)
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
	protected override IEnumerator PhysicalAttackCoroutine()
	{
		isInMotion = false;

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
		}
		else if (other.gameObject.tag == "Rock" && gameManager.isPlayerTurn)
		{
			slimeHealth.ReceiveDamage (Constants.EarthSpikeDamage);
		}
	}

	#endregion
}

