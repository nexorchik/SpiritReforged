using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Particles;

namespace SpiritReforged.Content.Ocean.Items;

public class MineralSlag : ModItem
{
	protected const int frameCount = 5;
	protected int subID = -1; //Controls the in-world sprite for this item

	public override void SetStaticDefaults() 
		=> Main.RegisterItemAnimation(Type, new Terraria.DataStructures.DrawAnimationVertical(2, frameCount) { NotActuallyAnimating = true });

	public override void SetDefaults()
	{
		Item.Size = new Vector2(20);
		Item.value = 100;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Blue;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (subID == -1)
			subID = Main.rand.Next(frameCount);

		var texture = TextureAssets.Item[Type].Value;
		var source = texture.Frame(1, frameCount, 0, subID, 0, -2);

		spriteBatch.Draw(texture, Item.position - Main.screenPosition, source, GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		subID = -1;
		return true;
	}
}

public class MineralSlagPickup : MineralSlag //Spawned strictly by Hydrothermal Vents
{
	private const int timeLeftMax = 60 * 60 * 2;
	private int timeLeft = timeLeftMax;
	private bool collided = false;

	public override string Name => "Mods.SpiritReforged.Items.MineralSlag.DisplayName";

	public override string Texture => base.Texture.Replace("Pickup", string.Empty);

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		if (timeLeft > timeLeftMax - 60 * 5) //Draw glow
		{
			float duration = 60f * 5;
			float intensity = (timeLeft - (timeLeftMax - duration)) / duration;

			var texture = TextureAssets.Item[Type].Value;
			var source = texture.Frame(1, frameCount, 0, subID, 0, -2);

			Main.EntitySpriteDraw(AssetLoader.LoadedTextures["Bloom"], Item.Center - Main.screenPosition, null,
				(Color.Yellow with { A = 0 }) * .35f * intensity, rotation, AssetLoader.LoadedTextures["Bloom"].Size() / 2, Item.scale * .25f, SpriteEffects.None);

			for (int i = 0; i < 3; i++)
				Main.EntitySpriteDraw(texture, Item.Center - Main.screenPosition, source,
					Color.Orange with { A = 0 } * intensity, rotation, source.Size() / 2, Item.scale + .1f * i * intensity, SpriteEffects.None);
		}
	}

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		if (--timeLeft <= 0)
			Item.active = false;

		if (!collided)
		{
			if (Item.velocity == Vector2.Zero)
			{
				Item.velocity = Vector2.UnitY * -1f;
				for (int i = 0; i < 10; i++)
				{
					var color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat());
					float magnitude = Main.rand.NextFloat();

					ParticleHandler.SpawnParticle(new GlowParticle(Item.Bottom, Vector2.UnitY * -magnitude,
						color, (1f - magnitude) * .5f, Main.rand.Next(30, 120), 5, extraUpdateAction: delegate (Particle p)
						{
							p.Velocity = p.Velocity.RotatedBy(Main.rand.NextFloat(-.1f, .1f));
						}));
				}

				collided = true;
			}

			if (Main.rand.NextBool(3))
			{
				int type = Main.rand.NextBool() ? DustID.Torch : DustID.Ash;
				var dust = Dust.NewDustDirect(Item.position, Item.width, Item.height, type, 0, 0, 0, default, Main.rand.NextFloat(.5f, 2f));
				dust.noGravity = true;
				dust.velocity = -Item.velocity * Main.rand.NextFloat(.5f);

				if (type == DustID.Ash)
					dust.alpha = 150;
			}
		}
	}

	public override bool OnPickup(Player player)
	{
		player.QuickSpawnItem(Item.GetSource_FromThis(), ModContent.ItemType<MineralSlag>());
		return false;
	}
}
