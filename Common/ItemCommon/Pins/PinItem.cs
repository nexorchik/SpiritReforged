using Terraria.Audio;

namespace SpiritReforged.Common.ItemCommon.Pins;

/// <summary>
/// Abstract class for a map pin.
/// Contains all the code needed for a map pin item to place, move, or remove map pins.
/// ColorName is the most important: it defines which map pin the item is associated with
/// </summary>
public abstract class PinItem : ModItem
{
	/// <summary>
	/// The name of this pin's color (capitalized, single word, please)
	/// Affects both the visible name and where it expects the item/map texture to be
	/// </summary>
	public abstract string PinName { get; }

	/// <summary>
	/// The color of this map pin's placement text
	/// </summary>
	public abstract Color TextColor { get; }

	/// <summary>
	/// How the pin's name appears in the tooltip description (formats to lowercase by default)
	/// </summary>
	public virtual string DescName => PinName.ToLower();

	public override string Texture => base.Texture + "Item";

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 1;

		PinMapLayer.Textures ??= [];
		PinMapLayer.Textures.Add(PinName, ModContent.Request<Texture2D>(base.Texture + "Map"));
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.useTime = 25;
		Item.useAnimation = 25;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.noMelee = true;
		Item.knockBack = 5;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Green;
		Item.autoReuse = false;
		Item.shootSpeed = 0f;
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool? UseItem(Player player)
	{
		SoundEngine.PlaySound(SoundID.Dig, player.Center);
		string text;

		if (player.altFunctionUse != 2)
		{
			text = Language.GetTextValue("Mods.SpiritReforged.Misc.Pins.Pinned");
			PinSystem.Place(PinName, player.Center / 16);
		}
		else
		{
			text = Language.GetTextValue("Mods.SpiritReforged.Misc.Pins.Unpinned");
			PinSystem.Remove(PinName);
		}

		CombatText.NewText(
			new Rectangle((int)player.position.X, (int)player.position.Y - 10, player.width, player.height),
			TextColor,
			text
		);

		return true;
	}
}