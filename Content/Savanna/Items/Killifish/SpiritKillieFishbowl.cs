using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Items.Killifish;

public class SpiritKillieFishbowlItem : ModItem
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
		Item.createTile = ModContent.TileType<SpiritKillieFishbowl_Tile>();
		Item.placeStyle = 0;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
	}
	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(Mod.Find<ModItem>("KillifishItem").Type, 1);
		recipe.AddIngredient(ItemID.BottledWater, 1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
public class SpiritKillieFishbowl_Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Origin = new Point16(1, 2);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
		Main.tileFrameImportant[Type] = Main.tileFrameImportant[TileID.FishBowl];
		Main.tileLavaDeath[Type] = Main.tileLavaDeath[TileID.FishBowl];
		Main.tileSolidTop[Type] = Main.tileSolidTop[TileID.FishBowl];
		Main.tileTable[Type] = Main.tileTable[TileID.FishBowl];
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		AnimationFrameHeight = 54;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(200, 200, 200), name);
	}
	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
	
	bool activeAnimation = false;
	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		if (Main.rand.NextBool(100) && Main.netMode != NetmodeID.Server)
		{
			activeAnimation = true;
		}
		if (activeAnimation)
		{
			frameCounter++;
			if (frameCounter >= 6)
			{
				frameCounter = 0;
				frame++;
				frame %= 22;
			}
			if (frame == 16)
				activeAnimation = false;
		}
		else
		{
			frameCounter++;
			if (frameCounter >= 26)
			{
				frameCounter = 0;
				frame++;
				frame %= 22;
			}
			if (frame == 18)
				frame = 17;
		}
	}
}