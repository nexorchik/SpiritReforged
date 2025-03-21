using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Misc;
using SpiritReforged.Common.TileCommon;

namespace SpiritReforged.Content.Savanna.Tiles;

public class Drywood : ModTile, IAutoloadTileItem
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;

		DustType = DustID.WoodFurniture;
		this.Merge(TileID.WoodBlock, TileID.Sand, ModContent.TileType<SavannaDirt>());
		AddMapEntry(new Color(145, 128, 109));

		//Set item static defaults
		var item = this.AutoItem();

		ItemID.Sets.ShimmerTransformToItem[item.type] = ItemID.Wood;
		Recipes.AddToGroup(RecipeGroupID.Wood, item.type);
		item.ResearchUnlockCount = 100;
	}
}