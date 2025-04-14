using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.CheckItemUse;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;
using SpiritReforged.Content.Underground.Moss.Oganesson;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Moss.Radon;

[AutoloadGlowmask("255,255,255")]
public class RadonMoss : GrassTile
{
	protected override int DirtType => TileID.Stone;
	protected virtual Color MapColor => new(252, 248, 3);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		RegisterItemDrop(ItemID.StoneBlock);
		AddMapEntry(MapColor);
		this.Merge(TileID.Stone, TileID.GrayBrick);

		DustType = ModContent.DustType<RadonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Yellow.ToVector3() * .2f);
		return true;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<RadonMossGrayBrick>(), 1, TileID.GrayBrick) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to gray bricks

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j) => TileExtensions.PlacePlant<RadonPlants>(i, j, Main.rand.Next(RadonPlants.StyleRange));
}

[AutoloadGlowmask("255,255,255")]
public class RadonMossGrayBrick : GrassTile
{
	protected override int DirtType => TileID.GrayBrick;
	protected virtual Color MapColor => new(252, 248, 3);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		Main.tileMoss[Type] = true;

		RegisterItemDrop(ItemID.GrayBrick);
		AddMapEntry(MapColor);
		this.Merge(TileID.Stone, TileID.GrayBrick);
		
		HitSound = SoundID.Grass;
		DustType = ModContent.DustType<RadonMossDust>();
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Yellow.ToVector3() * .2f);
		return true;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<RadonMoss>(), 1, TileID.Stone) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to stone

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j) => TileExtensions.PlacePlant<RadonPlants>(i, j, Main.rand.Next(RadonPlants.StyleRange));
}

[AutoloadGlowmask("255, 255, 255")]
public class RadonPlants : ModTile, ICheckItemUse
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
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<RadonMoss>(), ModContent.TileType<RadonMossGrayBrick>()];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = StyleRange;
		TileObjectData.newTile.DrawYOffset = 2;

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
		TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
		TileObjectData.newAlternate.DrawXOffset = 2;
		TileObjectData.newAlternate.DrawYOffset = 0;
		TileObjectData.addAlternate(StyleRange);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
		TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
		TileObjectData.newAlternate.DrawXOffset = -2;
		TileObjectData.newAlternate.DrawYOffset = 0;
		TileObjectData.addAlternate(StyleRange * 2);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
		TileObjectData.newAlternate.DrawYOffset = -2;
		TileObjectData.addAlternate(StyleRange * 3);

		TileObjectData.addTile(Type);

		AddMapEntry(new Color(206, 209, 23));
		
		DustType = ModContent.DustType<RadonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		int heldType = Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(0, 0), 16, 16)].HeldItem.type;

		if (heldType == ItemID.PaintScraper || heldType == ItemID.SpectrePaintScraper)
		{
			if (Main.rand.NextBool(9))
				yield return new Item(ModContent.ItemType<RadonMossItem>());
		}
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		int style = tileFrameX / 18;

		if (style >= StyleRange * 3)
			offsetY = -2;
		else if (style >= StyleRange * 2)
			width += 2;
		else if (style >= StyleRange)
			width -= 2;
		else
			offsetY = 2;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Yellow.ToVector3() * .2f);
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