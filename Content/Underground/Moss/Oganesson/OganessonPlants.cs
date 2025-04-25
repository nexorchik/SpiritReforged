using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.CheckItemUse;
using SpiritReforged.Content.Dusts;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Moss.Oganesson;

public class OganessonPlants : ModTile, ICheckItemUse
{
	public const int StyleRange = 3;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileCut[Type] = true;

		TileID.Sets.SwaysInWindBasic[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<OganessonMoss>(), ModContent.TileType<OganessonMossGrayBrick>()];
		TileObjectData.newTile.RandomStyleRange = StyleRange;
		TileObjectData.newTile.StyleHorizontal = true;

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
		int heldType = Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(), 16, 16)].HeldItem.type;

		if (heldType is ItemID.PaintScraper or ItemID.SpectrePaintScraper)
		{
			if (Main.rand.NextBool(9))
				yield return new Item(ModContent.ItemType<OganessonMossItem>());
		}
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.3f, 0.3f, 0.3f);

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile t = Main.tile[i, j];

		var texture = TextureAssets.Tile[Type].Value;
		var source = new Rectangle(t.TileFrameX, t.TileFrameY, 18, 18);
		var position = new Vector2(i, j) * 16 - Main.screenPosition + TileExtensions.TileOffset;

		position += (t.TileFrameX / 18 / StyleRange) switch
		{
			1 => new Vector2(2, 0),
			2 => new Vector2(-2, 0),
			3 => new Vector2(0, -2),
			_ => new Vector2(0, 2),
		};

		spriteBatch.Draw(texture, position, source, Color.White, 0, Vector2.Zero, 1, default, 0);
		return false;
	}

	public virtual bool? CheckItemUse(int type, int i, int j)
	{
		if (type is ItemID.PaintScraper or ItemID.SpectrePaintScraper)
		{
			WorldGen.KillTile(i, j);
			return true;
		}

		return null;
	}
}
