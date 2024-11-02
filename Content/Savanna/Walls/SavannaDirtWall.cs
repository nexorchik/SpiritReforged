using SpiritReforged.Common.WallCommon;

namespace SpiritReforged.Content.Savanna.Walls;

public class SavannaDirtWall : ModWall, IAutoloadWallItem
{
	public void AddItemRecipes(ModItem item)
	{
		var mod = SpiritReforgedMod.Instance; //Mod is null here, so get the instance manually

		item.CreateRecipe(4)
			.AddIngredient(mod.Find<ModItem>("SavannaDirtItem").Type)
			.AddTile(TileID.WorkBenches)
			.Register();

		//Allow wall items to be crafted back into base materials
		Recipe.Create(mod.Find<ModItem>("SavannaDirtItem").Type)
			.AddIngredient(item.Type, 4)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override void SetStaticDefaults() => AddMapEntry(new Color(98, 39, 5));
}