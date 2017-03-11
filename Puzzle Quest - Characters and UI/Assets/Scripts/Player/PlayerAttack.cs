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

	#region Getters and Setters

	/// <summary>
	/// Gets the score.
	/// </summary>
	/// <value>The score.</value>
	public float Score
	{
		get { return score; }
	}

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
		enemyHealth.ReceiveDamage (score * Constants.PhysicalDamage);
	}

	/// <summary>
	/// Raises the collision enter event.
	/// Attack when collide the enemy.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Enemy" && playerMovement.IsInMotion)
		{		
			playerMovement.IsInMotion = false;
			StartCoroutine (PhysicalAttackCoroutine (other.gameObject));
		}
	}

	/// <summary>
	/// Randomly chooses an attack to execute.
	/// </summary>
	/// <returns>The attack.</returns>
	/// <param name="stage">Stage.</param>
	private string ChooseAttack(StageEnum stage)
	{
		int rand = UnityEngine.Random.Range (1, 11);

		if (stage == StageEnum.FirstStage)
		{
			return Constants.PhysicalAttack;
		}
		else if (stage == StageEnum.SecondStage)
		{
			return (rand <= 3) ? Constants.FireBall : Constants.PhysicalAttack;
		}
		else if (stage == StageEnum.ThirdStage)
		{
			if (rand >= 5)
				return Constants.PhysicalAttack;
			else if (rand == 3 || rand == 4)
				return Constants.FireBall;
			else
				return Constants.EarthSpike;
		}
		else
		{
			if (rand >= 4)
				return Constants.PhysicalAttack;
			else if (rand == 1)
				return Constants.FireBall;
			else
				return Constants.EarthSpike;
//			else if (rand == 2)
//				return Constants.EarthSpike;
//			else
//				return Constants.IceShard;
		}
	}

	/// <summary>
	/// Handle the attack event sent from the grid.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void AttackHandler(object sender, EventArgs e)
	{
		ScoreEventArgs scoreEvent = e as ScoreEventArgs;
		score = scoreEvent.Score;
		string attack = ChooseAttack (gameManager.stage);

		if (gameManager.stage == StageEnum.FourthStage)
		{
			anim.Play ("Attack");
			EarthSpikeAttack earthSpikeSpell = gameObject.GetComponent<EarthSpikeAttack>();
			earthSpikeSpell.RaiseRocks ();
			return;
		}

		if (attack == Constants.PhysicalAttack)
		{
			playerMovement.IsInMotion = true;
		}
		else if (attack == Constants.FireBall)
		{
			anim.Play ("Attack");
			FireBallAttack fireBallSpell = gameObject.GetComponent<FireBallAttack> ();
			fireBallSpell.ShootFireBall ();
		}
		else if (attack == Constants.EarthSpike)
		{
			anim.Play ("Attack");
			EarthSpikeAttack earthSpikeSpell = gameObject.GetComponent<EarthSpikeAttack>();
			earthSpikeSpell.RaiseRocks ();
		}
		else
		{
			// TODO: Ice Shard

		}
	}

	#endregion
}
