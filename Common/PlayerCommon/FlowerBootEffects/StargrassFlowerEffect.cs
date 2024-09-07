using SpiritReforged.Content.Forest.Stargrass.Tiles;

namespace SpiritReforged.Common.PlayerCommon.FlowerBootEffects;

internal class StargrassFlowerEffect : FlowerBootEffect
{
	public override bool CanPlaceOn(int x, int y, Player player) => Main.tile[x, y].TileType == ModContent.TileType<StargrassTile>();

	public override bool PlaceOn(int x, int y, Player player)
	{
		WorldGen.PlaceTile(x, y, ModContent.TileType<StargrassFlowers>(), true, false, -1, Main.rand.Next(26));

		if (!Main.rand.NextBool(3))
			StargrassTile.SpawnParticles(player);
		
		return true;
	}
}
