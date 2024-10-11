using SpiritReforged.Common.Particle;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Ocean.Items.Reefhunter;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Hydrothermal.Tiles;

public class HydrothermalVent : ModTile
{
	private static readonly Point[] tops = [new Point(16, 16), new Point(16, 16), new Point(16, 24), new Point(12, 4), new Point(20, 4), new Point(16, 16), new Point(16, 16), new Point(16, 16)];
	private SoundStyle sound = new("SpiritReforged/Assets/SFX/Ambient/Bubbling") { SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileSpelunker[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
		TileObjectData.newTile.Origin = new(1, 3);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<Gravel>(), TileID.Sand];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 8;
		TileObjectData.addTile(Type);

		DustType = DustID.Stone;
		AddMapEntry(new Color(64, 54, 66), CreateMapEntryName());
	}

	public override bool RightClick(int i, int j)
	{
		//var player = Main.LocalPlayer;
		//if (player.ZoneBeach && player.GetModPlayer<OceanPlayer>().Submerged(45))
			HitWire(i, j);

		return true;
	}

	public override void MouseOver(int i, int j)
	{
		var player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<SulfurDeposit>();
	}

	public override void HitWire(int i, int j)
	{
		TileExtensions.GetTopLeft(ref i, ref j);
		//if (Wiring.CheckMech(i, j, 7200))
		//{
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16), ModContent.DustType<Dusts.BoneDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16), ModContent.DustType<Dusts.FireClubDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));

		var t = Framing.GetTileSafely(i, j);
		int fullWidth = TileObjectData.GetTileData(t).CoordinateFullWidth;
		var position = new Vector2(i, j) * 16 + tops[t.TileFrameX / fullWidth].ToVector2();
		Projectile.NewProjectileDirect(new EntitySource_Wiring(i, j), position, Vector2.UnitY * -4f, ModContent.ProjectileType<HydrothermalVentPlume>(), 5, 0f);
		//}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		var t = Framing.GetTileSafely(i, j);
		int fullWidth = TileObjectData.GetTileData(t).CoordinateFullWidth;

		if (t.TileFrameX % fullWidth == 0 && t.TileFrameY == 0) //Visuals
		{
			if (Main.gamePaused)
				return;

			var position = new Vector2(i, j) * 16 + tops[t.TileFrameX / fullWidth].ToVector2();
			if (Main.rand.NextBool(5))
			{
				var velocity = new Vector2(0, -Main.rand.NextFloat(2f, 2.5f));

				ParticleHandler.SpawnParticle(new DissipatingSmoke(position, velocity,
					new Color(40, 40, 50), Color.SlateGray, Main.rand.NextFloat(.005f, .06f), 150)
				{ squash = true });
			}

			if (Main.rand.NextBool())
			{
				float range = Main.rand.NextFloat();
				var velocity = new Vector2(0, -Main.rand.NextFloat(range * 8f)).RotatedByRandom((1f - range) * 1.5f);

				var dust = Dust.NewDustPerfect(position, DustID.Ash, velocity, Alpha: 180);
				dust.noGravity = true;
			}

			SoundEngine.PlaySound(sound, new Vector2(i, j) * 16);

			//Move the sound to the closest vent
			var activeSound = SoundEngine.FindActiveSound(in sound);
			activeSound.Position = (activeSound.Position.HasValue && Main.LocalPlayer.Distance(activeSound.Position.Value) > Main.LocalPlayer.Distance(new Vector2(i, j) * 16)) 
				? new Vector2(i, j) * 16 : activeSound.Position;
		}
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}
