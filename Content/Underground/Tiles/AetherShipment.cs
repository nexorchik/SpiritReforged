using RubbleAutoloader;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class AetherShipment : PotTile, ILootTile
{
	public override Dictionary<string, int[]> TileStyles => new() { { string.Empty, [0, 1, 2] } };

	public override void AddRecord(int type, StyleDatabase.StyleGroup group) => RecordHandler.Records.Add(new TileRecord(group.name, type, group.styles).AddRating(5));
	public override void AddObjectData()
	{
		Main.tileCut[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new(0, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.StyleWrapLimit = 3;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		DustType = DustID.ShimmerSpark;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (effectOnly || Autoloader.IsRubble(Type))
			return;

		fail = AdjustFrame(i, j);
	}

	public override bool KillSound(int i, int j, bool fail)
	{
		if (fail || Autoloader.IsRubble(Type))
			return true;

		var pos = new Vector2(i, j).ToWorldCoordinates(16, 16);

		SoundEngine.PlaySound(SoundID.Shatter, pos);
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Tile/PotBreak") with { Volume = .16f, PitchRange = (-.4f, 0), }, pos);

		return true;
	}

	private static bool AdjustFrame(int i, int j)
	{
		const int fullWidth = 36;

		TileExtensions.GetTopLeft(ref i, ref j);

		if (Main.tile[i, j].TileFrameX > fullWidth)
			return false; //Frame has already been adjusted

		for (int x = i; x < i + 2; x++)
		{
			for (int y = j; y < j + 2; y++)
			{
				var t = Main.tile[x, y];
				t.TileFrameX += fullWidth;
			}
		}

		return true;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var position = new Vector2(i, j).ToWorldCoordinates(12, 12);

			var p = Main.player[Player.FindClosest(position, 0, 0)];
			AddLoot(TileObjectData.GetTileStyle(Main.tile[i, j])).Resolve(new Rectangle((int)position.X - 16, (int)position.Y - 16, 32, 32), p);
		}

		if (Main.dedServ)
			return;

		var source = new EntitySource_TileBreak(i, j);

		for (int g = 1; g < 6; g++)
		{
			int goreType = Mod.Find<ModGore>("Aether" + g).Type;
			Gore.NewGore(source, Main.rand.NextVector2FromRectangle(new Rectangle(i * 16, j * 16, 32, 32)), Vector2.Zero, goreType);
		}
	}

	public LootTable AddLoot(int objectStyle)
	{
		var loot = new LootTable();
		loot.AddOneFromOptions(1, ItemID.AegisCrystal, ItemID.ArcaneCrystal, ItemID.AegisFruit, ItemID.Ambrosia, ItemID.GummyWorm, ItemID.GalaxyPearl);

		return loot;
	}
}