using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
	#region Member Variables

	/// <summary>
	/// The current stage of the game.
	/// </summary>
	public StageEnum stage;

	/// <summary>
	/// Indicate whose turn is next.
	/// </summary>
	public Text turnText;

	// Grid UI
	public Image backgroundImage;
	public Text player1_instruction;
	public Text player2_instruction;
	public Text instruction;
	public Text countDown;
	public Timer timer;
	public GridManager gridManager;
	public GridManager2 gridManager2;
	private bool gameStart;
	private float time;

	// Pop up window
	private float savedTimeScale;
	public Image popUpWindow;
	public Button okButton;
	public Text message;

	// Spell effects
	public Image spellEffectBackground;
	public Text spellEffectText;


	#endregion

	#region Private Methods

	void Start()
	{
		gameStart = false;
		time = 6.0f;
		okButton.onClick.AddListener (ClickOk);
		SetMessage ();
	}

	void Update()
	{		
		// Close the pop up window when user presses enter
		if ((Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) && IsPopUpWindowEnabled ())
		{
			ClickOk ();
		}

		// Disable the instruction once the coundown reaches to 0
		if (time <= 0.0f && !gameStart)
		{
			backgroundImage.gameObject.SetActive (false);
			player1_instruction.gameObject.SetActive (false);
			player2_instruction.gameObject.SetActive (false);
			instruction.gameObject.SetActive (false);
			countDown.gameObject.SetActive (false);
			timer.enabled = true;

			gridManager.score = 0;
			gridManager2.score = 0;

			gridManager.scoreText.text = "0";
			gridManager2.scoreText.text = "0";

			gameStart = true;
		}
		else if (!gameStart)
		{
			countDown.text = ((int)time).ToString ();
			time -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Determines whether pop up window is enabled or not.
	/// </summary>
	/// <returns><c>true</c> if this instance is pop up window enabled; otherwise, <c>false</c>.</returns>
	private bool IsPopUpWindowEnabled()
	{
		return popUpWindow.gameObject.activeSelf && okButton.gameObject.activeSelf &&
			   message.gameObject.activeSelf;
	}

	/// <summary>
	/// Sets the message depending on the stage.
	/// </summary>
	private void SetMessage()
	{
		switch (stage)
		{
			case (StageEnum.FirstStage):
				message.text = Constants.FireBallMessage;
				break;
			case (StageEnum.SecondStage):
				message.text = Constants.EarthSpikeMessage;
				break;
			case (StageEnum.ThirdStage):
				message.text = Constants.IceShardMessage;
				break;
			case (StageEnum.FourthStage):
				break;
			case(StageEnum.FiftheStage):
				break;
		}
	}

	/// <summary>
	/// Unfreez the game when player clicks ok.
	/// </summary>
	private void ClickOk()
	{
		Time.timeScale = savedTimeScale;
		popUpWindow.gameObject.SetActive (false);
		okButton.gameObject.SetActive (false);
		message.gameObject.SetActive (false);

		switch (stage)
		{
			case (StageEnum.FirstStage):
				SceneManager.LoadScene ("Stage 2");
				break;
			case (StageEnum.SecondStage):
				SceneManager.LoadScene ("Stage 3");
				break;
			case (StageEnum.ThirdStage):
				break;
			case (StageEnum.FourthStage):
				break;
			case(StageEnum.FiftheStage):
				break;
		}
	}

	/// <summary>
	/// Display the turn coroutine.
	/// </summary>
	/// <returns>The turn coroutine.</returns>
	/// <param name="isPlayerTurn">If set to <c>true</c> is player turn.</param>
	private IEnumerator DisplayTurnCoroutine(bool isPlayerTurn)
	{
		turnText.text = (isPlayerTurn) ? "Player's Turn" : "Enemy's Turn";
		turnText.color = (isPlayerTurn) ?  new Color (0.0f, 0.0f, 1.0f) : new Color (1.0f, 0.0f, 0.0f);
		turnText.gameObject.SetActive (true);

		yield return new WaitForSeconds (2.0f);
		turnText.gameObject.SetActive (false);
	}

	/// <summary>
	/// Display the spell effect coroutine.
	/// </summary>
	/// <returns>The spell effect coroutine.</returns>
	/// <param name="spellName">Spell name.</param>
	private IEnumerator DisplaySpellEffectCoroutine(string spellName)
	{
		// Set text effect
		if (spellName == Constants.FireBall)
		{
			spellEffectText.text = Constants.FireBallEffect;
		}
		else if (spellName == Constants.EarthSpike)
		{
			spellEffectText.text = Constants.EarthSpikeEffect;
		}
		else if (spellName == Constants.IceShard)
		{
			spellEffectText.text = Constants.IceShardEffect;
		}

		// Turn the text and background on
		spellEffectBackground.gameObject.SetActive (true);
		spellEffectText.gameObject.SetActive (true);

		// Turn off after 3 seconds
		yield return new WaitForSeconds (3.0f);
		spellEffectBackground.gameObject.SetActive (false);
		spellEffectText.gameObject.SetActive (false);
	}
		
	#endregion

	#region Public Methods

	/// <summary>
	/// Enables the pop up window.
	/// </summary>
	public void EnablePopUpWindow()
	{
		savedTimeScale = Time.timeScale;
		Time.timeScale = 0.0f;
		popUpWindow.gameObject.SetActive (true);
		okButton.gameObject.SetActive (true);
		message.gameObject.SetActive (true);
	}

	/// <summary>
	/// Display the turn.
	/// </summary>
	/// <param name="isPlayerTurn">If set to <c>true</c> is player turn.</param>
	public void DisplayTurn(bool isPlayerTurn)
	{
		StartCoroutine (DisplayTurnCoroutine (isPlayerTurn));
	}

	/// <summary>
	/// Display the spell effect.
	/// </summary>
	/// <param name="spellName">Spell name.</param>
	public void DisplaySpellEffect(string spellName)
	{
		StartCoroutine (DisplaySpellEffectCoroutine (spellName));
	}

	#endregion
}

