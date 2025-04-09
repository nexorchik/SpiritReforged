using RubbleAutoloader;
using SpiritReforged.Common.TileCommon.PresetTiles;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class StuffedPots : PotTile, ILootTile
{
	public override Dictionary<string, int[]> TileStyles => new() { { string.Empty, [0, 1, 2] } };

	public override bool KillSound(int i, int j, bool fail)
	{
		if (fail || Autoloader.IsRubble(Type))
			return true;

		var pos = new Vector2(i, j).ToWorldCoordinates(16, 16);

		SoundEngine.PlaySound(SoundID.Shatter, pos);
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Tile/PotBreak") with { Volume = .16f, PitchRange = (-.4f, 0), }, pos);

		return true;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.dedServ)
			return;

		var source = new EntitySource_TileBreak(i, j);
		var position = new Vector2(i, j).ToWorldCoordinates(16, 16);

		for (int g = 0; g < 3; g++)
		{
			int goreType = 51 + g;
			Gore.NewGore(source, position, Vector2.Zero, goreType);
		}
	}

	public LootTable AddLoot(int objectStyle)
	{
		var loot = new LootTable();
		return loot;
	}
}