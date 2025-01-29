namespace SpiritReforged.Common.PlayerCommon;

/// <summary>
/// Controls legacy pet bools and gives the PetFlag tool.<br/>
/// DO NOT make a new bool - simply use <see cref="PetFlag(Projectile)"/> in the pet's AI instead! Reduces field spam and is more readable.<br/>
/// Simply put this line in the pet's AI: <code>Owner.GetModPlayer&lt;GlobalClasses.Players.PetPlayer>().PetFlag(Projectile);</code>
/// </summary>
public class PetPlayer : ModPlayer
{
	public Dictionary<int, bool> pets = [];

	public override void ResetEffects()
	{
		foreach (int item in pets.Keys)
			pets[item] = false;
	}

	/// <summary>Automatically sets pet flag for any given projectile using <see cref="pets"/>.</summary>
	public void PetFlag(Projectile projectile)
	{
		var modPlayer = Main.player[projectile.owner].GetModPlayer<PetPlayer>();

		modPlayer.pets.TryAdd(projectile.type, true);

		if (Player.dead)
			modPlayer.pets[projectile.type] = false;

		if (modPlayer.pets[projectile.type])
			projectile.timeLeft = 2;
	}
}
