using SpiritReforged.Common.ItemCommon;
using Terraria;
using Terraria.ID;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpiritReforged.Common.PlayerCommon;

/// <summary> automatically spawns all minion accessory type projectiles in one place!
public class MinionAccessoryPlayer : ModPlayer
{
	public Dictionary<int, MinionAccessoryData> MinionDataByItemId = new();

	public override void ResetEffects() => MinionDataByItemId.Clear();

	// spawn all projectile types in the dictionary with their respective damages
	public override void PostUpdateEquips()
	{
		foreach(var projData in MinionDataByItemId)
		{
			int itemType = projData.Key;
			int projType = projData.Value.ProjType;
			int projDamage = projData.Value.Damage;

			//debug
			if (!Player.HasAccessory(itemType))
			{
				Main.NewText($"ItemType {itemType} not found in equipped accessories.");
			}

			if (Player.HasAccessory(itemType) && Player.ownedProjectileCounts[projType] < 1)
				Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, projType, (int)Player.GetDamage(DamageClass.Summon).ApplyTo(projDamage), 0f, Player.whoAmI);
		}
	}
}
