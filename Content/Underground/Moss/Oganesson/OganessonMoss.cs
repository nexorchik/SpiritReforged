using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.CheckItemUse;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;
using SpiritReforged.Content.Underground.Moss.Radon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Moss.Oganesson;

[AutoloadGlowmask("255,255,255")]
public class OganessonMoss : GrassTile
{
	protected override int DirtType => TileID.Stone;
	protected virtual Color MapColor => new(220, 220, 220);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		Main.tileMoss[Type] = true;

		RegisterItemDrop(ItemID.StoneBlock);
		AddMapEntry(MapColor);
		this.Merge(TileID.Stone, TileID.GrayBrick);

		DustType = ModContent.DustType<OganessonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.White.ToVector3() * .3f);
		return true;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<OganessonMossGrayBrick>(), 1, TileID.GrayBrick) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to gray bricks

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j) => TileExtensions.PlacePlant<OganessonPlants>(i, j, Main.rand.Next(OganessonPlants.StyleRange));
}

[AutoloadGlowmask("255,255,255")]
public class OganessonMossGrayBrick : GrassTile
{
	protected override int DirtType => TileID.GrayBrick;
	protected virtual Color MapColor => new(220, 220, 220);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		RegisterItemDrop(ItemID.GrayBrick);
		AddMapEntry(MapColor);
		this.Merge(TileID.Stone, TileID.GrayBrick);

		DustType = ModContent.DustType<OganessonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.White.ToVector3() * .3f);
		return true;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<OganessonMoss>(), 1, TileID.Stone) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to stone

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j) => TileExtensions.PlacePlant<OganessonPlants>(i, j, Main.rand.Next(OganessonPlants.StyleRange));
}

[AutoloadGlowmask("255, 255, 255")]
public class OganessonPlants : ModTile, ICheckItemUse
{
	public const int StyleRange = 3;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileCut[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<OganessonMoss>(), ModContent.TileType<OganessonMossGrayBrick>()];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = StyleRange;

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
		TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
		TileObjectData.addAlternate(StyleRange);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
		TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
		TileObjectData.addAlternate(StyleRange * 2);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
		TileObjectData.addAlternate(StyleRange * 3);

		TileObjectData.addTile(Type);

		AddMapEntry(new Color(180, 180, 180));

		DustType = ModContent.DustType<OganessonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		int heldType = Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(0, 0), 16, 16)].HeldItem.type;

		if (heldType == ItemID.PaintScraper || heldType == ItemID.SpectrePaintScraper)
		{
			if (Main.rand.NextBool(9))
				yield return new Item(ModContent.ItemType<OganessonMossItem>());
		}
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		int style = tileFrameX / 18;
		if (style < StyleRange * 3)
			offsetY = 2;
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.White.ToVector3() * .35f);
		return true;
	}

	public bool? CheckItemUse(int type, int i, int j)
	{
		if (type == ItemID.PaintScraper || type == ItemID.SpectrePaintScraper)
		{
			WorldGen.KillTile(i, j);
			return true;
		}

		return null;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);
}
