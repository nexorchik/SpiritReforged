using SpiritReforged.Common.TileCommon;
using System.Linq;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaGrass : ModTile
{
	private static int DirtType => ModContent.TileType<SavannaDirt>();

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileNoFail[Type] = true;

		Main.tileMerge[DirtType][Type] = true;
		Main.tileMerge[Type][DirtType] = true;

		TileID.Sets.Grass[Type] = true;
		TileID.Sets.NeedsGrassFramingDirt[Type] = DirtType;
		TileID.Sets.CanBeDugByShovel[Type] = true;

		RegisterItemDrop(DirtType);
		AddMapEntry(new Color(104, 156, 70));

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
}
