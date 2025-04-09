using RubbleAutoloader;
using SpiritReforged.Common.TileCommon.PresetTiles;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI;

namespace SpiritReforged.Content.Underground.Tiles;

public class StuffedPots : PotTile
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

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (!closer || !TileObjectData.IsTopLeft(i, j))
			return;

		var position = new Vector2(i, j).ToWorldCoordinates(16, 0);
		float chance = Main.LocalPlayer.DistanceSQ(position) / (200 * 200) + 5;

		if (Main.rand.NextFloat(chance) < .1f)
			EmoteBubble.NewBubble(EmoteID.EmotionAnger, new WorldUIAnchor(position + new Vector2(12, 0)), 60);
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (!Autoloader.IsRubble(Type))
			NPC.NewNPCDirect(new EntitySource_TileBreak(i, j), new Vector2(i, j).ToWorldCoordinates(16, 16), NPCID.SkeletonMerchant);

		base.KillMultiTile(i, j, frameX, frameY);
	}

	public override void DeathEffects(int i, int j, int frameX, int frameY)
	{
		var source = new EntitySource_TileBreak(i, j);
		var position = new Vector2(i, j).ToWorldCoordinates(16, 16);

		for (int g = 0; g < 3; g++)
		{
			int goreType = 51 + g;
			Gore.NewGore(source, position, Vector2.Zero, goreType);
		}
	}
}