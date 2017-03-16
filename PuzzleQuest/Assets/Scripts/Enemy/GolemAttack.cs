using UnityEngine;
using UnityEngine.Networking;
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
	[SyncVar]
	private int iceCount;

	/// <summary>
	/// The meteors attack.
	/// </summary>
	private MeteorsAttack meteorsAttack;

	/// <summary>
	/// Indicate if the boss is frozen or not.
	/// </summary>
	[SyncVar(hook = "OnIsFrozen")]
	private bool isFrozen;

	/// <summary>
	/// Indicate if the attak is physical attack or meteor.
	/// </summary>
	[SyncVar]
	private bool usePhysicalAttack;

	/// <summary>
	/// The player network.
	/// </summary>
	private PlayerNetwork playerNetwork1;
	private PlayerNetwork playerNetwork2;

	/// <summary>
	/// Boss's roar.
	/// </summary>
	private AudioSource roarAudio;

	/// <summary>
	/// The frozen ice.
	/// </summary>
	public GameObject frozenIce;

	#endregion

	#region Getters and Setters

	/// <summary>
	/// Gets the reduce attack damage.
	/// </summary>
	/// <value>The reduce attack damage.</value>
	public float ReduceAttackDamage
	{
		get { return reduceAttackDamage; }
		set { reduceAttackDamage = value; }
	}

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
		// PlayerNetwork1 has the local authority, PlayerNetwork2 doesn't
		playerNetwork1 = GameObject.FindGameObjectsWithTag ("Lobby Player") [0].GetComponent<PlayerNetwork>();
		if (!playerNetwork1.hasAuthority)
		{
			playerNetwork1 = GameObject.FindGameObjectsWithTag ("Lobby Player") [1].GetComponent<PlayerNetwork> ();
			playerNetwork2 = GameObject.FindGameObjectsWithTag ("Lobby Player") [0].GetComponent<PlayerNetwork> ();
		}
		else
		{
			playerNetwork2 = GameObject.FindGameObjectsWithTag ("Lobby Player") [1].GetComponent<PlayerNetwork> ();
		}

		playerNetwork1.golemAttack = this;
		playerNetwork2.golemAttack = this;

		reduceAttackDamage = 0.0f;
		isFrozen = false;
		usePhysicalAttack = true;
		player = GameObject.FindGameObjectWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();

		startPosition = transform.position;
		isInMotion = false;
		golemHealth = gameObject.GetComponent<GolemHealth> ();
		anim = gameObject.GetComponent<Animation> ();
		meteorsAttack = gameObject.GetComponent<MeteorsAttack> ();
		roarAudio = gameObject.GetComponent<AudioSource> ();

		timer.EventTimesUp += TimesUpHandler;
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
		anim.Stop ();
		anim.Play ("hit2");

		yield return new WaitForSeconds(0.4f);
		playerHealth.ReceiveDamage (Constants.GolemDamage - reduceAttackDamage);
	}

	/// <summary>
	/// Switch turn coroutine.
	/// </summary>
	/// <returns>The switch turn coroutine.</returns>
	private IEnumerator SwitchTurnCoroutine(float timeToWait)
	{		
		// After 7.5 seconds, switch turn
		yield return new WaitForSeconds (timeToWait);
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

	/// <summary>
	/// Roar coroutine.
	/// </summary>
	/// <returns>The coroutine.</returns>
	private IEnumerator RoarCoroutine()
	{
		anim.Play ("rage");
		roarAudio.volume = 1.0f;
		roarAudio.Play ();

		yield return new WaitForSeconds (1.0f);
		roarAudio.volume = 0.7f;
		yield return new WaitForSeconds (1.0f);
		roarAudio.volume = 0.5f;
		yield return new WaitForSeconds (1.0f);
		roarAudio.volume = 0.3f;
		yield return new WaitForSeconds (1.0f);
		roarAudio.volume = 0.1f;
		yield return new WaitForSeconds (0.5f);
		roarAudio.volume = 0.0f;
	}

	/// <summary>
	/// Meteors attack coroutine.
	/// </summary>
	/// <returns>The attack coroutine.</returns>
	private IEnumerator MeteorsAttackCoroutine()
	{
		yield return new WaitForSeconds (7.0f);

		// Only summon meteors if boss is not frozen
		if (!isFrozen)
		{
			StartCoroutine (RoarCoroutine ());

			// Only let the server spawns the meteors
			if (isServer)
			{
				meteorsAttack.SummonMeteors ();
			}

			yield return new WaitForSeconds (1.2f);

			// Switch turn
			StartCoroutine (SwitchTurnCoroutine (7.0f));
		}
	}

	/// <summary>
	/// Raises the collision enter event.
	/// Attack when collide the player or receive damage from player.
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
			// Reduce the boss's attack damage
			reduceAttackDamage = Constants.GolemDamage / 2;

			// Display the spell effect
			gameManager.uiManager.DisplaySpellEffect (Constants.EarthSpike);

			// Deal damage to golem
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
				// Only freeeze the boss if its HP is above 0 and if it's the server
				if (golemHealth.CurrentHealth > 0.0f && isServer)
				{
					iceCount = 0;
					isFrozen = true;
					frozenIce.SetActive (true);
				}

				// Display the spell effect
				gameManager.uiManager.DisplaySpellEffect (Constants.IceShard);

				// Switch turn
				StartCoroutine (SwitchTurnCoroutine (7.8f));
			}
		}
	}

	/// <summary>
	/// Handler for when the time is up.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected override void TimesUpHandler(bool isPlayerTurn)
	{
		// For client
		if (!isServer)
		{
			if (isPlayerTurn)
			{
				isInMotion = false;
			}
			else
			{
				isInMotion = true;
			}
			return;
		}

		// For server
		usePhysicalAttack = true;
		isInMotion = !isInMotion;

		// Unfreeze the boss when the next turn is player's
		if (isPlayerTurn && isFrozen)
		{
			isFrozen = false;
			frozenIce.SetActive (false);
		}

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="b">If set to <c>true</c> b.</param>
	private void OnIsFrozen(bool b)
	{
		isFrozen = b;
		frozenIce.gameObject.SetActive (b);
	}

	#endregion
}