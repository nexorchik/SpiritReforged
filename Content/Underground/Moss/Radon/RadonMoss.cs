using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.Corruption;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;
using SpiritReforged.Content.Ocean.Tiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Moss.Radon;

[AutoloadGlowmask("255,255,255")]
public class RadonMoss : GrassTile
{
	protected override int[] DirtType => [TileID.Stone, TileID.GrayBrick];
	protected virtual Color MapColor => new(252, 248, 3);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		RegisterItemDrop(ItemID.StoneBlock);
		AddMapEntry(MapColor);
		this.Merge(TileID.Stone, TileID.GrayBrick);
		DustType = ModContent.DustType<RadonMossDust>();
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Yellow.ToVector3() * .3f);
		return true;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 4, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j)
	{
		Point16[] offset = [new Point16(0, -1), new Point16(-1, 0), new Point16(1, 0), new Point16(0, 1)];
		var coords = new Point16(i, j) + offset[Main.rand.Next(4)];

		var current = Framing.GetTileSafely(coords);
		if (!current.HasTile)
		{
			Placer.PlaceTile<RadonPlants>(coords.X, coords.Y, Main.rand.Next(RadonPlants.StyleRange));
			return;
		}
	}
}

[AutoloadGlowmask("255, 255, 255")]
public class RadonPlants : ModTile
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
		TileObjectData.newTile.AnchorAlternateTiles = [ModContent.TileType<RadonMoss>()];
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

		AddMapEntry(new Color(252, 248, 3));
		DustType = ModContent.DustType<RadonMossDust>();
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Yellow.ToVector3() * .35f);
		return true;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);
}