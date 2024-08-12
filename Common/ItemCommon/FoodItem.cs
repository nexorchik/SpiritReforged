using Terraria.DataStructures;

namespace SpiritReforged.Common.ItemCommon;

public abstract class FoodItem : ModItem
{
	internal abstract Point Size { get; }
	internal virtual int Rarity => ItemRarityID.Blue;
	internal virtual bool Consumeable => true;

	public sealed override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 5;

		ItemID.Sets.IsFood[Type] = true;
		Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, 3) { NotActuallyAnimating = true });

		StaticDefaults();
	}

	public override sealed void SetDefaults()
	{
		Item.width = Size.X;
		Item.height = Size.Y;
		Item.rare = Rarity;
		Item.maxStack = Item.CommonMaxStack;
		Item.value = Item.sellPrice(0, 0, 0, 50);
		Item.noUseGraphic = false;
		Item.useStyle = ItemUseStyleID.EatFood;
		Item.useTime = Item.useAnimation = 20;
		Item.noMelee = true;
		Item.consumable = Consumeable;
		Item.autoReuse = false;
		Item.UseSound = SoundID.Item2;
		Item.buffTime = 5 * 60 * 60;
		Item.buffType = BuffID.WellFed;

		Defaults();
	}

	public virtual void StaticDefaults() { }
	public virtual void Defaults() { }

	public override bool PreDrawInWorld(SpriteBatch sb, Color light, Color a, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D tex = TextureAssets.Item[Type].Value;
		sb.Draw(tex, Item.Center - Main.screenPosition, new Rectangle(0, 0, Item.width, Item.height), light, rotation, Item.Size / 2f, scale, SpriteEffects.None, 0f);
		return false;
	}
}
