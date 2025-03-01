using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Savanna.Items.Vanity;

[AutoloadEquip(EquipType.Legs)]
public class OstrichPants : ModItem, IFrameEffects
{
	public const string WaistEquip = "OstrichPantsWaist";

	public override void Load() => EquipLoader.AddEquipTexture(Mod, Texture + "_Legs", EquipType.Waist, name: WaistEquip);

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.vanity = true;
	}

	//Despite being a legs equip, we need to use the waist layer due to draw order.
	public void FrameEffects(Player player) => player.waist = EquipLoader.GetEquipSlot(Mod, WaistEquip, EquipType.Waist);
}