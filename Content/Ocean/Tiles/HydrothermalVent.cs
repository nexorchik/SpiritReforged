using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Ocean.Tiles;

public abstract class HydrothermalVent : ModTile
{
	public override void Load() => On_WorldGen.PlaceObject += OnPlaceObject; //Automatically register points during worldgen

	private bool OnPlaceObject(On_WorldGen.orig_PlaceObject orig, int x, int y, int type, bool mute, int style, int alternate, int random, int direction)
	{
		bool placed = orig(x, y, type, mute, style, alternate, random, direction);

		if (placed && (type == ModContent.TileType<HydrothermalVent1x2>() || type == ModContent.TileType<HydrothermalVent1x3>()))
		{
			var tile = Main.tile[x, y];
			y -= tile.TileFrameY / 18; //Select the topmost tile

			VentSystem.VentPoints.Add(new Point16(x, y));
		}

		return placed;
	}

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
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0 && Wiring.CheckMech(i, j, 0))
			(r, g, b) = (0.45f, 0.2f, 0);
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		var tile = Main.tile[i, j];
		j -= tile.TileFrameY / 18; //Select the topmost tile

		VentSystem.VentPoints.Add(new Point16(i, j));

		if (Main.netMode != NetmodeID.SinglePlayer)
		{
			ModPacket packet = SpiritReforgedMod.Instance.GetPacket(Common.Misc.ReforgedMultiplayer.MessageType.SendVentPoint, 2);
			packet.Write(i);
			packet.Write(j);
			packet.Send(); //Send to server
		}
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY) => VentSystem.VentPoints.Remove(new Point16(i, j));
}

public class VentSystem : ModSystem
{
	internal static HashSet<Point16> VentPoints = new();

	/// <summary> Returns all points inside a horizontal boundary around player, used for NPC spawning. </summary>
	public static List<Point16> GetValidPoints(Player player)
	{
		int spawnRange = (int)((double)(NPC.sWidth / 16) * 0.7);
		int safeRange = (int)((double)(NPC.sWidth / 16) * 0.52);
		int posX = (int)(player.Center.X / 16);

		return VentPoints.Where(x => Math.Abs(posX - x.X) < spawnRange && Math.Abs(posX - x.X) > safeRange).ToList();
	}

	public override void SaveWorldData(TagCompound tag) => tag[nameof(VentPoints)] = VentPoints.ToList();

	public override void LoadWorldData(TagCompound tag) => VentPoints = new HashSet<Point16>(tag.GetList<Point16>(nameof(VentPoints)));

	public override void NetSend(BinaryWriter writer)
	{
		static void WritePoint16(BinaryWriter writer, Point16 point)
		{
			writer.Write(point.X);
			writer.Write(point.Y);
		}

		var points = VentPoints.ToList();
		int count = points.Count;

		writer.Write(count);

		for (int i = 0; i < count; i++)
			WritePoint16(writer, points[i]);
	}

	public override void NetReceive(BinaryReader reader)
	{
		int count = reader.ReadInt32();

		for (int i = 0; i < count; i++)
		{
			var point = new Point16(reader.ReadInt16(), reader.ReadInt16());
			VentPoints.Add(point);
		}
	}
}
