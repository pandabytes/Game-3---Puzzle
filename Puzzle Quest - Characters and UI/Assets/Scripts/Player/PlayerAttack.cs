using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour 
{
	#region Member variables

	/// <summary>
	/// The enemy object.
	/// </summary>
	public GameObject enemy;

	/// <summary>
	/// The enemy health.
	/// </summary>
	public EnemyHealth enemyHealth;

	/// <summary>
	/// Player's speed.
	/// </summary>
	public float speed;

	/// <summary>
	/// The game manager.
	/// </summary>
	public GameManager gameManager;

	/// <summary>
	/// The start position of player.
	/// </summary>
	private Vector3 startPosition;

	/// <summary>
	/// Flag indicate whether player is in motion (aka moving).
	/// </summary>
	private bool isInMotion;

	/// <summary>
	/// The animation component of the player.
	/// </summary>
	private Animation anim;

	/// <summary>
	/// The player health.
	/// </summary>
	private PlayerHealth playerHealth;

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		startPosition = transform.position;
		isInMotion = false;
		anim = GetComponent<Animation> ();
		playerHealth = gameObject.GetComponent<PlayerHealth> ();
		enemyHealth = enemy.GetComponent<EnemyHealth> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space) && gameManager.isPlayerTurn &&
		    playerHealth.CurrentHealth > 0.0f && enemyHealth.CurrentHealth > 0.0f)
		{
			isInMotion = true;
		}

		MoveTowardToEnemy ();
		ResetToStartPosition ();
	}

	/// <summary>
	/// Move toward to the enemy.
	/// </summary>
	private void MoveTowardToEnemy()
	{
		if (isInMotion)
		{
			anim.Play ("Walk");
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, enemy.transform.position, step);
		}
	}

	/// <summary>
	/// Reset the character to its orignal starting position after an attack.
	/// Move back to start position every frame.
	/// </summary>
	private void ResetToStartPosition()
	{
		if (!anim.IsPlaying("Attack") && !isInMotion && transform.position != startPosition)
		{
			float step = 30 * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, startPosition, step);
		}
	}

	/// <summary>
	/// THe coroutine of physical attack.
	/// </summary>
	/// <returns>The attack coroutine.</returns>
	/// <param name="enemy">Enemy.</param>
	IEnumerator PhysicalAttackCoroutine(GameObject enemy)
	{
		isInMotion = false;
		anim.Stop ();
		anim.Play ("Attack");

		yield return new WaitForSeconds(0.2f);

		EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
		enemyHealth.ReceiveDamage (Constants.PhysicalDamage);
	}

	/// <summary>
	/// Raises the collision enter event.
	/// Attack when collide the enemy.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Enemy" && gameManager.isPlayerTurn)
		{
			StartCoroutine (PhysicalAttackCoroutine (other.gameObject));
		}
	}

	#endregion
}
