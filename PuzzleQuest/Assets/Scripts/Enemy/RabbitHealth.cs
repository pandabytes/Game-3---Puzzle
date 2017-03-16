using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class RabbitHealth : EnemyHealth
{
	#region Member variables

	/// <summary>
	/// The rabbit full health.
	/// </summary>
	public float rabbitFullHealth;

	/// <summary>
	/// The rabbit current health.
	/// </summary>
	[SyncVar(hook = "OnCurrentHealth")]
	private float rabbitCurrentHealth;

	/// <summary>
	/// The original texture of the enemy.
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
		get { return rabbitCurrentHealth; }
	}

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		rabbitCurrentHealth = rabbitFullHealth;
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

		rabbitCurrentHealth = (rabbitCurrentHealth - damage < 0.0f) ? 0.0f : rabbitCurrentHealth - damage;
		float scaledDamage = rabbitCurrentHealth / rabbitFullHealth;
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
		rabbitCurrentHealth = c;
		SetHealth (rabbitCurrentHealth / rabbitFullHealth);

		if (rabbitCurrentHealth <= 0.0f)
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
		if (rabbitCurrentHealth > 0.0f)
		{
			StartCoroutine (ReceiveDamageCoroutine (damage));
		}

		if (rabbitCurrentHealth <= 0.0f)
		{
			// Notify the game manager that this enemy has been defeated
			OnEnemyDefeated (this, EventArgs.Empty);

			gameObject.SetActive (false);
		}
	}

	#endregion
}

