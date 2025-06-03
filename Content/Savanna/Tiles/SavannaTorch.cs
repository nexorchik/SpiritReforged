using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Content.Savanna.Biome;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaTorch : TorchTile
{
	public override float GetTorchLuck(Player player)
	{
		float value = -0.5f;

		if (player.InModBiome<SavannaBiome>())
			value = 1f;
		else if (player.ZoneDesert || player.ZoneJungle)
			value = 0.5f;
		else if (player.ZoneSnow)
			value = -1f;

		return value;
	}
}

public class SavannaTorchItem : ModItem
{
	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 100;

		ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
		ItemID.Sets.SingleUseInGamepad[Type] = true;
		ItemID.Sets.Torches[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DefaultToTorch(ModContent.TileType<SavannaTorch>(), 0);
		Item.value = 50;
	}

	public override void HoldItem(Player player)
	{
		if (player.wet)
			return;

		if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30))
		{
			var d = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 14f * player.gravDir), 4, 4, DustID.Torch, 0f, 0f, 100);
			if (!Main.rand.NextBool(3))
				d.noGravity = true;

			d.velocity *= 0.3f;
			d.velocity.Y -= 1.5f;
			d.position = player.RotatedRelativePoint(d.position);
		}

		Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
		Lighting.AddLight(position, 1f, 1f, 1f);
	}

	public override void PostUpdate()
	{
		if (!Item.wet)
			Lighting.AddLight(Item.Center, 1f, 1f, 1f);
	}

	public override void AddRecipes() => CreateRecipe(3).AddIngredient(ItemID.Gel).AddIngredient(ItemMethods.AutoItemType<Drywood>()).Register();
}