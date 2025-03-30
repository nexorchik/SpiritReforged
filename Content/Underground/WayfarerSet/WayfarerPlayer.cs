using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Content.Ocean.Items.Pearl;
using SpiritReforged.Content.Particles;
using Terraria;
using Terraria.Audio;
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
			{
				if (!player.HasBuff(ModContent.BuffType<ExplorerPot>()))
					DoFX();

				player.AddBuff(ModContent.BuffType<ExplorerPot>(), 600);
			}

			if (player.GetModPlayer<WayfarerPlayer>().active && Main.tileSpelunker[type] && Main.tileSolid[type])
			{
				if (!player.HasBuff(ModContent.BuffType<ExplorerMine>()))
					DoFX();

				player.AddBuff(ModContent.BuffType<ExplorerMine>(), 600);
			}
		}
	}
	public void DoFX()
	{
		Player player = Main.LocalPlayer;
		SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal with { Pitch = 2f }, player.Center);
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Ambient/PositiveOutcome") with { Pitch = -.35f }, player.Center);

		for (int i = 0; i < 12; i++)
			ParticleHandler.SpawnParticle(new GlowParticle(player.Center, Main.rand.NextVector2CircularEdge(1, 1), Color.PapayaWhip, Main.rand.NextFloat(0.25f, 0.4f), Main.rand.Next(30, 50), 8));

	}
}

