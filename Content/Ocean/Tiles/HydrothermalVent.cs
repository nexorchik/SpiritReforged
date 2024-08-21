using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public abstract class HydrothermalVent : ModTile
{
	public sealed override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileSpelunker[Type] = true;
		Main.tileLighted[Type] = true;

		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = DustID.Stone;

		AddMapEntry(new Color(64, 54, 66), Language.GetText("Mods.SpiritReforged.Tiles.VentMapEntry"));

		StaticDefaults();
	}

	public virtual void StaticDefaults() { }

	public override bool RightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;

		if (player.ZoneBeach && player.GetModPlayer<OceanPlayer>().Submerged(45))
			HitWire(i, j);

		return true;
	}

	public override void HitWire(int i, int j)
	{
		j -= Main.tile[i, j].TileFrameY / 18; //Interact with only the topmost tile in the multitile, since each tile stores its own cooldown

		if (Wiring.CheckMech(i, j, 7200))
		{
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(new Vector2(i * 16 + 12, j * 16), ModContent.DustType<Dusts.BoneDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(new Vector2(i * 16 + 12, j * 16), ModContent.DustType<Dusts.FireClubDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));

			Projectile.NewProjectile(new EntitySource_Wiring(i, j), i * 16 + 12, j * 16, 0, -4, ModContent.ProjectileType<Projectiles.HydrothermalVentPlume>(), 5, 0f);
		}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		var config = ModContent.GetInstance<Common.ConfigurationCommon.ReforgedServerConfig>();
		Tile t = Framing.GetTileSafely(i, j);

		if (t.TileFrameY != 0)
			return;

		#region visual effects
		var pos = new Vector2(i, j - 1) * 16;

		if (Main.rand.NextBool(16))
			Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), pos - new Vector2(12, 0), new Vector2(0, Main.rand.NextFloat(-2.2f, -1.5f)), 99, Main.rand.NextFloat(0.5f, 0.8f));
		if (Main.rand.NextBool(2))
			Dust.NewDustPerfect(pos + new Vector2(8, 20) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f), DustID.Ash, (Vector2.UnitY * -Main.rand.NextFloat(3f)).RotatedByRandom(.5f), 40, default, Main.rand.NextFloat(2f)).noGravity = true;

		if (Wiring.CheckMech(i, j, 0) && Main.rand.NextBool(4))
		{
			var dust = Dust.NewDustPerfect(pos + new Vector2(8, 20) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f), DustID.Torch, (Vector2.UnitY * -Main.rand.NextFloat(3f)).RotatedByRandom(.5f), 40, default, Main.rand.NextFloat(2f));
			dust.noGravity = true;
			dust.noLightEmittence = true;
		}
		#endregion

		if (config.VentCritters && t.LiquidAmount > 155)
		{
			SpawnCritter<NPCs.TinyCrab>(i, j, 1100);
			SpawnCritter<NPCs.Crinoid>(i, j, 1350);
			SpawnCritter<NPCs.TubeWorm>(i, j, 1200);
		}
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0 && Wiring.CheckMech(i, j, 0))
			(r, g, b) = (0.45f, 0.2f, 0);
	}

	internal static void SpawnCritter<T>(int i, int j, int denominator) where T : ModNPC
	{
		static Point GetRelativeSpawn(int i, int j, int tileRange = 5)
		{
			i += Main.rand.Next(-tileRange, tileRange);
			for (int a = 0; a < 10; a++)
			{
				var on = Framing.GetTileSafely(i, j);
				var below = Framing.GetTileSafely(i, j + 1);

				if (on.HasTile && Main.tileSolid[on.TileType])
					j--;
				else if (!below.HasTile || !Main.tileSolid[below.TileType])
					j++;
				else
					break;
			}

			return (new Point(i, j).ToVector2() * 16).ToPoint();
		}

		int npcIndex = -1;
		int type = ModContent.NPCType<T>();

		var pos = GetRelativeSpawn(i, j);
		if (Main.rand.NextBool(denominator) && NPC.MechSpawn(pos.X, pos.Y, type) && NPC.CountNPCS(type) < 10)
			npcIndex = NPC.NewNPC(new EntitySource_TileUpdate(i, j), pos.X, pos.Y, type);

		if (npcIndex >= 0)
		{
			Main.npc[npcIndex].value = 0f;
			Main.npc[npcIndex].npcSlots = 0f;
		}
	}
}
