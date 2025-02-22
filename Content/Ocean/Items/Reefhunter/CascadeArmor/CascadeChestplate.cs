namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

[AutoloadEquip(EquipType.Body)]
public class CascadeChestplate : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 26;
		Item.value = 5600;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 4;
	}
}
