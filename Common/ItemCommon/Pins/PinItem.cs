using Terraria.Audio;

namespace SpiritReforged.Common.ItemCommon.Pins;

/// <summary> Abstract class for a map pin. Contains all the code needed for a map pin item to place, move, or remove map pins.  </summary>
public abstract class PinItem : ModItem
{
	/// <summary> The color of this map pin's placement text. </summary> //Currently unused
	public abstract Color TextColor { get; }

	public override string Texture => base.Texture + "Item";

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 1;

		PinMapLayer.Textures ??= [];
		PinMapLayer.Textures.Add(Name, ModContent.Request<Texture2D>(base.Texture + "Map"));
		PinSystem.ItemByName.Add(Name, Item);
	}

	public override void SetDefaults()
	{
		Item.width = Item.height = 32;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Green;
	}

	public override bool CanRightClick() => !Main.LocalPlayer.PinUnlocked(Name);

	public override void RightClick(Player player)
	{
		player.UnlockPin(Name);
		SoundEngine.PlaySound(SoundID.Grab, player.Center);
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		int index = tooltips.FindIndex(tooltip => tooltip.Name.Contains("Tooltip"));
		if (index == -1) //Insert in the tooltip line, above sell value
			return;

		if (Main.LocalPlayer.PinUnlocked(Name))
			tooltips.Insert(index, new TooltipLine(Mod, "Obtained", Language.GetTextValue("Mods.SpiritReforged.Misc.Pins.Obtained")) { OverrideColor = Color.Orange });
		else
			tooltips.Insert(index, new TooltipLine(Mod, "UseItem", Language.GetTextValue("Mods.SpiritReforged.Misc.Pins.UseItem")));
	}
}