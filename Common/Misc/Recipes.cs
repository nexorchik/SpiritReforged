using SpiritReforged.Content.Ocean.Items.Driftwood;

namespace SpiritReforged.Common.Misc;

internal class Recipes : ModSystem
{
	public override void AddRecipeGroups()
	{
		RecipeGroup woodGrp = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Wood"]];
		woodGrp.ValidItems.Add(ModContent.ItemType<DriftwoodTileItem>());

		RecipeGroup BaseGroup(object GroupName, int[] Items)
		{
			string Name = "";
			Name += GroupName switch
			{
				//modcontent items
				int i => Lang.GetItemNameValue((int)GroupName),
				//vanilla item ids
				short s => Lang.GetItemNameValue((short)GroupName),
				//custom group names
				_ => GroupName.ToString(),
			};
			return new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Name, Items);
		}

		RecipeGroup.RegisterGroup("CopperBars", BaseGroup(ItemID.CopperBar, [ItemID.CopperBar, ItemID.TinBar]));
		RecipeGroup.RegisterGroup("SilverBars", BaseGroup(ItemID.SilverBar, [ItemID.SilverBar, ItemID.TungstenBar]));
		RecipeGroup.RegisterGroup("GoldBars", BaseGroup(ItemID.GoldBar, [ItemID.GoldBar, ItemID.PlatinumBar]));
		RecipeGroup.RegisterGroup("Tier3HMBar", BaseGroup(ItemID.AdamantiteBar, [ItemID.AdamantiteBar, ItemID.TitaniumBar]));
		RecipeGroup.RegisterGroup("PHMEvilMaterial", BaseGroup(ItemID.ShadowScale, [ItemID.ShadowScale, ItemID.TissueSample]));
		RecipeGroup.RegisterGroup("EvilMaterial", BaseGroup(ItemID.CursedFlame, [ItemID.CursedFlame, ItemID.Ichor]));
	}
}
