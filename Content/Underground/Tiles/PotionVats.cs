using RubbleAutoloader;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.Audio;
using Terraria.DataStructures;
using SpiritReforged.Common.Misc;

namespace SpiritReforged.Content.Underground.Tiles;

[AutoloadGlowmask("200,200,200")]
public class PotionVats : PotTile
{
	private static Asset<Texture2D> FluidTexture;

	public override Dictionary<string, int[]> TileStyles => new()
	{
		{ "Antique", [0, 1, 2, 3] },
		{ "Cloning", [4, 5, 6, 7] }
	};

	public override void AddRecord(int type, StyleDatabase.StyleGroup group) => RecordHandler.Records.Add(new TileRecord(group.name, type, group.styles).AddRating(4));
	public override void AddObjectData()
	{
		Main.tileCut[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
		TileObjectData.newTile.Origin = new(1, 4);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.StyleWrapLimit = 4;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		FluidTexture = ModContent.Request<Texture2D>(Texture + "_Fluid");
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (effectOnly || Autoloader.IsRubble(Type))
			return;

		fail = AdjustFrame(i, j);
	}

	public override bool KillSound(int i, int j, bool fail)
	{
		if (fail || Autoloader.IsRubble(Type))
			return true;

		var pos = new Vector2(i, j).ToWorldCoordinates(16, 16);

		SoundEngine.PlaySound(SoundID.Shatter with { Pitch = .5f }, pos);
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Tile/PotBreak") with { Volume = .16f, Pitch = .8f, }, pos);

		return true;
	}

	private static bool AdjustFrame(int i, int j)
	{
		const int fullWidth = 54;

		TileExtensions.GetTopLeft(ref i, ref j);

		if (Main.tile[i, j].TileFrameX > fullWidth * 2)
			return false; //Frame has already been adjusted to capacity

		for (int x = i; x < i + 3; x++)
		{
			for (int y = j; y < j + 5; y++)
			{
				var t = Main.tile[x, y];
				t.TileFrameX += fullWidth;
			}
		}

		return true;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var texture = FluidTexture.Value;
		var position = new Vector2(i, j) * 16 - Main.screenPosition + TileExtensions.TileOffset + new Vector2(0, 2);

		var t = Main.tile[i, j];
		var frame = new Rectangle(t.TileFrameX, t.TileFrameY, 16, 16);

		var color = Lighting.GetColor(i, j).MultiplyRGBA(Main.DiscoColor.Additive(150));
		spriteBatch.Draw(texture, position, frame, color);

		/*var square = new SquarePrimitive()
		{
			Color = Color.White,
			Height = 30,
			Length = 20,
			Position = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition
		};

		PrimitiveRenderer.DrawPrimitiveShapeBatched([square]);*/
		return true;
	}
}