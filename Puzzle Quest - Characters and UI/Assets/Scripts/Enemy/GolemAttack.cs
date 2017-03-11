using UnityEngine;
using System;
using System.Collections;

public class GolemAttack : EnemyAttack
{
	#region Member Variables

	/// <summary>
	/// The golem health.
	/// </summary>
	private GolemHealth golemHealth;

	/// <summary>
	/// The amount of damage to reduce by when hit by EarthSpike spell.
	/// </summary>
	private float reduceAttackDamage;

	/// <summary>
	/// Count the number of ice shards when hit.
	/// </summary>
	private int iceCount;

	/// <summary>
	/// Indicate if the boss is frozen or not.
	/// </summary>
	private bool isFrozen;

	/// <summary>
	/// The frozen ice.
	/// </summary>
	public GameObject frozenIce;

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start ()
	{
		reduceAttackDamage = 0.0f;
		isFrozen = false;
		player = GameObject.FindGameObjectWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();

		startPosition = transform.position;
		isInMotion = false;
		golemHealth = gameObject.GetComponent<GolemHealth> ();
		anim = gameObject.GetComponent<Animation> ();

		timer.TimesUp += new EventHandler (TimesUpHandler);
	}

	// Update is called once per frame
	void Update ()
	{
		if (golemHealth.CurrentHealth > 0.0f && !gameManager.isPlayerTurn && isInMotion && !isFrozen)
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
		anim.Play ("walk");
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, step);
	}

	/// <summary>
	/// Reset the enemy to its orignal starting position after an attack.
	/// Move back to start position every frame.
	/// </summary>
	protected override void ResetToStartPosition()
	{
		if (!anim.IsPlaying("hit2") && !isInMotion && transform.position != startPosition)
		{
			float step = 30 * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, startPosition, step);

			// Send a notifcation to indicate that the enemy's turn has completed.
			// Enemy doesn't need a countdown 
			if (transform.position == startPosition)
			{
				timer.Second = Constants.TimeLimit;
				timer.StopTimer = false;
				isInMotion = true;
				timer.OnTimesUp (!gameManager.isPlayerTurn, EventArgs.Empty);
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
		anim.Stop ();
		anim.Play ("hit2");

		yield return new WaitForSeconds(0.4f);
		playerHealth.ReceiveDamage (Constants.BossDamage - reduceAttackDamage);

		if (reduceAttackDamage != 0.0f)
		{
			reduceAttackDamage = 0.0f;
		}
	}

	/// <summary>
	/// Unfreezes the boss coroutine.
	/// </summary>
	/// <returns>The boss coroutine.</returns>
	private IEnumerator SwitchTurnCoroutine()
	{
		// After 7.5 seconds, switch turn
		yield return new WaitForSeconds (7.5f);
		timer.Second = Constants.TimeLimit;
		timer.StopTimer = false;
		isInMotion = true;
		timer.OnTimesUp (!gameManager.isPlayerTurn, EventArgs.Empty);
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
		else if (other.gameObject.tag == "Rock" && gameManager.isPlayerTurn)
		{
			// Display the spell effect
			reduceAttackDamage = Constants.BossDamage / 2;
			gameManager.uiManager.DisplaySpellEffect (Constants.EarthSpike);

			PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
			golemHealth.ReceiveDamage (playerAttack.Score * Constants.EarthSpikeDamage);
		}
		else if (other.gameObject.tag == "Ice" && gameManager.isPlayerTurn)
		{
			PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
			golemHealth.ReceiveDamage (playerAttack.Score * Constants.IceShardDamage);
			Destroy (other.gameObject);

			iceCount++;

			// Display spell effect after the last ice shard is destroyed and make boss frozen
			if (iceCount == 10)
			{
				if (golemHealth.CurrentHealth > 0.0f)
				{
					iceCount = 0;
					isFrozen = true;
					frozenIce.SetActive (true);
				}

				// Display the spell effect
				gameManager.uiManager.DisplaySpellEffect (Constants.IceShard);

				StartCoroutine (SwitchTurnCoroutine ());
			}
		}
	}

	/// <summary>
	/// Handler for when the time is up.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected override void TimesUpHandler(object sender, EventArgs e)
	{
		bool isPlayerTurn = (bool)sender;
		isInMotion = !isInMotion;

		// Unfreeze the boss when the next turn is player's
		if (isPlayerTurn)
		{
			isFrozen = false;
			frozenIce.SetActive (false);
		}
	}

	#endregion
}

