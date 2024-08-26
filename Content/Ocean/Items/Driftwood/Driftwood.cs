using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class Driftwood1Item : FloatingItem
{
	public override float SpawnWeight => 0.9f;
	public override float Weight => base.Weight * 0.9f;
	public override float Bouyancy => base.Bouyancy * 1.05f;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.createTile = ModContent.TileType<Driftwood1Tile>();
		Item.maxStack = Item.CommonMaxStack;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
	}
	public override void AddRecipes()
	{
		var recipe = Recipe.Create(ModContent.ItemType<DriftwoodTileItem>(), 10);
		recipe.AddIngredient(this, 1);
		recipe.Register();
	}
	public override bool OnPickup(Player player)
	{
		player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "Pickup"), ModContent.ItemType<DriftwoodTileItem>(), 10);
		return false;
	}
}

public class Driftwood1Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); //facing right will use the second texture style
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		// name.SetDefault("Driftwood");
		AddMapEntry(new Color(69, 54, 43), name);
		DustType = DustID.Stone;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}

public class Driftwood2Item : FloatingItem
{
	public override float Weight => base.Weight * 0.9f;
	public override float Bouyancy => base.Bouyancy * 1.05f;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.createTile = ModContent.TileType<Driftwood2Tile>();
		Item.maxStack = Item.CommonMaxStack;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
	}
	public override void AddRecipes()
	{
		var recipe = Recipe.Create(ModContent.ItemType<DriftwoodTileItem>(), 20);
		recipe.AddIngredient(this, 1);
		recipe.Register();
	}
	public override bool OnPickup(Player player)
	{
		player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "Pickup"), ModContent.ItemType<DriftwoodTileItem>(), 20);
		return false;
	}
}

public class Driftwood2Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 4;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); //facing right will use the second texture style
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		// name.SetDefault("Driftwood");
		AddMapEntry(new Color(69, 54, 43), name);
		DustType = DustID.Stone;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}

public class Driftwood3Item : FloatingItem
{
	public override float Weight => base.Weight * 0.9f;
	public override float Bouyancy => base.Bouyancy * 1.05f;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.createTile = ModContent.TileType<Driftwood3Tile>();
		Item.maxStack = Item.CommonMaxStack;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
	}
	public override void AddRecipes()
	{
		var recipe = Recipe.Create(ModContent.ItemType<DriftwoodTileItem>(), 25);
		recipe.AddIngredient(this, 1);
		recipe.Register();
	}
	public override bool OnPickup(Player player)
	{
		player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "Pickup"), ModContent.ItemType<DriftwoodTileItem>(), 25);
		return false;
	}
}

public class Driftwood3Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 4;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); //facing right will use the second texture style
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		// name.SetDefault("Driftwood");
		AddMapEntry(new Color(69, 54, 43), name);
		DustType = DustID.Stone;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}