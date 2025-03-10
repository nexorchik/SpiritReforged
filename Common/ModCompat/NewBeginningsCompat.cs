using SpiritReforged.Common.ItemCommon.Backpacks;
using SpiritReforged.Common.WorldGeneration.Micropasses.Passes;
using SpiritReforged.Common.WorldGeneration.PointOfInterest;
using SpiritReforged.Content.Forest.Backpacks;
using SpiritReforged.Content.Forest.Botanist.Items;
using SpiritReforged.Content.Forest.Botanist.Tiles;
using SpiritReforged.Content.Forest.Stargrass.Tiles;
using SpiritReforged.Content.Ocean.Items.KoiTotem;
using SpiritReforged.Content.Ocean.Items.Reefhunter.OceanPendant;
using SpiritReforged.Content.Ocean.Items.Vanity.DiverSet;
using SpiritReforged.Content.Savanna.Items.HuntingRifle;
using SpiritReforged.Content.Savanna.Items.Vanity;
using SpiritReforged.Content.Savanna.Tiles;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.ModCompat;

internal class NewBeginningsCompat : ModSystem
{
	public static Asset<Texture2D> GetIcon(string name) => ModContent.Request<Texture2D>("SpiritReforged/Common/ModCompat/NewBeginningsOrigins/" + name);

	public override void Load()
	{
		if (!ModLoader.TryGetMod("NewBeginnings", out Mod beginnings))
			return;

		beginnings.Call("Delay", () =>
		{
			AddDiver();
			AddBotanist();
			AddRecluse();
			AddHunter();
			AddHiker();
		});

		void AddDiver()
		{
			object equip = beginnings.Call("EquipData", ModContent.ItemType<DiverHead>(), ModContent.ItemType<DiverBody>(), ModContent.ItemType<DiverLegs>(),
				new int[] { ItemID.Flipper, ModContent.ItemType<OceanPendant>() });
			object misc = beginnings.Call("MiscData", 100, 20, -1);
			object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindBeachSpawnPoint);
			object result = beginnings.Call("ShortAddOrigin", GetIcon("Diver"), "ReforgedDiver",
				"Mods.SpiritReforged.Origins.Diver", Array.Empty<(int, int)>(), equip, misc, dele);
		}

		void AddBotanist()
		{
			object equip = beginnings.Call("EquipData", ModContent.ItemType<BotanistHat>(), ModContent.ItemType<BotanistBody>(), ModContent.ItemType<BotanistLegs>(),
				Array.Empty<int>());
			object misc = beginnings.Call("MiscData", 100, 20, -1, ItemID.Sickle);
			object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindScarecrowSpawnPoint);
			object result = beginnings.Call("ShortAddOrigin", GetIcon("Botanist"), "ReforgedBotanist",
				"Mods.SpiritReforged.Origins.Botanist", new (int, int)[] { (ItemID.HerbBag, 3) }, equip, misc, dele);
		}

		void AddRecluse()
		{
			object equip = beginnings.Call("EquipData", ItemID.AnglerHat, ItemID.None, ItemID.None, Array.Empty<int>());
			object misc = beginnings.Call("MiscData", 100, 20, -1);
			object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindRecluseSpawn);
			object result = beginnings.Call("ShortAddOrigin", GetIcon("Recluse"), "ReforgedRecluse",
				"Mods.SpiritReforged.Origins.Recluse", new (int, int)[] { (ItemID.FiberglassFishingPole, 1), (ItemID.MasterBait, 2), 
					(ItemID.ApprenticeBait, 10), (ItemID.WoodenCrate, 3), (ItemID.Torch, 25) }, equip, misc, dele);
		}

		void AddHunter()
		{
			object equip = beginnings.Call("EquipData", ModContent.ItemType<SafariHat>(), ModContent.ItemType<SafariVest>(), ModContent.ItemType<SafariShorts>(), 
				Array.Empty<int>());
			object misc = beginnings.Call("MiscData", 100, 20, -1, ModContent.ItemType<HuntingRifle>());
			object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindHunterSpawn);
			object result = beginnings.Call("ShortAddOrigin", GetIcon("Hunter"), "ReforgedHunter",
				"Mods.SpiritReforged.Origins.Hunter", new (int, int)[] { (ItemID.MusketBall, 60) }, equip, misc, dele);
		}

		void AddHiker()
		{
			object equip = beginnings.Call("EquipData", ItemID.None, ItemID.None, ItemID.None, Array.Empty<int>());
			object misc = beginnings.Call("MiscData", 100, 20, -1);
			object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindHighestSurface, 
				(Action<Player>)AddHikerBackpack);
			object result = beginnings.Call("ShortAddOrigin", GetIcon("Hiker"), "ReforgedHiker",
				"Mods.SpiritReforged.Origins.Hiker", new (int, int)[] { (ItemID.Rope, 150), (ItemID.Glowstick, 30), (ItemID.Torch, 60) }, equip, misc, dele);
		}
	}

	private Point16 FindHighestSurface()
	{
		int dir = WorldGen.genRand.NextBool() ? -1 : 1;
		Point position = new(Main.spawnTileX, Main.spawnTileY);
		Point16 highest = new(Main.spawnTileX, Main.spawnTileY);

		HashSet<int> moveDownTiles = [TileID.Cloud, TileID.RainCloud];
		HashSet<int> skipVertTiles = [TileID.LivingWood, TileID.LeafBlock];
		HashSet<int> grasses = [TileID.Grass, ModContent.TileType<StargrassTile>(), ModContent.TileType<SavannaGrass>()];

		while (WorldGen.InWorld(position.X, position.Y, 40))
		{
			position.X += dir;

			if (Main.tile[position].HasTile && !skipVertTiles.Contains(Main.tile[position].TileType))
			{
				while (WorldGen.SolidOrSlopedTile(position.X, position.Y))
				{
					if (!moveDownTiles.Contains(Main.tile[position.X, position.Y].TileType))
						position.Y--;
					else
						position.Y++;
				}
			}

			if (highest.Y > position.Y && grasses.Contains(Main.tile[position.X, position.Y + 1].TileType))
				highest = new Point16(position.X, position.Y);
		}

		return highest;
	}

	private void AddHikerBackpack(Player player)
	{
		player.GetModPlayer<BackpackPlayer>().backpack = new Item(ModContent.ItemType<LeatherBackpack>(), 1);
		(player.GetModPlayer<BackpackPlayer>().backpack.ModItem as BackpackItem).items[0] = new Item(ItemID.CalmingPotion, 3);
	}

	private static Point16 FindHunterSpawn()
	{
		List<Point16> spawns = [];

		for (int x = 200; x < Main.maxTilesX - 200; ++x)
		{
			for (int y = (int)(Main.worldSurface * 0.35f); y < Main.worldSurface + 120; ++y)
			{
				Tile tile = Main.tile[x, y];

				if (tile.HasTile && tile.TileType == ModContent.TileType<SavannaGrass>())
					spawns.Add(new Point16(x, y - 3));
			}
		}

		if (spawns.Count == 0)
			return new Point16(Main.spawnTileX, Main.spawnTileY - 3);

		return WorldGen.genRand.Next([.. spawns]);
	}

	private static Point16 FindRecluseSpawn()
	{
		while (true)
		{
			Point16 pos = WorldGen.genRand.Next([.. FishingAreaMicropass.CovePositions]);

			if (ScanForKoi(pos, out Point16 newPos))
			{
				return newPos;
			}
		}
	}

	private static bool ScanForKoi(Point16 pos, out Point16 resultPos)
	{
		resultPos = Point16.Zero;

		for (int i = pos.X - 10; i < pos.X + 80; ++i)
		{
			for (int j = pos.Y - 10; j < pos.Y + 60; ++j)
			{
				if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<KoiTotemTile>())
				{
					resultPos = new Point16(i, j);
					return true;
				}
			}
		}

		return false;
	}

	private static Point16 FindScarecrowSpawnPoint()
	{
		Point16[] poI = [.. PointOfInterestSystem.Instance.WorldGen_PointsOfInterestByPosition[InterestType.Curiosity]];
		Point16 pos = WorldGen.genRand.Next(poI);

		while (CantFindScarecrowNearby(pos))
		{
			pos = WorldGen.genRand.Next(poI);
		}

		return pos;
	}

	private static bool CantFindScarecrowNearby(Point16 pos)
	{
		for (int i = pos.X - 60; i < pos.X + 60; ++i)
		{
			for (int j = pos.Y - 40; j < pos.Y + 40; ++j)
			{
				if (Main.tile[i, j].TileType == ModContent.TileType<Scarecrow>())
					return false;
			}
		}

		return true;
	}

	public static Point16 FindBeachSpawnPoint()
	{
		bool left = WorldGen.genRand.NextBool(2);
		int x = left ? 280 : Main.maxTilesX - 280;
		int y = 80;

		while (Main.tile[x, y].LiquidAmount <= 0 && !Main.tile[x, y].HasTile)
			y++;

		return new Point16(x, y - 8);
	}
}
