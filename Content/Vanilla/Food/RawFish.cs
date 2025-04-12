using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.ModCompat.Classic;

namespace SpiritReforged.Content.Vanilla.Food;

[FromClassic("RawFish")]
public class RawFish : FoodItem
{
	internal override Point Size => new(34, 22);

	public override void StaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<RawMeat>();

	public override bool CanUseItem(Player player)
	{
		player.AddBuff(BuffID.Poisoned, 600);
		return true;
	}

	public override void AddRecipes()
	{
		Recipe.Create(ItemID.CookedFish).AddIngredient(ModContent.ItemType<RawFish>()).AddTile(TileID.CookingPots).Register();
		Recipe.Create(ItemID.Sashimi).AddIngredient(ModContent.ItemType<RawFish>()).AddTile(TileID.WorkBenches).Register();
	}
}
