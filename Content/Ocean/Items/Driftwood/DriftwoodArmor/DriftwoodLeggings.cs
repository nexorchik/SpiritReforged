namespace SpiritReforged.Content.Ocean.Items.Driftwood.DriftwoodArmor;

[AutoloadEquip(EquipType.Legs)]
public class DriftwoodLeggings : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 26;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.defense = 1;
	}
}
