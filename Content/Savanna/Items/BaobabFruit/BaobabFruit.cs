using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Savanna.Items.BaobabFruit;

public class BaobabFruit : FoodItem
{
	internal override Point Size => new(24, 24);
	public override void StaticDefaults() => FruitItemsSet.Add(Type);
}
