using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Timer : NetworkBehaviour 
{
	#region Member Variables

	[SyncVar]
	private float second;

	[SyncVar]
	private bool stopTimer;

	private string timeFormat;
	private string secondText;

	public Text timeText;
	public GameManager gameManager;

	#endregion

	#region Getters and Setters

	public float Second 
	{
		get { return second; }
		set { second = value; }
	}

	public bool StopTimer
	{
		set { stopTimer = value; }
	}

	#endregion

	#region Private Methods

	// Use this for initialization
	void Start () 
	{
		timeFormat = "{0}";
		secondText = "00";
		second = Constants.TimeLimit;
		stopTimer = false;
	}

	// Update is called once per frame
	void Update () 
	{
		// Stop the coutdown time if true
		if (stopTimer)
			return;
		
		second -= Time.deltaTime;
		if (second <= 0.0f)
		{
			second = Constants.TimeLimit;
			stopTimer = true;

			if (isServer)
			{
				// Send notifcation to indicate whose turn is next
				OnTimesUp (!gameManager.isPlayerTurn);
			}
		}

		SetTimerText ();
	}

	/// <summary>
	/// Sets the timer text.
	/// </summary>
	public void SetTimerText()
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
			
		timeText.text = secondText;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Sets the start time.
	/// </summary>
	/// <param name="startSecond">Start second.</param>
	/// <param name="startMinute">Start minute.</param>
	/// <param name="startHour">Start hour.</param>
	public void SetStartTime(float startSecond)
	{
		second = startSecond;
	}

	#endregion

	#region Public Events

	/// <summary>
	/// Occurs when the time is up (15 seconds).
	/// </summary>
	//[SyncEvent]
	public event EventHandler TimesUp;

	public delegate void TimesUpDelegate(bool isPlayerTurn);

	[SyncEvent]
	public event TimesUpDelegate EventTimesUp;

	/// <summary>
	/// Raises the times up event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	public virtual void OnTimesUp(bool isPlayerTurn)
	{
		if (EventTimesUp != null)
		{
			EventTimesUp (isPlayerTurn);
		}
	}

	#endregion
}
