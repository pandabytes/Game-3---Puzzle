using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttack : NetworkBehaviour 
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

	/// <summary>
	/// The player network.
	/// </summary>
	private PlayerNetwork playerNetwork1;
	private PlayerNetwork playerNetwork2;

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

		playerNetwork1.playerAttack = this;
		playerNetwork2.playerAttack = this;

		score = 0.0f;
		anim = GetComponent<Animation> ();
		playerMovement = gameObject.GetComponent<PlayerMovement> ();
		gridManager.Attack += new EventHandler (CmdAttackHandler);
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
			return (rand <= 5) ? Constants.FireBall : Constants.PhysicalAttack;
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
			else if (rand == 2)
				return Constants.EarthSpike;
			else
				return Constants.IceShard;
		}
	}

	/// <summary>
	/// Handle the attack event sent from the grid.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void CmdAttackHandler(object sender, EventArgs e)
	{
		if (!isServer)
			return;
		
		ScoreEventArgs scoreEvent = e as ScoreEventArgs;
		score = scoreEvent.Score;
		
		// Randomly choose an attack
		string attackName = ChooseAttack (gameManager.stage);

		// Client attacks
		NetworkIdentity playerNetworkID = gameObject.GetComponent<NetworkIdentity>();
		playerNetwork1.RpcExecuteAttack(attackName, playerNetworkID);

		// Server attacks
		if (attackName == Constants.PhysicalAttack)
		{
			playerMovement.IsInMotion = true;
		}
		else if (attackName == Constants.FireBall)
		{
			anim.Play ("Attack");
			FireBallAttack fireBallSpell = gameObject.GetComponent<FireBallAttack> ();
			fireBallSpell.ShootFireBall ();
		}
		else if (attackName == Constants.EarthSpike)
		{
			anim.Play ("Attack");
			EarthSpikeAttack earthSpikeSpell = gameObject.GetComponent<EarthSpikeAttack>();
			earthSpikeSpell.RaiseRocks ();
		}
		else
		{
			anim.Play ("Attack");
			IceShardsAttack iceShardSpell = gameObject.GetComponent<IceShardsAttack> ();
			iceShardSpell.UnleashIce ();
		}
	}

	#endregion

	/// <summary>
	/// This will run on Client so to perform the same attack as server does
	/// </summary>
	/// <param name="attackName">Attack name.</param>
	/// <param name="playerNetworkID">Player network ID.</param>
	public void ClientAttack(string attackName, NetworkIdentity playerNetworkID)
	{
		GameObject playerServer = ClientScene.FindLocalObject (playerNetworkID.netId);
		if (attackName == Constants.PhysicalAttack)
		{
			playerServer.GetComponent<PlayerMovement> ().IsInMotion = true;
		}
		else if (attackName == Constants.FireBall)
		{
			playerServer.GetComponent<Animation> ().Play ("Attack");
			FireBallAttack fireBallSpell = playerServer.GetComponent<FireBallAttack> ();
			fireBallSpell.ShootFireBall ();
		}
		else if (attackName == Constants.EarthSpike)
		{
			anim.Play ("Attack");
			EarthSpikeAttack earthSpikeSpell = playerServer.GetComponent<EarthSpikeAttack>();
			earthSpikeSpell.RaiseRocks ();
		}
		else
		{
			anim.Play ("Attack");
			IceShardsAttack iceShardSpell = playerServer.GetComponent<IceShardsAttack> ();
			iceShardSpell.UnleashIce ();
		}
	}
}
