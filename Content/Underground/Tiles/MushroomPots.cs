using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using Terraria.Audio;

namespace SpiritReforged.Content.Underground.Tiles;

[AutoloadGlowmask("200,200,200")]
public class MushroomPots : PotTile, ILootTile
{
	public override Dictionary<string, int[]> TileStyles => new() { { string.Empty, [0, 1, 2] } };

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (effectOnly || fail)
			return;

		//Do vanilla pot break effects
		var t = Main.tile[i, j];
		short oldFrameY = t.TileFrameY;

		t.TileFrameY = t.TileFrameX;
		WorldGen.CheckPot(i, j);
		t.TileFrameY = oldFrameY;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Blue.ToVector3() * .8f);
		return true;
	}

	public override bool KillSound(int i, int j, bool fail)
	{
		if (!fail)
		{
			var pos = new Vector2(i, j).ToWorldCoordinates(16, 16);
			SoundEngine.PlaySound(SoundID.Shatter, pos);

			return false;
		}

		return true;
	}

	public LootTable AddLoot(int objectStyle) => ModContent.GetInstance<Pots>().AddLoot(objectStyle);
}