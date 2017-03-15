using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour 
{
	#region Member variables

	/// <summary>
	/// Full health
	/// </summary>
	private float fullHealth;

	/// <summary>
	/// The current health.
	/// </summary>
	[SyncVar]
	private float currentHealth;

	/// <summary>
	/// Indicate whether the player is damaged.
	/// </summary>
	[SyncVar]
	private bool isDamaged;

	/// <summary>
	/// The color of the flash.
	/// </summary>
	[SyncVar]
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

	/// <summary>
	/// The healing aura.
	/// </summary>
	public GameObject healingAura;

	/// <summary>
	/// The grid manager.
	/// </summary>
	public GridManager2 gridManager;

	/// <summary>
	/// The game manager.
	/// </summary>
	public GameManager gameManager;

	/// <summary>
	/// The state of the game.
	/// </summary>
	public GameState gameState;

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

		gridManager.Heal += new EventHandler (HealHandler);
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if (Input.GetKeyDown (KeyCode.H))
			HealPlayer (Constants.HealAmount);
		
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

	/// <summary>
	/// Heal player coroutine.
	/// </summary>
	/// <returns>The player coroutine.</returns>
	/// <param name="healAmount">Heal amount.</param>
	private IEnumerator HealPlayerCoroutine(float healAmount)
	{
		healingAura.SetActive (!healingAura.activeSelf);
		healingAura.transform.GetChild(0).gameObject.GetComponent<ParticleSystem> ().Play ();

		currentHealth = (currentHealth + healAmount >= fullHealth) ? fullHealth : currentHealth + healAmount;
		float scaledHealAmount = currentHealth / fullHealth;
		SetHealth (scaledHealAmount);

		// Turn off healing aura after 3 seconds
		yield return new WaitForSeconds (3.0f);
		healingAura.transform.GetChild(0).gameObject.GetComponent<ParticleSystem> ().Stop();
		yield return new WaitForSeconds (2.0f);
		healingAura.SetActive (!healingAura.activeSelf);
	}

	/// <summary>
	/// Player death coroutine.
	/// </summary>
	/// <returns>The death coroutine.</returns>
	private IEnumerator PlayerDeathCoroutine()
	{
		anim.Stop ();
		anim.Play ("Dead");
		gameState.stage = gameManager.stage;

		// After 0.5 second, load the game over scene
		yield return new WaitForSeconds (0.5f);
		SceneManager.LoadScene ("GameOver");
	}

	/// <summary>
	/// Handle the heal event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void HealHandler(object sender, EventArgs e)
	{
		ScoreEventArgs scoreEvent = e as ScoreEventArgs;
		HealPlayer (Constants.HealAmount * scoreEvent.Score);
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Receives the damage.
	/// </summary>
	/// <param name="damage">Damage amount.</param>
	public void CmdReceiveDamage(float damage)
	{
		if (currentHealth > 0.0f)
		{
			isDamaged = true;
			anim.Stop ();
			anim.Play ("Damage");

			currentHealth = (currentHealth - damage < 0.0f) ? 0.0f : currentHealth - damage;
			float scaledDamage = currentHealth / fullHealth;
			SetHealth (scaledDamage);
		}

		// Indicate when the player is dead.
		if (currentHealth <= 0.0f)
		{
			StartCoroutine (PlayerDeathCoroutine ());
		}
	}

	/// <summary>
	/// Heal the player with a certain amount of HP.
	/// </summary>
	/// <param name="healAmount">Heal amount.</param>
	public void HealPlayer(float healAmount)
	{
		if (gameManager.isPlayerTurn)
		{
			StartCoroutine (HealPlayerCoroutine (healAmount));
		}
	}

	#endregion
}
