using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.Corruption;
using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaGrass : ModTile, IConvertibleTile
{
	protected virtual int DirtType => ModContent.TileType<SavannaDirt>();
	protected virtual Color MapColor => new(104, 156, 70);
	protected virtual int Foliage => ModContent.TileType<SavannaFoliage>();

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileNoFail[Type] = true;

		this.Merge(DirtType, ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaGrassCorrupt>(), ModContent.TileType<SavannaGrassHallow>(), 
			ModContent.TileType<SavannaGrassCrimson>());

		TileID.Sets.Grass[Type] = true;
		TileID.Sets.NeedsGrassFramingDirt[Type] = DirtType;
		TileID.Sets.CanBeDugByShovel[Type] = true;

		AddMapEntry(MapColor);

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

		GrowFoliage(i, j);
	}

	protected virtual void GrowFoliage(int i, int j)
	{
		if (Main.rand.NextBool(30)) //Grow vines
			TileExtensions.GrowVine(i, j + 1, ModContent.TileType<SavannaVine>());

		var above = Framing.GetTileSafely(i, j - 1);

		if (Main.rand.NextBool(90) && !above.HasTile && above.LiquidAmount < 80) //The majority of elephant grass generation happens in that class
		{
			if (WorldGen.PlaceObject(i, j, ModContent.TileType<ElephantGrassShort>(), true, style: Main.rand.Next(3)))
				NetMessage.SendTileSquare(-1, i, j - 1, 1, 2, TileChangeType.None);
		}

		if (Main.rand.NextBool(120) && !above.HasTile && above.LiquidAmount < 80 && !WorldGen.PlayerLOS(i, j)) //Place small termite nests
		{
			if (WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundSmall>(), true, style: Main.rand.Next(3)))
				NetMessage.SendTileSquare(-1, i, j - 2, 1, 3, TileChangeType.None);
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

	public override void FloorVisuals(Player player)
	{
		if (player.flowerBoots) //Flower Boots functionality
		{
			var pos = ((player.Bottom - new Vector2(0, 8 * player.gravDir)) / 16).ToPoint16();

			if (!Main.tile[pos.X, pos.Y].HasTile)
			{
				WorldGen.PlaceTile(pos.X, pos.Y, Foliage, true, style: Main.rand.Next(5));
				NetMessage.SendTileSquare(-1, pos.X, pos.Y);
			}
		}
	}

	public bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		if (type == ConversionType.Purify)
			return false;

		Tile tile = Main.tile[i, j];
		int oldId = tile.TileType;

		tile.TileType = (ushort)(type switch
		{
			ConversionType.Hallow => ModContent.TileType<SavannaGrassHallow>(),
			ConversionType.Crimson => ModContent.TileType<SavannaGrassCrimson>(),
			ConversionType.Corrupt => ModContent.TileType<SavannaGrassCorrupt>(),
			_ => Type,
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
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.Corrupt[Type] = true;
		TileID.Sets.AddCorruptionTile(Type, 1);
	}

	protected override Color MapColor => new(109, 106, 174);
}

public class SavannaGrassHallow : SavannaGrass
{
	protected override Color MapColor => new(78, 193, 227);
}

public class SavannaGrassCrimson : SavannaGrass
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.AddCorruptionTile(Type, 1);
		TileID.Sets.Corrupt[Type] = true;
	}

	protected override Color MapColor => new(183, 69, 68);
}