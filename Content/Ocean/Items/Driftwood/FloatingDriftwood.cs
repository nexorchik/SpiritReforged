using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Ocean.Items.Driftwood;

//The naturally spawning variant of our driftwood decoration items and the player's source of driftwood blocks
public class FloatingDriftwood : FloatingItem
{
	private int subID = -1; //Controls the in-world sprite for this item

	public override float Weight => base.Weight * 0.9f;
	public override float Bouyancy => base.Bouyancy * 1.05f;

	public override string Texture => base.Texture.Replace("Floating", string.Empty);

	public override void SetStaticDefaults() => Main.RegisterItemAnimation(Type, new Terraria.DataStructures.DrawAnimationVertical(2, 3) { NotActuallyAnimating = true });

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 18;
		Item.rare = ItemRarityID.White;
		Item.maxStack = 1;
	}

	public override bool OnPickup(Player player)
	{
		int stack = subID switch
		{
			1 => 20,
			2 => 25,
			_ => 10
		};

		player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "Pickup"), ModContent.ItemType<DriftwoodTileItem>(), stack);
		return false;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (subID == -1)
			subID = Main.rand.Next(3);

		Texture2D tex = TextureAssets.Item[Type].Value;
		spriteBatch.Draw(tex, Item.position - Main.screenPosition, new Rectangle(0, 22 * subID, 36, 22), GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		subID = -1;
		return true;
	}
}
