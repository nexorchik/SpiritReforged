using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Ocean.Items.Reefhunter;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Ocean.Tiles;

public class HydrothermalVent : ModTile
{
	public override void Load() => On_WorldGen.PlaceObject += OnPlaceObject; //Automatically register points during worldgen

	private bool OnPlaceObject(On_WorldGen.orig_PlaceObject orig, int x, int y, int type, bool mute, int style, int alternate, int random, int direction)
	{
		bool placed = orig(x, y, type, mute, style, alternate, random, direction);
		if (placed && type == Type)
		{
			TileExtensions.GetTopLeft(ref x, ref y);
			VentSystem.ventPoints.Add(new Point16(x, y));
		}

		return placed;
	}

	public sealed override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileSpelunker[Type] = true;
		Main.tileLighted[Type] = true;
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
		var player = Main.LocalPlayer;
		if (player.ZoneBeach && player.GetModPlayer<OceanPlayer>().Submerged(45))
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
		if (Wiring.CheckMech(i, j, 7200))
		{
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16), ModContent.DustType<Dusts.BoneDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16), ModContent.DustType<Dusts.FireClubDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));

			Projectile.NewProjectile(new EntitySource_Wiring(i, j), i * 16 + 12, j * 16, 0, -4, ModContent.ProjectileType<Projectiles.HydrothermalVentPlume>(), 5, 0f);
		}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		var t = Framing.GetTileSafely(i, j);
		if (t.TileFrameX % (18 * 2) != 18 || t.TileFrameY != 0)
			return;

		#region visual effects
		var pos = new Vector2(i, j) * 16;

		if (Main.rand.NextBool(16))
			Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), pos - new Vector2(20, 0), new Vector2(0, Main.rand.NextFloat(-2.2f, -1.5f)), 99, Main.rand.NextFloat(0.5f, 0.8f));
		if (Main.rand.NextBool(2))
			Dust.NewDustPerfect(pos + new Vector2(0, 20) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f), DustID.Ash, (Vector2.UnitY * -Main.rand.NextFloat(3f)).RotatedByRandom(.5f), 40, default, Main.rand.NextFloat(2f)).noGravity = true;

		if (Wiring.CheckMech(i, j, 0) && Main.rand.NextBool(4))
		{
			var dust = Dust.NewDustPerfect(pos + new Vector2(0, 20) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f), DustID.Torch, (Vector2.UnitY * -Main.rand.NextFloat(3f)).RotatedByRandom(.5f), 40, default, Main.rand.NextFloat(2f));
			dust.noGravity = true;
			dust.noLightEmittence = true;
		}
		#endregion
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0 && Wiring.CheckMech(i, j, 0))
			(r, g, b) = (0.45f, 0.2f, 0);
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		TileExtensions.GetTopLeft(ref i, ref j);
		VentSystem.ventPoints.Add(new Point16(i, j));

		if (Main.netMode != NetmodeID.SinglePlayer)
		{
			ModPacket packet = SpiritReforgedMod.Instance.GetPacket(Common.Misc.ReforgedMultiplayer.MessageType.SendVentPoint, 2);
			packet.Write(i);
			packet.Write(j);
			packet.Send(); //Send to server
		}
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY) => VentSystem.ventPoints.Remove(new Point16(i, j));
}

public class VentSystem : ModSystem
{
	internal static HashSet<Point16> ventPoints = [];

	/// <summary> Returns all points inside a horizontal boundary around player, used for NPC spawning. </summary>
	public static List<Point16> GetValidPoints(Player player)
	{
		int spawnRange = (int)(NPC.sWidth / 16 * 0.7);
		int safeRange = (int)(NPC.sWidth / 16 * 0.52);
		int posX = (int)(player.Center.X / 16);

		return ventPoints.Where(x => Math.Abs(posX - x.X) < spawnRange && Math.Abs(posX - x.X) > safeRange).ToList();
	}

	public override void SaveWorldData(TagCompound tag) => tag[nameof(ventPoints)] = ventPoints.ToList();
	public override void LoadWorldData(TagCompound tag) => ventPoints = new HashSet<Point16>(tag.GetList<Point16>(nameof(ventPoints)));

	public override void NetSend(BinaryWriter writer)
	{
		static void WritePoint16(BinaryWriter writer, Point16 point)
		{
			writer.Write(point.X);
			writer.Write(point.Y);
		}

		var points = ventPoints.ToList();
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
			ventPoints.Add(point);
		}
	}
}
