using SpiritReforged.Content.Savanna.Tiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Items.BaobabFruit;

public class BaobabFruitTile : ModTile
{
	/// <summary> Grows a baobab vine from the given coordinates including one-time framing logic. </summary>
	internal static void GrowVine(int i, int j, int length = 1)
	{
		const int maxVineLength = 5;

		int type = ModContent.TileType<BaobabFruitTile>();

		for (int l = 0; l < length; l++)
		{
			for (int x = 0; x < maxVineLength; x++)
			{
				if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == type)
					j++; //Move to the next non-baobab tile below
			}

			WorldGen.PlaceObject(i, j, ModContent.TileType<BaobabFruitTile>(), true, Main.rand.Next(2));

			if (Main.tile[i, j].TileType != type)
				break; //Tile placement failed

			var above = Main.tile[i, j - 1];
			if (above.TileType == type)
			{
				above.TileFrameY = 18;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(-1, i, j - 1, 1, 2);
			}
			else if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, i, j, 1, 1);

			j++; //Only applicable if length is greater than 1
		}
	}

	public override void SetStaticDefaults()
	{
		Main.tileBlockLight[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.VineThreads[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<LivingBaobabLeaf>()];
		TileObjectData.newTile.AnchorAlternateTiles = [Type];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 2;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(140, 140, 100));
		DustType = DustID.t_PearlWood;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		var above = Main.tile[i, j - 1];
		var below = Main.tile[i, j + 1];
		var tile = Main.tile[i, j];

		if (above.TileType != ModContent.TileType<LivingBaobabLeaf>() && above.TileType != Type)
		{
			WorldGen.KillTile(i, j);
			return false;
		}

		if (resetFrame)
		{
			if (below.HasTile && below.TileType == Type)
				tile.TileFrameY = 18;
			else
				tile.TileFrameY = 0;
		}

		return false; //True results in the tile being invisible in most cases
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		var tile = Main.tile[i, j];

		if (tile.TileFrameY == 0) //This is a fruit frame
		{
			var position = new Vector2(i, j) * 16 + new Vector2(8);
			Projectile.NewProjectile(new EntitySource_TileBreak(i, j), position, Vector2.Zero, ModContent.ProjectileType<BaobabFruitProj>(), 10, 0f, ai0: tile.TileFrameX / 18);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.LightingEveryFrame)
			Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);

		return false;
	}
}
