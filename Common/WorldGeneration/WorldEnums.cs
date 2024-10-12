namespace SpiritReforged.Common.WorldGeneration;

public enum InterestType : byte
{
	FloatingIsland,
	EnchantedSword,
	Shimmer,
	Savanna,
	Hive,
	Count,
}

public enum VanillaChestID : byte
{
	Wood,
	Gold,
	LockedGold,
	Shadow,
	LockedShadow,
	Barrel,
	TrashCan,
	Ebonwood,
	Mahogany,
	Pearlwood,
	Ivy,
	Frozen,
	LivingWood,
	Sky,
	Shadewood,
	Webbed,
	Lihahzrd,
	Water,
	Jungle,
	Corruption,
	Crimson,
	Hallow,
	Ice,
	JungleLocked,
	CorruptionLocked,
	CrimsonLocked,
	HallowLocked,
	IceLocked,
	Dynasty,
	Honey,
	Steampunk,
	PalmWood,
	Mushroom,
	BorealWood,
	Slime,
	DungeonGreen,
	DungeonGreenLocked,
	DungeonPink,
	DungeonPinkLocked,
	DungeonBlue,
	DungeonBlueLocked,
	Bone,
	Cactus,
	Flesh,
	Obsidian,
	Pumpkin,
	Spooky,
	Glass,
	Martian,
	Meteorite,
	Granite,
	Marble, 
	Crystal, 
	Golden
}

public static class ChestTools
{
	/// <summary>
	/// Checks if the tile at i, j is a chest, and returns what kind of chest it is if so.
	/// </summary>
	/// <param name="i">X position.</param>
	/// <param name="j">Y position.</param>
	/// <param name="type">The type of the chest, if any.</param>
	/// <returns>If the tile is a chest or not.</returns>
	public static bool TryGetChestID(int i, int j, out VanillaChestID type)
	{
		Tile tile = Main.tile[i, j];
		type = VanillaChestID.Wood;
		
		if (tile.HasTile && tile.TileType == TileID.Containers && tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0)
		{
			type = (VanillaChestID)(tile.TileFrameX / 36);
			return true;
		}

		return false;
	}
}