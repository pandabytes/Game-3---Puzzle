using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour 
{
	#region Member variables

	/// <summary>
	/// Full health
	/// </summary>
	public float fullHealth;

	/// <summary>
	/// The current health.
	/// </summary>
	protected float currentHealth;

	/// <summary>
	/// Reference to healthBar object
	/// </summary>
	public GameObject healthBar;

	/// <summary>
	/// The animation.
	/// </summary>
	protected Animation anim;

	/// <summary>
	/// The model in which contains the material to be changed when damage.
	/// </summary>
	public GameObject model;

	/// <summary>
	/// The damage color of the enemy.
	/// </summary>
	protected Color damageColor;

	/// <summary>
	/// The original material of the enemy.
	/// </summary>
	public Material originalMaterial;

	#endregion

	#region Getters and Setters

	/// <summary>
	/// Gets the current health.
	/// </summary>
	/// <value>The current health.</value>
	public float CurrentHealth 
	{
		get { return currentHealth; }
	}

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		currentHealth = fullHealth;
		anim = gameObject.GetComponent<Animation> ();

		damageColor = new Color (1.0f, 0.0f, 0.0f, 0.5f);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if (currentHealth > 0.0f && !(anim.IsPlaying ("Attack") || anim.IsPlaying ("Damage") || anim.IsPlaying ("Walk")))
			anim.Play ("Wait");
	}

	/// <summary>
	/// Sets the health.
	/// </summary>
	/// <param name="scaledDamage">Scaled damage.</param>
	protected virtual void SetHealth(float scaledDamage)
	{
		float y = healthBar.transform.localScale.y;
		float z = healthBar.transform.localScale.z;
		healthBar.transform.localScale = new Vector3 (scaledDamage, y, z);	
	}

	/// <summary>
	/// Execute the receive damage coroutine
	/// </summary>
	/// <returns>The damage coroutine.</returns>
	/// <param name="damage">Damage amount.</param>
	protected virtual IEnumerator ReceiveDamageCoroutine(float damage)
	{
		// Set to damage color
		model.GetComponent<Renderer> ().material.color = damageColor;

		currentHealth = (currentHealth - damage < 0.0f) ? 0.0f : currentHealth - damage;
		float scaledDamage = currentHealth / fullHealth;
		SetHealth (scaledDamage);

		// Reset enemy color afer 0.1 second
		yield return new WaitForSeconds(0.1f);
		model.GetComponent<Renderer> ().material = originalMaterial;
	}

	/// <summary>
	/// Enemy death coroutine
	/// </summary>
	/// <returns>The death coroutine.</returns>
	protected virtual IEnumerator EnemyDeathCoroutine()
	{
		// Play the dead animation
		anim.Stop ();
		anim.Play ("Dead");

		yield return new WaitForSeconds (2.5f);

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
	public virtual void ReceiveDamage(float damage)
	{
		if (currentHealth > 0.0f)
		{
			anim.Stop ();
			anim.Play ("Damage");
			StartCoroutine (ReceiveDamageCoroutine (damage));
		}

		if (currentHealth <= 0.0f)
		{
			StartCoroutine (EnemyDeathCoroutine ());
		}
	}

	#endregion

	#region Public Events

	/// <summary>
	/// Occurs when enemy is defeated.
	/// </summary>
	public event EventHandler EnemyDefeated;

	/// <summary>
	/// Raises the enemy defeated event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event data.</param>
	protected virtual void OnEnemyDefeated(object sender, EventArgs e)
	{
		if (EnemyDefeated != null)
		{
			EnemyDefeated (sender, e);
		}
	}

	#endregion 
}
