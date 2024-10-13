using SpiritReforged.Common.TileCommon;

namespace SpiritReforged.Content.Ocean.Hydrothermal.Tiles;

public class Magmastone : ModTile, IAutoloadTileItem
{
	private static Asset<Texture2D> glowTexture;
	public override void Load() => glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;

		Main.tileMerge[Type][TileID.Sand] = true; //Ensure magmastone tries to merge with sand
		Main.tileMerge[TileID.Sand][Type] = true; //Ensure sand tries to merge back with magmastone
		Main.tileMerge[Type][TileID.HardenedSand] = true;
		Main.tileMerge[TileID.HardenedSand][Type] = true;
		Main.tileMerge[Type][ModContent.TileType<Gravel>()] = true;
		Main.tileMerge[ModContent.TileType<Gravel>()][Type] = true;
		TileID.Sets.CanBeDugByShovel[Type] = true;

		AddMapEntry(new Color(200, 160, 80));
		DustType = DustID.Asphalt;
		MineResist = .5f;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var t = Framing.GetTileSafely(i, j);
		if (t.Slope != SlopeType.Solid || t.IsHalfBlock)
			return;

		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		spriteBatch.Draw(glowTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(t.TileFrameX, t.TileFrameY, 16, 16), GetColor(i, j));
	}

	private static Color GetColor(int i, int j)
	{
		var defaultColor = Lighting.GetColor(i, j) * 2;
		int desVal = HydrothermalVent.cooldownMax - HydrothermalVent.eruptDuration;
		const int range = 10;

		foreach (var cooldown in HydrothermalVent.cooldowns)
		{
			if (cooldown.Value > desVal && cooldown.Key.ToVector2().Distance(new Vector2(i, j)) <= range)
				return Color.Lerp(defaultColor, Color.White, (cooldown.Value - desVal) / (float)HydrothermalVent.eruptDuration);
		} //Glow brightly when a nearby vent is erupting

		return defaultColor;
	}
}