using SpiritReforged.Content.Ocean.Tiles;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Items.GarInAJar;

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
		//recipe.AddIngredient(ModContent.ItemType<LuvdiscItem>(), 1); TODO: Gar Item
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
	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		offsetY = 2; 
		Main.critterCage = true;
	}
	/* This is the vanilla way of doing fishbowls, where the animation is stuttered, pauses occasionally, and is more varied
	   It looks funny and kinda forces the gar to loop bumping into the wall, but occasionally looks glitchy.
	   I've commented it out for now, but anyone testing should compare it to the generic animation below and let me know if it works. 
	 */ 
	
	public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
	{
		Tile tile = Main.tile[i, j];
		int tileCageFrameIndex = TileDrawing.GetWaterAnimalCageFrame(i, j, tile.TileFrameX, tile.TileFrameY);
		frameYOffset = Main.fishBowlFrame[tileCageFrameIndex] * AnimationFrameHeight;
	}
}