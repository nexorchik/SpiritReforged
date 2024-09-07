using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class LargeVentItem : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.value = 0;
		Item.rare = ItemRarityID.Blue;
		Item.createTile = ModContent.TileType<Breakable1x3Vent>();
		Item.maxStack = Item.CommonMaxStack;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
	}

	public override bool? UseItem(Player player)
	{
		Item.placeStyle = Main.rand.Next(2);
		return null;
	}

	public override void AddRecipes()
	{
		var recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.AshBlock, 20);
		recipe.AddIngredient(ItemID.Obsidian, 1);
		recipe.AddTile(TileID.TinkerersWorkbench);
		recipe.Register();
	}
}

[TileTag(TileTags.Indestructible)]
public class HydrothermalVent1x3 : HydrothermalVent
{
	public override void StaticDefaults()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(0, 2);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<LargeVentItem>();
	}
}

public class Breakable1x3Vent : HydrothermalVent1x3
{
	public override string Texture => base.Texture.Replace(nameof(Breakable1x3Vent), nameof(HydrothermalVent1x3));

	public override bool CanExplode(int i, int j) => true;
	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => true;
}
