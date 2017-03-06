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

	// Pop up window
	private float savedTimeScale;
	public Image popUpWindow;
	public Button okButton;
	public Text message;

	#endregion

	#region Private Methods

	void Start()
	{
		okButton.onClick.AddListener (ClickOk);
		SetMessage ();
	}

	void Update()
	{
		// Close the pop up window when user preses enter
		if ((Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) && IsPopUpWindowEnabled ())
		{
			ClickOk ();
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
				LoadSecondStageScene ();
				break;
			case (StageEnum.SecondStage):
				break;
		}

	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Loads the first stage scene.
	/// </summary>
	public void LoadFirstStageScene()
	{
		SceneManager.LoadScene ("Stage 1");
	}

	/// <summary>
	/// Loads the second stage scene.
	/// </summary>
	public void LoadSecondStageScene()
	{
		SceneManager.LoadScene ("Stage 2");
	}

	/// <summary>
	/// Quit this instance.
	/// </summary>
	public void Quit()
	{
		Application.Quit ();
	}

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

	#endregion
}

