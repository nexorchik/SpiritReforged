using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.Botanist.Items;

[AutoloadEquip(EquipType.Body, EquipType.Waist)]
public class BotanistBody : ModItem, IFrameEffects
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 20;
		Item.value = Item.sellPrice(0, 0, 10, 0);
		Item.rare = ItemRarityID.White;
		Item.defense = 1;
	}

	public override void AddRecipes() => CreateRecipe()
			.AddIngredient(ItemID.Silk, 10)
			.AddTile(TileID.Loom)
			.Register();

	public void FrameEffects(Player player)
	{
		var bodyVanitySlot = player.armor[11];

		if (bodyVanitySlot.IsAir || bodyVanitySlot.type == Type)
			player.waist = EquipLoader.GetEquipSlot(Mod, nameof(BotanistBody), EquipType.Waist);
	}
}