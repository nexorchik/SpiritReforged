using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Vanilla.Items.Food;

public class RawMeat : FoodItem
{
	internal override Point Size => new(30, 26);
	private int subID = -1;

	public override bool CanUseItem(Player player)
	{
		player.AddBuff(BuffID.Poisoned, 45 * 60);
		return true;
	}
	public override void Defaults()
	{
		Item.buffTime = 2 * 60 * 60;
	}
	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		if (subID != -1)
			Item.height = subID switch
			{
				0 => 22,
				1 => 22,
				_ => 26
			};
	}
	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (subID == -1)
			subID = Main.rand.Next(3);

		Texture2D tex = ModContent.Request<Texture2D>(Texture + "_World").Value;
		spriteBatch.Draw(tex, Item.position - Main.screenPosition, new Rectangle(0, 28 * subID, 30, 26), GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		subID = -1;
		return true;
	}
}

