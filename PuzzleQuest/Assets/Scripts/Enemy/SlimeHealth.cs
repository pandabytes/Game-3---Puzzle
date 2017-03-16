using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class SlimeHealth : EnemyHealth
{
	#region Member variables

	/// <summary>
	/// The slime full health.
	/// </summary>
	public float slimeFullHealth;

	/// <summary>
	/// The slime current health.
	/// </summary>
	[SyncVar(hook = "OnCurrentHealth")]
	private float slimeCurrentHealth;

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

	public override float CurrentHealth 
	{
		get { return slimeCurrentHealth; }
	}

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		slimeCurrentHealth = slimeFullHealth;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{}

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

		slimeCurrentHealth = (slimeCurrentHealth - damage < 0.0f) ? 0.0f : slimeCurrentHealth - damage;
		float scaledDamage = slimeCurrentHealth / slimeFullHealth;
		SetHealth (scaledDamage);

		// Reset enemy color/texture afer 0.1 second
		yield return new WaitForSeconds(0.1f);
		model.GetComponent<Renderer> ().material.mainTexture = originalTexture;
	}

	/// <summary>
	/// Call this when current health is changed on the server.
	/// Update the health bar on the client side.
	/// </summary>
	/// <param name="c">C.</param>
	protected override void OnCurrentHealth(float c)
	{
		slimeCurrentHealth = c;
		SetHealth (slimeCurrentHealth / slimeFullHealth);

		if (slimeCurrentHealth <= 0.0f)
		{
			// Notify the game manager that this enemy has been defeated
			OnEnemyDefeated (this, EventArgs.Empty);

			gameObject.SetActive (false);
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
		if (slimeCurrentHealth > 0.0f)
		{
			StartCoroutine (ReceiveDamageCoroutine (damage));
		}

		if (slimeCurrentHealth <= 0.0f)
		{
			// Notify the game manager that this enemy has been defeated
			OnEnemyDefeated (this, EventArgs.Empty);

			gameObject.SetActive (false);
		}
	}

	#endregion
}

