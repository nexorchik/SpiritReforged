namespace SpiritReforged.Content.Common.PlayerCommon;

/// <summary> automatically spawns all minion accessory type projectiles in one place!
public class MinionAccessoryPlayer : ModPlayer
{
	//A dictionary for each minion projectile (key) and its corresponding damage (val)
	public Dictionary<int, int> MinionProjectileData = new();

	public override void ResetEffects() => MinionProjectileData.Clear();

	// spawn all projectile types in the dictionary with their respective damages
	public override void PostUpdateEquips()
	{
		foreach(var projData in MinionProjectileData)
		{
			int projType = projData.Key;
			int projDamage = projData.Value;

			if (Player.ownedProjectileCounts[projType] < 1)
				Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, projType, (int)Player.GetDamage(DamageClass.Summon).ApplyTo(projDamage), 0f, Player.whoAmI);

		}
	}
}
