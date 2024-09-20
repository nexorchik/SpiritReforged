namespace SpiritReforged.Content.Savanna.Items.Gar;

public class GarInAJar : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 28;
		Item.value = 500;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTime = 10;
		Item.useAnimation = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.createTile = ModContent.TileType<GarInAJar_Tile>();
		Item.placeStyle = 0;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
	}
	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(Mod.Find<ModItem>("GarItem").Type, 1);
		recipe.AddIngredient(ItemID.BottledWater, 1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
public class GarInAJar_Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.UsesCustomCanPlace = true;
		Main.tileFrameImportant[Type] = Main.tileFrameImportant[TileID.FishBowl];
		Main.tileLavaDeath[Type] = Main.tileLavaDeath[TileID.FishBowl];
		Main.tileSolidTop[Type] = Main.tileSolidTop[TileID.FishBowl];
		Main.tileTable[Type] = Main.tileTable[TileID.FishBowl];
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		AnimationFrameHeight = 36;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(200, 200, 200), name);
	}
	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		frameCounter++;
		if (frameCounter >= 10)
		{
			frameCounter = 0;
			frame++;
			frame %= 19;
		}
	}
}