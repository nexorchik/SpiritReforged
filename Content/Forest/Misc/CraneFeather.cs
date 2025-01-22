namespace SpiritReforged.Content.Forest.Misc;

public class CraneFeather : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 28;
		Item.value = Item.sellPrice(gold: 2);
		Item.rare = ItemRarityID.Green;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (player.velocity.Y != 0 && player.wings <= 0 && !player.mount.Active)
		{
			player.runAcceleration *= 2f;
			player.maxRunSpeed *= 1.5f;
		}
	}
}
