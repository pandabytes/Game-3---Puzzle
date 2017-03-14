using UnityEngine;
using System;
using System.Collections;

public class ScoreEventArgs : EventArgs
{
	/// <summary>
	/// Amount of score to send.
	/// </summary>
	private float score;

	/// <summary>
	/// Gets the score.
	/// </summary>
	/// <value>The score.</value>
	public float Score
	{
		get { return score; }
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="p">P.</param>
	public ScoreEventArgs(float p)
	{
		score = p;
	}
}

