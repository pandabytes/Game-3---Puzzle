using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "GameState")]
public class GameState : ScriptableObject
{
	/// <summary>
	/// The stage.
	/// </summary>
	public StageEnum stage;
}

