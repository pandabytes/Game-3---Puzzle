using UnityEngine;
using System;
using System.Collections;

public class GolemHealth : EnemyHealth
{
	#region Member variables

	/// <summary>
	/// The original material of the enemy.
	/// </summary>s
	public Texture originalTexture;

	/// <summary>
	/// The damage texture for the enemy.
	/// </summary>
	public Texture damageTexture;

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		currentHealth = fullHealth;
		anim = gameObject.GetComponent<Animation> ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if (currentHealth > 0.0f && !(anim.IsPlaying ("hit2") || anim.IsPlaying ("damage") || anim.IsPlaying ("walk") || anim.IsPlaying("rage")))
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

		currentHealth = (currentHealth - damage < 0.0f) ? 0.0f : currentHealth - damage;
		float scaledDamage = currentHealth / fullHealth;
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

	#endregion

	#region Public Methods

	/// <summary>
	/// Receives the damage.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public override void ReceiveDamage(float damage)
	{
		if (currentHealth > 0.0f)
		{
			anim.Stop ();
			anim.Play ("damage");
			StartCoroutine (ReceiveDamageCoroutine (damage));
		}

		if (currentHealth <= 0.0f)
		{
			StartCoroutine (EnemyDeathCoroutine ());
		}
	}

	#endregion
}

