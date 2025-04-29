using SpiritReforged.Content.Underground.Pottery;
using SpiritReforged.Content.Underground.Tiles;
using SpiritReforged.Content.Underground.Tiles.Potion;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes;

internal class PotteryStructureMicropass : Micropass
{
	public override string WorldGenName => "Pottery Structures";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex)
	{
		afterIndex = false;
		return passes.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
	}

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		const int maxTries = 1000; //Failsafe

		progress.Message = Language.GetTextValue("Mods.SpiritReforged.Generation.Pottery");

		int maxStructures = Main.maxTilesX / WorldGen.WorldSizeSmallX * 5;
		int structures = 0;

		for (int t = 0; t < maxTries; t++)
		{
			int x = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
			int y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer - 20);

			WorldMethods.FindGround(x, ref y);

			if (y > Main.UnderworldLayer || WorldGen.oceanDepths(x, y))
				continue;

			if (CreateStructure(x, y) && ++structures >= maxStructures)
				break;
		}
	}

	public static bool CreateStructure(int x, int y)
	{
		int radius = 20 + (int)((float)Main.maxTilesX / WorldGen.WorldSizeSmallX * 5f);
		var pt = new Point(x, y);
		Rectangle area = new(x - radius, y - radius, radius * 2, radius * 2);

		if (!GenVars.structures.CanPlace(area, 4))
			return false;

		Dictionary<ushort, int> typeToCount = [];
		WorldUtils.Gen(pt - new Point(radius, radius), new Shapes.Rectangle(area.Width, area.Height), new Actions.TileScanner(TileID.Stone).Output(typeToCount));

		if (typeToCount[TileID.Stone] < radius * radius * .8f)
			return false; //Not enough stone

		ShapeData data = new();
		WorldUtils.Gen(pt, new Shapes.Slime(radius, 1.0, 0.8), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.Clear()).Output(data));
		WorldUtils.Gen(pt + new Point(0, 10), new Shapes.Circle(radius + 4, 5), Actions.Chain(new Modifiers.IsNotSolid(), new Actions.SetTile(TileID.Stone)).Output(data));
		WorldUtils.Gen(pt, new ModShapes.All(data), Actions.Chain(new Actions.SetFrames(true), new Modifiers.Blotches(), new Modifiers.IsTouchingAir(), new Actions.Smooth()));

		int halfWidth = radius + 4;
		WorldUtils.Gen(pt - new Point(halfWidth, 2), new Shapes.Rectangle(halfWidth * 2, 1), Actions.Chain(new Modifiers.IsNotSolid(), new Actions.PlaceTile(TileID.Platforms)));
		
		for (int i = 0; i < 3; i++)
			CreateColumn(x - radius + (int)(radius * 2 * (i / 2f)), y - 2);

		for (int i = 0; i < 9; i++)
		{
			var webPoint = WorldGen.genRand.NextVector2FromRectangle(area).ToPoint();
			if (WorldGen.SolidOrSlopedTile(webPoint.X, webPoint.Y))
			{
				i--;
				continue;
			}

			ushort[] types = [TileID.Stone, TileID.Platforms];
			WorldUtils.Gen(webPoint, new Shapes.Circle(4), Actions.Chain(new Modifiers.Blotches(3), new Modifiers.Dither(),
				new Modifiers.IsTouching(true, types), new Modifiers.SkipTiles(types), new Actions.PlaceTile(TileID.Cobweb)));
		}

		WorldMethods.FindGround(x, ref y);
		WorldGen.PlaceTile(x, y - 1, ModContent.TileType<PotteryWheel>(), true);

		AddPots(area);
		return true;
	}

	private static void CreateColumn(int x, int y)
	{
		while (WorldGen.InWorld(x, y, 20) && !WorldGen.SolidTile(x, y))
		{
			WorldGen.PlaceWall(x - 1, y, WallID.WoodenFence);
			WorldGen.PlaceWall(x, y, WallID.Wood);
			WorldGen.PlaceWall(x + 1, y, WallID.WoodenFence);

			y++;
		}
	}

	private static void AddPots(Rectangle area)
	{
		WeightedRandom<int> selection = new();
		selection.Add(ModContent.TileType<BiomePots>());
		selection.Add(ModContent.TileType<UpsideDownPot>(), .005f);
		selection.Add(ModContent.TileType<OrnatePots>(), .008f);
		selection.Add(ModContent.TileType<WormPot>(), .05f);
		selection.Add(ModContent.TileType<SilverPlatters>(), .05f);
		selection.Add(ModContent.TileType<ScryingPot>(), .03f);
		selection.Add(ModContent.TileType<PotionVats>(), .06f);

		for (int i = 0; i < 200; i++)
		{
			var random = WorldGen.genRand.NextVector2FromRectangle(area).ToPoint();
			while (WorldGen.InWorld(random.X, random.Y, 20) && !WorldGen.SolidTile(random.X, random.Y + 1) && !Main.tileSolidTop[Framing.GetTileSafely(random.X, random.Y + 1).TileType])
				random.Y++;
			if (WorldGen.SolidOrSlopedTile(random.X, random.Y))
				continue;

			if (WorldGen.genRand.NextBool(5))
			{
				PotsMicropass.CreateStack(random.X, random.Y);
			}
			else
			{
				int type = selection;
				int range = TileObjectData.GetTileData(type, 0)?.RandomStyleRange ?? 1;

				WorldGen.PlaceTile(random.X, random.Y, type, true, style: WorldGen.genRand.Next(range));
			}
		}
	}
}