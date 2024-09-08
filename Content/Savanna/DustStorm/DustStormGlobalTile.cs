using SpiritReforged.Content.Savanna.Tiles;
using System.Linq;

namespace SpiritReforged.Content.Savanna.DustStorm;

public class DustStormGlobalTile : GlobalTile
{
	public override void NearbyEffects(int i, int j, int type, bool closer)
	{
		if (!Main.LocalPlayer.GetModPlayer<DustStormPlayer>().ZoneDustStorm)
			return;

		var tileAbove = Main.tile[i, j - 1];

		int[] types = [ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaDirt>(), TileID.Sand];
		if (types.Contains(type) && !WorldGen.SolidTile(tileAbove)) //Spawn our dusts
		{
			float wind = Main.rand.NextFloat(Main.windSpeedCurrent);

			if (Main.rand.NextBool(50))
			{
				var dust = Dust.NewDustDirect(new Vector2(i, j - 1) * 16, 32, 32, ModContent.DustType<SavannaCloud>(), 0, 0, 0, Color.SandyBrown * Main.rand.NextFloat(1.2f, 1.6f), Main.rand.NextFloat(.5f, 2f));
				dust.velocity = new Vector2(wind * 5, Math.Abs(wind) * -.25f);
			}

			if (Main.rand.NextBool(10))
			{
				var dust = Dust.NewDustPerfect(new Vector2(i, j) * 16, ModContent.DustType<SavannaSand>(), new Vector2(wind * 10, -Math.Abs(wind)), 150, default);
				dust.noGravity = true;

				bool twirly = Main.rand.NextBool(30);
				dust.customData = twirly ? .02f : Main.rand.NextFloat(.005f, .011f);
				dust.scale = twirly ? Main.rand.NextFloat(1f, 1.5f) : Main.rand.NextFloat(.5f, 1.1f);
			}
		}
	}
}
