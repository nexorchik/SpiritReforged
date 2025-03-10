namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

[AutoloadEquip(EquipType.Body)]
public class CascadeChestplate : ModItem
{
	internal static int Slot { get; private set; }

	public override void SetStaticDefaults() => Slot = EquipLoader.GetEquipSlot(Mod, nameof(CascadeChestplate), EquipType.Body);

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 26;
		Item.value = 5600;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 4;
	}
}
