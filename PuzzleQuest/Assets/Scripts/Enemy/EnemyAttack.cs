using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class EnemyAttack : NetworkBehaviour
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
	/// The timer.
	/// </summary>
	public Timer timer;

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
	[SyncVar]
	protected bool isInMotion;

	/// <summary>
	/// The animation component of the enemy.
	/// </summary>
	protected Animation anim;

	/// <summary>
	/// The enemy health.
	/// </summary>
	protected EnemyHealth enemyHealth;

	#endregion 

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();

		startPosition = transform.position;
		isInMotion = false;
		anim = gameObject.GetComponent<Animation> ();
		enemyHealth = gameObject.GetComponent<EnemyHealth> ();

		timer.EventTimesUp += TimesUpHandler;
	}

	// Update is called once per frame
	void Update () 
	{
		if (enemyHealth.CurrentHealth > 0.0f && !gameManager.isPlayerTurn && isInMotion)
		{
			MoveTowardToPlayer ();
		}
		ResetToStartPosition ();
	}

	/// <summary>
	/// Move toward to the player.
	/// </summary>
	protected virtual void MoveTowardToPlayer()
	{
		anim.Play ("Walk");
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, step);
	}

	/// <summary>
	/// Reset the enemy to its orignal starting position after an attack.
	/// Move back to start position every frame.
	/// </summary>
	protected virtual void ResetToStartPosition()
	{
		if (!anim.IsPlaying("Attack") && !isInMotion && transform.position != startPosition)
		{
			float step = 30 * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, startPosition, step);

			// Only server can send a notifcation to indicate that the enemy's turn has completed.
			// Enemy doesn't need a countdown 
			if (transform.position == startPosition)
			{
				timer.Second = Constants.TimeLimit;
				timer.StopTimer = false;

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
	protected virtual IEnumerator PhysicalAttackCoroutine()
	{
		isInMotion = false;
		anim.Stop ();
		anim.Play ("Attack");

		yield return new WaitForSeconds(0.3f);
		playerHealth.CmdReceiveDamage (Constants.EnemyDamage);
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

	/// <summary>
	/// Handler for when the time is up.
	/// </summary>
	/// <param name="isPlayerTurn">Boolean indicate whose turn is next</param>
	protected virtual void TimesUpHandler(bool isPlayerTurn)
	{
		// For client
		if (isClient)
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
		isInMotion = !isInMotion;
	}

	#endregion
}

