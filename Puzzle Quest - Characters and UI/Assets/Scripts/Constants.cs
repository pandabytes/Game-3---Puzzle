using System;

public enum StageEnum
{
	FirstStage = 0,
	SecondStage = 1,
	ThirdStage = 2,
	FourthStage = 3,
	FiftheStage = 4
};

public static class Constants
{
	#region Attack Damage

	/// <summary>
	/// The physical damage.
	/// </summary>
	public const float PhysicalDamage = 50.0f;

	/// <summary>
	/// The fire damage.
	/// </summary>
	public const float FireDamage = 17.6f;

	#endregion

	/// <summary>
	/// THe amount of HP recovered from healing
	/// </summary>
	public const float HealAmount = 5.0f;

	#region Messages

	public const string FireBallMessage = "You gain Fire Ball spell !!!";
	public const string EarthSpikeMessage = "You gain Earth Spike spell !!!";
	public const string IceShardMessage = "You gain Ice Shard spell !!!";
	public const string LightingZapMessage = "You gain Lighting Zap spell !!!";

	#endregion
}