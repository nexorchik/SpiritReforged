using SpiritReforged.Common.SimpleEntity;
using SpiritReforged.Common.TileCommon.CustomTree;
using SpiritReforged.Common.TileCommon.TileSway;
using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaTree : CustomTree
{
	/// <summary> How much acacia tree tops sway in the wind. Used by the client for drawing and platform logic. </summary>
	public static float GetSway(int i, int j, double factor = 0)
	{
		if (factor == 0)
			factor = TileSwaySystem.Instance.TreeWindCounter;

		return Main.instance.TilesRenderer.GetWindCycle(i, j, factor) * .4f;
	}
	public static IEnumerable<AcaciaPlatform> Platforms => SimpleEntitySystem.entities.Where(x => x is AcaciaPlatform).Cast<AcaciaPlatform>();

	public override int TreeHeight => WorldGen.genRand.Next(8, 14);

	public override void PostSetStaticDefaults()
	{
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>()];
		RegisterItemDrop(ItemID.Wood);
		DustType = DustID.WoodFurniture;
	}

	public override void NearbyEffects(int i, int j, bool closer) //Spawn platforms
	{
		var pt = new Point16(i, j);
		if (treeDrawPoints.Contains(pt) && Framing.GetTileSafely(i, j - 1).TileType != Type 
			&& !Platforms.Where(x => x.TreePosition == pt).Any())
		{
			int type = SimpleEntitySystem.types[typeof(AcaciaPlatform)];
			//Spawn our entity at direct tile coordinates where it can reposition itself after updating
			SimpleEntitySystem.NewEntity(type, pt.ToVector2());
		}
	}

	public override void DrawTreeFoliage(int i, int j, SpriteBatch spriteBatch)
	{
		var position = new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(10, 0) + GetPalmTreeOffset(i, j);
		float rotation = GetSway(i, j) * .08f;

		if (Framing.GetTileSafely(i, j).TileType == Type && Framing.GetTileSafely(i, j - 1).TileType != Type) //Draw a treetop
		{
			var source = topsTexture.Frame();
			var origin = source.Bottom() - new Vector2(8, 0);

			spriteBatch.Draw(topsTexture.Value, position, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0);
		}
		else if (branchesTexture != null) //Draw branches
		{
			int frameX = ((Framing.GetTileSafely(i, j).TileFrameX / 18 + 1) % 2 == 0) ? 1 : 0;
			var source = branchesTexture.Frame(2, 1, frameX, sizeOffsetX: -2, sizeOffsetY: -2);
			var origin = new Vector2(frameX == 0 ? source.Width : 0, source.Height / 1.5f);

			position.X += 6 * (frameX == 0 ? -1 : 1);

			spriteBatch.Draw(branchesTexture.Value, position, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0);
		}
	}

	public override void AddDrawPoints(int i, int j, SpriteBatch spriteBatch)
	{
		if (Framing.GetTileSafely(i, j - 1).TileType != Type && TileObjectData.GetTileStyle(Framing.GetTileSafely(i, j)) < 2)
			treeDrawPoints.Add(new Point16(i, j));
		else if (TileObjectData.GetTileStyle(Framing.GetTileSafely(i, j)) == 1)
			treeDrawPoints.Add(new Point16(i, j));
	}

	protected override void GenerateTree(int i, int j, int height)
	{
		short GetPalmOffset(int variance, int height, ref short offset)
		{
			if (j != 0 && offset != variance)
			{
				double num5 = (double)j / (double)height;
				if (!(num5 < 0.25))
				{
					if ((!(num5 < 0.5) || !WorldGen.genRand.NextBool(13)) && (!(num5 < 0.7) || !WorldGen.genRand.NextBool(9)) && num5 < 0.95)
						WorldGen.genRand.Next(5);

					short num6 = (short)Math.Sign(variance);
					offset = (short)(offset + (short)(num6 * 2));
				}
			}

			return offset;
		}

		int variance = WorldGen.genRand.Next(-8, 9) * 2;
		short xOff = 0;

		for (int h = 0; h < height; h++)
		{
			int style = 0;

			if (h > height / 2 && h % 2 == 0 && WorldGen.genRand.NextBool(4)) //Randomly select rare segments above half height
				style = 1;

			WorldGen.PlaceTile(i, j - h, Type, true);
			Framing.GetTileSafely(i, j - h).TileFrameX = (short)(style * frameSize * 3 + WorldGen.genRand.Next(3) * frameSize);
			Framing.GetTileSafely(i, j - h).TileFrameY = GetPalmOffset(variance, height, ref xOff);
		}

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j + 1 - height, 1, height, TileChangeType.None);
	}
}
