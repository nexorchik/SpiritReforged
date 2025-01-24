using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.Corruption;
using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaGrass : ModTile, IConvertibleTile
{
	protected virtual int DirtType => ModContent.TileType<SavannaDirt>();
	protected virtual Color MapColor => new(104, 156, 70);

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileNoFail[Type] = true;

		TileID.Sets.Grass[Type] = true;
		TileID.Sets.NeedsGrassFramingDirt[Type] = DirtType;
		TileID.Sets.CanBeDugByShovel[Type] = true;

		RegisterItemDrop(DirtType);
		AddMapEntry(MapColor);
		this.Merge(DirtType, ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaGrassCorrupt>(), 
			ModContent.TileType<SavannaGrassHallow>(), ModContent.TileType<SavannaGrassCrimson>());

		var data = TileObjectData.GetTileData(TileID.Sunflower, 0);
		data.AnchorValidTiles = data.AnchorValidTiles.Concat([Type]).ToArray(); //Allow sunflowers to be planted on this tile
	}

	public override bool CanExplode(int i, int j)
	{
		WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
		return true;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 4, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread grass

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j)
	{
		if (Main.rand.NextBool(30)) //Grow vines
			TileExtensions.GrowVine(i, j + 1, ModContent.TileType<SavannaVine>());

		var above = Framing.GetTileSafely(i, j - 1);

		if (!above.HasTile && above.LiquidAmount < 80)
		{
			int grassChance = GrassAny() ? 6 : 90;

			if (Main.rand.NextBool(grassChance) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<ElephantGrass>(), true, style: Main.rand.Next(5, 8)))
				NetMessage.SendTileSquare(-1, i, j - 2, 1, 2, TileChangeType.None);

			if (Main.rand.NextBool(250) && WorldGen.PlaceTile(i, j - 1, TileID.DyePlants, true, style: 2))
				NetMessage.SendTileSquare(-1, i, j - 1, TileChangeType.None);

			if (!WorldGen.PlayerLOS(i, j))
			{
				if (Main.rand.NextBool(70) && WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundSmall>(), true, style: Main.rand.Next(3)))
					NetMessage.SendTileSquare(-1, i, j - 1, 2, 1, TileChangeType.None);
				else if (Main.rand.NextBool(85) && WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundMedium>(), true, style: Main.rand.Next(2)))
					NetMessage.SendTileSquare(-1, i, j - 4, 3, 4, TileChangeType.None);
				else if (Main.rand.NextBool(100) && WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundLarge>(), true))
					NetMessage.SendTileSquare(-1, i, j - 5, 3, 5, TileChangeType.None);
			}
		}

		bool GrassAny()
		{
			int type = ModContent.TileType<ElephantGrass>();
			return Framing.GetTileSafely(i - 1, j - 1).TileType == type || Framing.GetTileSafely(i + 1, j - 1).TileType == type;
		}
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!fail) //Change self into dirt
		{
			fail = true;
			Framing.GetTileSafely(i, j).TileType = (ushort)DirtType;
		}
	}

	public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
	{
		Framing.GetTileSafely(i, j).TileType = (ushort)DirtType;
		return true;
	}

	public override void FloorVisuals(Player player)
	{
		if (player.flowerBoots) //Flower Boots functionality
		{
			var pos = ((player.Bottom - new Vector2(0, 8 * player.gravDir)) / 16).ToPoint16();

			if (!Main.tile[pos.X, pos.Y].HasTile)
			{
				WorldGen.PlaceTile(pos.X, pos.Y, ModContent.TileType<SavannaFoliage>(), true, style: Main.rand.Next(5));
				NetMessage.SendTileSquare(-1, pos.X, pos.Y);
			}
		}
	}

	public bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int oldId = tile.TileType;

		tile.TileType = (ushort)(type switch
		{
			ConversionType.Hallow => ModContent.TileType<SavannaGrassHallow>(),
			ConversionType.Crimson => ModContent.TileType<SavannaGrassCrimson>(),
			ConversionType.Corrupt => ModContent.TileType<SavannaGrassCorrupt>(),
			_ => ModContent.TileType<SavannaGrass>(),
		});

		if (oldId != tile.TileType)
		{
			TileCorruptor.Convert(new EntitySource_TileUpdate(i, j), type, i, j - 1);
			return true;
		}

		return false;
	}
}

public class SavannaGrassCorrupt : SavannaGrass
{
	protected override Color MapColor => new(109, 106, 174);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.Corrupt[Type] = true;
		TileID.Sets.AddCorruptionTile(Type, 20);
	}
}

public class SavannaGrassHallow : SavannaGrass
{
	protected override Color MapColor => new(78, 193, 227);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.Hallow[Type] = true;
		TileID.Sets.HallowBiome[Type] = 20;
	}
}

public class SavannaGrassCrimson : SavannaGrass
{
	protected override Color MapColor => new(183, 69, 68);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.AddCrimsonTile(Type, 20);
		TileID.Sets.Crimson[Type] = true;
	}
}