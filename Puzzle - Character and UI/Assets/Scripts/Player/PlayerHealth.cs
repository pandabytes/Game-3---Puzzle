using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour 
{
	#region Member variables

	/// <summary>
	/// Full health
	/// </summary>
	public float fullHealth;

	/// <summary>
	/// The current health.
	/// </summary>
	private float currentHealth;

	/// <summary>
	/// Indicate whether the player is damaged.
	/// </summary>
	private bool isDamaged;

	/// <summary>
	/// The color of the flash.
	/// </summary>
	private Color flashColor;

	/// <summary>
	/// The animation component.
	/// </summary>
	private Animation anim;

	/// <summary>
	/// Reference to the Health Bar object
	/// </summary>
	public GameObject healthBar;

	/// <summary>
	/// Flash this image when damage.
	/// </summary>
	public Image damageImage;

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
		fullHealth = 100.0f;
		currentHealth = fullHealth;
		anim = gameObject.GetComponent<Animation> ();

		isDamaged = false;
		damageImage.color = Color.clear;
		damageImage.gameObject.SetActive (true);
		flashColor = new Color (1.0f, 0.0f, 0.0f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (currentHealth > 0.0f && !isDamaged && !(anim.IsPlaying ("Attack") || anim.IsPlaying ("Damage") || anim.IsPlaying ("Walk")))
			anim.Play ("Wait");
		
		if (isDamaged)
		{
			damageImage.color = flashColor;
		}
		else
		{
			damageImage.color = Color.clear;
		}
		isDamaged = false;
	}

	/// <summary>
	/// Sets the health.
	/// </summary>
	/// <param name="scaledDamage">Scaled damage.</param>
	private void SetHealth(float scaledDamage)
	{
		float y = healthBar.transform.localScale.y;
		float z = healthBar.transform.localScale.z;
		healthBar.transform.localScale = new Vector3 (scaledDamage, y, z);	
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Receives the damage.
	/// </summary>
	/// <param name="damage">Damage amount.</param>
	public void ReceiveDamage(float damage)
	{
		isDamaged = true;
		anim.Stop ();
		anim.Play ("Damage");

		currentHealth = (currentHealth - damage < 0.0f) ? 0.0f : currentHealth - damage;
		float scaledDamage = currentHealth / fullHealth;
		SetHealth (scaledDamage);

		if (currentHealth <= 0)
		{
			anim.Stop ();
			anim.Play ("Dead");
			OnPlayerDeath (this, EventArgs.Empty);
		}
	}

	#endregion

	#region Public Events

	/// <summary>
	/// Occurs when player dies;
	/// </summary>
	public event EventHandler PlayerDeath;

	/// <summary>
	/// Send notification when the player dies.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void OnPlayerDeath(object sender, EventArgs e)
	{
		PlayerDeath (sender, e);
	}

	#endregion
}
