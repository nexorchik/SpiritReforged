using SpiritReforged.Content.Savanna.NPCs.Gar;

namespace SpiritReforged.Content.Savanna;

public class SavannaGlobalItem : GlobalItem
{
	public override bool InstancePerEntity => true;
	public override bool? UseItem(Item item, Player player)
	{
		if (player.GetModPlayer<SavannaPlayer>().quenchPotion)
		{
			if (item.useStyle == ItemUseStyleID.DrinkLiquid && item.buffType > 0 && item.buffTime > 0)
			{
				if (!player.HasBuff(item.buffType))
				{
					player.AddBuff(item.buffType, item.buffTime);

					for (int i = 0; i < player.buffType.Length; i++)
					{
						if (player.buffType[i] == item.buffType)
						{
							player.buffTime[i] = (int)(player.buffTime[i] * 1.25f);
							break;
						}
					}
				}
			}
		}	

		return null;
	}
	public override void SetDefaults(Item item)
	{
		if (item.type == Mod.Find<ModItem>("GoldGarItem").Type)
			item.value = Item.sellPrice(0, 10, 0, 0);

		if (item.type == Mod.Find<ModItem>("KillifishItem").Type)
			item.value = Item.sellPrice(0, 10, 0, 0);

		if (item.type == Mod.Find<ModItem>("KillifishBannerItem").Type)
			item.value = Item.sellPrice(0, 0, 2, 0);

		if (item.type == Mod.Find<ModItem>("GarBannerItem").Type)
			item.value = Item.sellPrice(0, 0, 2, 0);

		if (item.type == Mod.Find<ModItem>("GarItem").Type)
			item.value = Item.sellPrice(0, 0, 5, 37);

		if (item.type == Mod.Find<ModItem>("KillifishItem").Type)
			item.value = Item.sellPrice(0, 0, 3, 29);

		if (item.type == Mod.Find<ModItem>("TermiteItem").Type)
		{
			item.value = Item.sellPrice(0, 0, 0, 95);
			item.bait = 9;
		}
	}
	public override void AddRecipes()
	{
		Recipe recipe = Recipe.Create(ItemID.HunterPotion, 1);
		recipe.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.Blinkroot)
			.AddIngredient(Mod.Find<ModItem>("GarItem").Type)
			.AddTile(TileID.Bottles)
			.Register();
	}
}
