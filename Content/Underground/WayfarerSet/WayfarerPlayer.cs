using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Content.Ocean.Items.Pearl;
using Terraria;
using Terraria.ModLoader;

namespace SpiritReforged.Content.Underground.WayfarerSet;

internal class WayfarerPlayer : ModPlayer
{
	public bool active = false;
	public int miningStacks = 1;
	public int movementStacks = 1;

	public override void ResetEffects()
	{
		active = false;

		if (Player.FindBuffIndex(ModContent.BuffType<ExplorerMine>()) < 0)
			miningStacks = 1;

		if (Player.FindBuffIndex(ModContent.BuffType<ExplorerPot>()) < 0)
			movementStacks = 1;
	}

	public override void PostUpdateEquips()
	{
		Item bodyVanitySlot = Player.armor[11];
		Item bodyArmorSlot = Player.armor[1];
		if (bodyVanitySlot.type == ModContent.ItemType<WayfarerBody>() || bodyArmorSlot.type == ModContent.ItemType<WayfarerBody>() && bodyVanitySlot.IsAir)
			Player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, nameof(WayfarerBody), EquipType.Back);
	}
}

internal class WayfarerNPC : GlobalNPC
{
	public override bool PreKill(NPC npc)
	{
		if (Main.player[npc.lastInteraction].GetModPlayer<WayfarerPlayer>().active)
			npc.value *= 1.1f; //+10% coin drops (same as pearl string)

		return true;
	}
}

internal class WayfarerTile : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!Main.dedServ)
		{
			// TODO: Add all pot variants here
			Player player = Main.LocalPlayer;
			if (player.GetModPlayer<WayfarerPlayer>().active && type == TileID.Pots)
				player.AddBuff(ModContent.BuffType<ExplorerPot>(), 600);

			if (player.GetModPlayer<WayfarerPlayer>().active && Main.tileSpelunker[type] && Main.tileSolid[type])
				player.AddBuff(ModContent.BuffType<ExplorerMine>(), 600);
		}
	}
}

