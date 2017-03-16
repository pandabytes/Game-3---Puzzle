using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class GolemHealth : EnemyHealth
{
	#region Member variables

	/// <summary>
	/// The golem full health.
	/// </summary>
	public float golemFullHealth;

	/// <summary>
	/// The golem current health.
	/// </summary>
	[SyncVar(hook = "OnCurrentHealth")]
	public float golemCurrentHealth;

	/// <summary>
	/// The original material of the enemy.
	/// </summary>s
	public Texture originalTexture;

	/// <summary>
	/// The damage texture for the enemy.
	/// </summary>
	public Texture damageTexture;

	#endregion

	#region Getters and Setters 

	/// <summary>
	/// Gets the current health.
	/// </summary>
	/// <value>The current health.</value>
	public override float CurrentHealth 
	{
		get { return golemCurrentHealth; }
	}

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		golemCurrentHealth = golemFullHealth;
		anim = gameObject.GetComponent<Animation> ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if (golemCurrentHealth > 0.0f && !(anim.IsPlaying ("hit2") || anim.IsPlaying ("damage") || anim.IsPlaying ("walk") || anim.IsPlaying("rage")))
			anim.Play ("idle");
	}

	/// <summary>
	/// Execute the receive damage coroutine
	/// </summary>
	/// <returns>The damage coroutine.</returns>
	/// <param name="damage">Damage amount.</param>
	protected override IEnumerator ReceiveDamageCoroutine(float damage)
	{
		// Set to damage color/texture
		model.GetComponent<Renderer>().material.mainTexture = damageTexture;

		if (!isServer)
		{
			yield return new WaitForSeconds(0.1f);
			model.GetComponent<Renderer> ().material.mainTexture = originalTexture;
			yield break;
		}

		golemCurrentHealth = (golemCurrentHealth - damage < 0.0f) ? 0.0f : golemCurrentHealth - damage;
		float scaledDamage = golemCurrentHealth / golemFullHealth;
		SetHealth (scaledDamage);

		// Reset enemy color/texture afer 0.1 second
		yield return new WaitForSeconds(0.1f);
		model.GetComponent<Renderer> ().material.mainTexture = originalTexture;
	}

	/// <summary>
	/// Enemy death coroutine
	/// </summary>
	/// <returns>The death coroutine.</returns>
	protected override IEnumerator EnemyDeathCoroutine()
	{
		// Play the dead animation
		anim.Stop ();
		anim.Play ("die");

		yield return new WaitForSeconds (3.0f);

		// Notify the game manager that this enemy has been defeated
		OnEnemyDefeated (this, EventArgs.Empty);

		gameObject.SetActive (false);
	}

	/// <summary>
	/// Call this when current health is changed on the server.
	/// Update the health bar on the client side.
	/// </summary>
	/// <param name="c">C.</param>
	protected override void OnCurrentHealth(float c)
	{
		golemCurrentHealth = c;
		SetHealth (golemCurrentHealth / golemFullHealth);

		if (golemCurrentHealth <= 0.0f)
		{
			StartCoroutine (EnemyDeathCoroutine ());
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Receives the damage.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public override void ReceiveDamage(float damage)
	{
		if (golemCurrentHealth > 0.0f)
		{
			anim.Stop ();
			anim.Play ("damage");
			StartCoroutine (ReceiveDamageCoroutine (damage));
		}

		if (golemCurrentHealth <= 0.0f && isServer)
		{
			StartCoroutine (EnemyDeathCoroutine ());
		}
	}

	#endregion
}

