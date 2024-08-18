using Terraria.Audio;

namespace SpiritReforged.Content.Bamboo.Buffs;

public class Impaled : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = false;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		Tile tile = Framing.GetTileSafely(player.Bottom);

		if (tile.HasTile && tile.TileType == ModContent.TileType<Tiles.BambooPike>())
		{
			if (player.buffTime[buffIndex] % 30 == 25)
				SoundEngine.PlaySound(SoundID.NPCHit1 with { PitchVariance = .2f, Pitch = -.5f }, player.Center);

			player.lifeRegen = -15;
			player.velocity = new Vector2(0, .25f);
			player.gravity = 0f;
		}
		else
		{
			player.lifeRegen = 0;
			player.velocity = new Vector2(player.velocity.X * .8f, (player.velocity.Y > 0) ? player.velocity.Y : player.velocity.Y * .8f);
		}

		if (Main.rand.NextBool(7))
		{
			Vector2 position = player.position + new Vector2(player.width * Main.rand.NextFloat(), player.height * Main.rand.NextFloat());
			Dust.NewDustPerfect(position, DustID.Blood, Vector2.Zero, 0, default, Main.rand.NextFloat(0.5f, 1.2f));
		}
	}
}