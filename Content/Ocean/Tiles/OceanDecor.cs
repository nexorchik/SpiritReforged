using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class OceanDecor1x2 : ModTile
{
	private static readonly Dictionary<int, Asset<Texture2D>> _GlowById = [];

	public override void SetStaticDefaults()
	{
		_GlowById.Add(Type, ModContent.Request<Texture2D>(Texture + "_Glow"));

		Main.tileSolid[Type] = false;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileFrameImportant[Type] = true;
		
		AddTileObjectData();

		AddMapEntry(new Color(121, 92, 19));
		DustType = DustID.Coralstone;
		HitSound = SoundID.Dig;
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		Tile tile = Main.tile[i, j];
		var data = TileObjectData.GetTileData(tile);

		if (tile.TileFrameX >= data.Width * 18)
			type = DustID.Coralstone;
		else
			type = DustID.Grass;

		return true;
	}

	protected virtual void AddTileObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.Origin = new(0, 1);
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Pearlsand, TileID.Ebonsand];
		TileObjectData.addTile(Type);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];

		spriteBatch.Draw(_GlowById[Type].Value, this.DrawPosition(i, j), new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
	}
}

public class OceanDecor2x2 : OceanDecor1x2
{
	protected override void AddTileObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.RandomStyleRange = 4;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new(1, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Pearlsand, TileID.Ebonsand];
		TileObjectData.addTile(Type);
	}
}

public class OceanDecor2x3 : OceanDecor1x2
{
	protected override void AddTileObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
		TileObjectData.newTile.RandomStyleRange = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(1, 2);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Pearlsand, TileID.Ebonsand];
		TileObjectData.addTile(Type);
	}
}
