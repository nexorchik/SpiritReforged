namespace SpiritReforged.Content.Ocean.Items.Reefhunter;

public class IridescentScale : ModItem
{
	private int subID = -1; //Controls the in-world sprite for this item
	private static Asset<Texture2D> WorldTexture;

	public override void SetStaticDefaults()
	{
		if (!Main.dedServ)
			WorldTexture = ModContent.Request<Texture2D>(Texture + "_World");
	}

	public override void SetDefaults()
	{
		Item.value = 100;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Blue;
		Item.width = 26;
		Item.height = 28;
	}

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		if (subID != -1)
			Item.height = subID switch
			{
				0 => 20,
				1 => 32,
				_ => 28
			};
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (subID == -1)
			subID = Main.rand.Next(3);

		var texture = WorldTexture.Value;
		spriteBatch.Draw(texture, Item.position - Main.screenPosition, new Rectangle(0, 34 * subID, 26, 32), GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		subID = -1;
		return true;
	}
}
