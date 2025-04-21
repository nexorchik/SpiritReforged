using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes;

internal class NewStatuesMicropass : Micropass
{
	/// <summary> The list of statues to generate in this micropass. </summary>
	public static readonly List<int> Statues = [];

	public override string WorldGenName => "New Statues";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex)
	{
		afterIndex = true;
		return passes.FindIndex(genpass => genpass.Name.Equals("Statues"));
	}

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		const int maxTries = 5000; //Failsafe
		const int numPerType = 4;

		progress.Message = Lang.gen[29].Value; //Localization for `Statues`

		int maxStatues = Main.maxTilesX / WorldGen.WorldSizeSmallX * numPerType * Statues.Count;
		int statues = 0;

		for (int t = 0; t < maxTries; t++)
		{
			int x = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
			int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.UnderworldLayer - 20);

			WorldMethods.FindGround(x, ref y);

			if (y > Main.UnderworldLayer || WorldGen.oceanDepths(x, y))
				continue;

			int type = Statues[Math.Clamp(statues / numPerType, 0, Statues.Count - 1)];
			if (CreateStatue(x, y - 1, type) && ++statues >= maxStatues)
				break;
		}
	}

	private static bool CreateStatue(int x, int y, int type)
	{
		WorldGen.PlaceTile(x, y, type, true);
		return Main.tile[x, y].TileType == type;
	}
}