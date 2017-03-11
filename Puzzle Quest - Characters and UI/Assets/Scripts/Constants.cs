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
	public const float PhysicalDamage = 1.0f;

	/// <summary>
	/// The enemy damage.
	/// </summary>
	public const float EnemyDamage = 22.0f;

	/// <summary>
	/// The boss damage.
	/// </summary>
	public const float BossDamage = 30.0f;

	/// <summary>
	/// The extra damage for fire ball.
	/// </summary>
	public const float ExtraDamage = 10.0f;

	/// <summary>
	/// The fire ball damage.
	/// </summary>
	public const float FireBallDamage = 1.2f;

	/// <summary>
	/// The earth spike damage.
	/// </summary>
	public const float EarthSpikeDamage = 1.7f;

	/// <summary>
	/// The ice shard damage.
	/// </summary>
	public const float IceShardDamage = 2.0f;

	#endregion

	#region Attack Names

	/// <summary>
	/// The physical attack.
	/// </summary>
	public const string PhysicalAttack = "PhysicalAttack";

	/// <summary>
	/// The fire ball.
	/// </summary>
	public const string FireBall = "FireBall";

	/// <summary>
	/// The earth spike.
	/// </summary>
	public const string EarthSpike = "EarthSpike";

	/// <summary>
	/// The ice shard.
	/// </summary>
	public const string IceShard = "IceShard";

	#endregion

	#region Spell Effects Description

	/// <summary>
	/// The fire ball effect description.
	/// </summary>
	public const string FireBallEffect = "You deal 10 extra damage !";

	/// <summary>
	/// The earth spike effect description.
	/// </summary>
	public const string EarthSpikeEffect = "Enemy damage is reduced by 1/2 !";

	/// <summary>
	/// The ice shard effect description.
	/// </summary>
	public const string IceShardEffect = "Enemy is frozen for 1 turn !";

	#endregion

	#region Messages

	/// <summary>
	/// The fire ball message.
	/// </summary>
	public const string FireBallMessage = "You gain Fire Ball spell !!!";

	/// <summary>
	/// The earth spike message.
	/// </summary>
	public const string EarthSpikeMessage = "You gain Earth Spike spell !!!";

	/// <summary>
	/// The ice shard message.
	/// </summary>
	public const string IceShardMessage = "You gain Ice Shard spell !!!";

	/// <summary>
	/// The final message.
	/// </summary>
	public const string FinalMessage = "You made it through !";

	#endregion

	/// <summary>
	/// THe amount of HP recovered from healing
	/// </summary>
	public const float HealAmount = 1.0f;

	/// <summary>
	/// The time limit.
	/// </summary>
	public const float TimeLimit = 15.0f;
}