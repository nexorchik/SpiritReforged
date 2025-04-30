using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Common.PlayerCommon;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Snow;

[AutoloadEquip(EquipType.Waist)]
public class FrostGiantBelt : AccessoryItem
{
	/// <summary> Checks if <paramref name="player"/> is using a club projectile. </summary>
	public static bool ClubActive(Player player) => player.HeldItem?.ModItem is ClubItem && player.ItemAnimationActive;
	public override void SetStaticDefaults() => NPCLootDatabase.AddLoot(new(NPCLootDatabase.MatchId(NPCID.UndeadViking), ItemDropRule.Common(Type, 15)));

	public override void SetDefaults()
	{
		Item.Size = new Vector2(30);
		Item.value = Item.sellPrice(0, 0, 90, 0);
		Item.rare = ItemRarityID.Orange;
		Item.accessory = true;
	}
}

internal class FrostGiantPlayer : ModPlayer
{
	public float extraDefense;

	public override void UpdateEquips()
	{
		if (Player.HasAccessory<FrostGiantBelt>() && FrostGiantBelt.ClubActive(Player))
		{
			extraDefense = Math.Min(extraDefense + (float)(1f / 6f), 15);
			Player.statDefense += (int)extraDefense;
		}
		else
		{
			extraDefense = 0;
		}
	}

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		if (Player.HasAccessory<FrostGiantBelt>() && FrostGiantBelt.ClubActive(Player))
			modifiers.Knockback *= 0.5f;
	}
}