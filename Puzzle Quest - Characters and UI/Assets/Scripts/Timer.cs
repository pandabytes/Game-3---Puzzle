using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour 
{
	#region Member Variables

	private float second = 0;
	private float minute = 0;
	private float hour = 0;

	private string timeFormat;
	private string secondText;
	private string minuteText;
	private string hourText;

	public Text timeText;
	public GameManager gameManager;

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		timeFormat = "{0}:{1}:{2}";
		secondText = "00";
		minuteText = "00";
		hourText = "00";
	}

	// Update is called once per frame
	void Update () 
	{
		if (second >= 60.0f)
		{
			second = 0.0f;
			minute++;
		}

		if (minute >= 60.0f)
		{
			minute = 0.0f;
			hour++;
		}

		second += Time.deltaTime;
		if ((int)second == 16)
		{
			second = 0.0f;

			// Send notifcation to indicate whose turn is next
			OnTimesUp (!gameManager.isPlayerTurn, EventArgs.Empty);
		}

		SetTimerText ();
	}

	/// <summary>
	/// Sets the timer text.
	/// </summary>
	private void SetTimerText()
	{
		// Second format
		if (second < 10)
		{
			secondText = "0" + ((int)second).ToString ();
		}
		else
		{
			secondText = ((int)second).ToString ();
		}

		// Minute format
		if (minute < 10)
		{
			minuteText = "0" + ((int)minute).ToString ();
		}
		else
		{
			minuteText = ((int)minute).ToString ();
		}

		// Hour format
		if (hour < 10)
		{
			hourText = "0" + ((int)hour).ToString ();
		}
		else
		{
			hourText = ((int)hour).ToString ();
		}
			
		timeText.text = string.Format (timeFormat, hourText, minuteText, secondText);
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Sets the start time.
	/// </summary>
	/// <param name="startSecond">Start second.</param>
	/// <param name="startMinute">Start minute.</param>
	/// <param name="startHour">Start hour.</param>
	public void SetStartTime(float startSecond, float startMinute, float startHour)
	{
		second = startSecond;
		minute = startMinute;
		hour = startHour;
	}

	#endregion

	#region Public Events

	/// <summary>
	/// Occurs when the time is up (15 seconds).
	/// </summary>
	public event EventHandler TimesUp;

	/// <summary>
	/// Raises the times up event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	public virtual void OnTimesUp(object sender, EventArgs e)
	{
		if (TimesUp != null)
		{
			TimesUp (sender, e);
		}
	}

	#endregion
}
