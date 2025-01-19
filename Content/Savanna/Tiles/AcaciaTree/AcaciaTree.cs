using SpiritReforged.Common.SimpleEntity;
using SpiritReforged.Common.TileCommon.Corruption;
using SpiritReforged.Common.TileCommon.CustomTree;
using SpiritReforged.Common.TileCommon.TileSway;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaTree : CustomTree, IConvertibleTile
{
	public static IEnumerable<TreetopPlatform> Platforms => SimpleEntitySystem.entities.Where(x => x is TreetopPlatform).Cast<TreetopPlatform>();

	public override int TreeHeight => WorldGen.genRand.Next(8, 16);
	protected virtual int ValidAnchor => ModContent.TileType<SavannaGrass>();

	/// <summary> How much acacia tree tops sway in the wind. Used by the client for drawing and platform logic. </summary>
	public static float GetSway(int i, int j, double factor = 0)
	{
		if (factor == 0)
			factor = TileSwaySystem.Instance.TreeWindCounter;

		return Main.instance.TilesRenderer.GetWindCycle(i, j, factor) * .4f;
	}
	public override void PreAddTileObjectData()
	{
		TileObjectData.newTile.AnchorValidTiles = [ValidAnchor];

		AddMapEntry(new Color(120, 80, 75));
		RegisterItemDrop(ModContent.ItemType<Items.Drywood.Drywood>());
		DustType = DustID.WoodFurniture;
	}

	public override bool IsTreeTop(int i, int j) => Main.tile[i, j - 1].TileType != Type && ModContent.GetModTile(Main.tile[i, j].TileType) is AcaciaTree 
		&& Main.tile[i, j].TileFrameX <= FrameSize * 5;

	public override void NearbyEffects(int i, int j, bool closer) //Spawn platforms
	{
		var pt = new Point16(i, j);

		if (IsTreeTop(i, j) && !Platforms.Where(x => x.TreePosition == pt).Any())
		{
			int type = SimpleEntitySystem.types[typeof(TreetopPlatform)];
			//Spawn our entity at direct tile coordinates where it can reposition itself after updating
			SimpleEntitySystem.NewEntity(type, pt.ToVector2());
		}
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		foreach (var item in base.GetItemDrops(i, j))
		{
			item.stack *= 2;
			yield return item;
		}
	}

	protected override void OnShakeTree(int i, int j)
	{
		var drop = new WeightedRandom<int>();

		drop.Add(ItemID.None, .8f);
		drop.Add(ModContent.ItemType<Items.Food.Caryocar>(), .2f);
		drop.Add(ModContent.ItemType<Items.Food.CustardApple>(), .2f);
		drop.Add(ModContent.ItemType<Items.BaobabFruit.BaobabFruit>(), .2f);
		drop.Add(ModContent.ItemType<Items.Drywood.Drywood>(), .8f);
		drop.Add(ItemID.Acorn, .7f);

		var position = new Vector2(i, j - 3) * 16;
		int dropType = (int)drop;
		if (dropType > ItemID.None)
			Item.NewItem(null, new Rectangle((int)position.X, (int)position.Y, 16, 16), dropType);

		GrowEffects(i, j, true);
	}

	public override void DrawTreeFoliage(int i, int j, SpriteBatch spriteBatch)
	{
		var position = new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(10, 0) + TreeHelper.GetPalmTreeOffset(i, j);
		float rotation = GetSway(i, j) * .08f;

		if (IsTreeTop(i, j)) //Draw treetops
		{
			const int framesY = 2;

			int frameY = Framing.GetTileSafely(i, j).TileFrameX / FrameSize % framesY;
			var source = TopTexture.Frame(1, framesY, 0, frameY, sizeOffsetY: -2);
			var origin = new Vector2(source.Width / 2, source.Height) - new Vector2(0, 2);

			spriteBatch.Draw(TopTexture.Value, position, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0);
		}
		else //Draw branches
		{
			const int framesX = 2;
			const int framesY = 3;

			int frameX = (Noise(new Vector2(i, j)) > 0) ? 1 : 0;
			int frameY = Framing.GetTileSafely(i, j).TileFrameX / FrameSize % framesY;
			var source = BranchTexture.Frame(framesX, framesY, frameX, frameY, -2, -2);
			var origin = new Vector2(frameX == 0 ? source.Width : 0, 44);

			position += new Vector2(6 * ((frameX == 0) ? -1 : 1), 8); //Directional offset

			spriteBatch.Draw(BranchTexture.Value, position, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0);
		}
	}

	protected override void OnGrowEffects(int i, int j, int height)
	{
		for (int h = 0; h < height; h++)
		{
			var center = new Vector2(i, j + h) * 16f + new Vector2(8);
			int range = 20;
			int loops = 3;

			if (h == 0)
			{
				center.Y -= 16 * 3;
				range = 80;
				loops = 20;
			}

			for (int g = 0; g < loops; g++)
				Gore.NewGorePerfect(new EntitySource_TileBreak(i, j), center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(range), 
					Main.rand.NextVector2Unit(), GoreID.TreeLeaf_Palm, .7f + Main.rand.NextFloat() * .6f);
		}
	}

	protected override void GenerateTree(int i, int j, int height)
	{
		int variance = WorldGen.genRand.Next(-8, 9) * 2;
		short xOff = 0;

		for (int h = 0; h < height; h++)
		{
			int style = WorldGen.genRand.NextBool(6) ? 1 : 0; //Rare segments

			WorldGen.PlaceTile(i, j - h, Type, true);
			var tile = Framing.GetTileSafely(i, j - h);

			if (tile.HasTile && tile.TileType == Type)
			{
				Framing.GetTileSafely(i, j - h).TileFrameX = (short)(style * FrameSize * 3 + WorldGen.genRand.Next(3) * FrameSize);
				Framing.GetTileSafely(i, j - h).TileFrameY = TreeHelper.GetPalmOffset(j, variance, height, ref xOff);
			}
		}

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j + 1 - height, 1, height, TileChangeType.None);
	}

	public bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		if (source is EntitySource_Parent { Entity: Projectile })
			return false;

		int id = Main.tile[i, j].TileType;

		if (TileCorruptor.GetConversionType<AcaciaTree, CorruptAcaciaTree, CrimsonAcaciaTree, HallowAcaciaTree>(id, type, out int conversionType))
		{
			Tile tile = Main.tile[i, j];
			tile.TileType = (ushort)conversionType;
		}

		if (Main.tile[i, j - 1].TileType == id)
			TileCorruptor.Convert(new EntitySource_TileUpdate(i, j), type, i, j - 1);

		return true;
	}
}

public class CorruptAcaciaTree : AcaciaTree
{
	protected override int ValidAnchor => ModContent.TileType<SavannaGrassCorrupt>();
}

public class CrimsonAcaciaTree : AcaciaTree
{
	protected override int ValidAnchor => ModContent.TileType<SavannaGrassCrimson>();
}

public class HallowAcaciaTree : AcaciaTree
{
	protected override int ValidAnchor => ModContent.TileType<SavannaGrassHallow>();
}