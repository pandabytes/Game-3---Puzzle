using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

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

		timer.TimesUp += new EventHandler (TimesUpHandler);
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
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void TimesUpHandler(object sender, EventArgs e)
	{
		isInMotion = !isInMotion;
	}

	#endregion
}

