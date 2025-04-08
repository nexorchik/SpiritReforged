using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Underground.Tiles;

/// <summary> A stand-in for vanilla pot tiles, used to contain custom data. </summary>
public class Pots : PotTile, ILootTile
{
	public const string PotTexture = "Terraria/Images/Tiles_28";

	public override string Texture => PotTexture;
	public override Dictionary<string, int[]> TileStyles
	{
		get
		{
			string[] names = ["Cavern", "Ice", "Jungle", "Dungeon", "Hell", "Corruption", "Spider", "Crimson", "Pyramid", "Temple", "Marble", "Desert"];
			Dictionary<string, int[]> groups = [];

			groups.Add(names[0], [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]);
			const int length = 9;

			for (int i = 1; i < names.Length; i++)
			{
				int skip = i * length + 3;
				int[] styles = [skip, skip + 1, skip + 2, skip + 3, skip + 4, skip + 5, skip + 6, skip + 7, skip + 8];

				groups.Add(names[i], styles);
			}

			return groups;
		}
	}

	public LootTable AddLoot(int objectStyle)
	{
		string styleName = StyleDatabase.GetName(Type, (byte)objectStyle);
		var loot = new LootTable();

		if (styleName == "PotsDungeon")
			loot.AddCommon(ItemID.GoldenKey, 35);

		List<int> potions = [ItemID.IronskinPotion, ItemID.ShinePotion, ItemID.NightOwlPotion, ItemID.SwiftnessPotion,
			ItemID.MiningPotion, ItemID.CalmingPotion, ItemID.BuilderPotion, ItemID.RecallPotion, ItemID.ArcheryPotion,
			ItemID.GillsPotion, ItemID.HunterPotion, ItemID.TrapsightPotion, ItemID.FeatherfallPotion, ItemID.WaterWalkingPotion,
			ItemID.GravitationPotion, ItemID.InvisibilityPotion, ItemID.ThornsPotion, ItemID.HeartreachPotion, ItemID.FlipperPotion,
			ItemID.ManaRegenerationPotion, ItemID.ObsidianSkinPotion, ItemID.MagicPowerPotion, ItemID.BattlePotion, ItemID.TitanPotion];

		var pCond0 = ItemDropRule.OneFromOptions(13, [.. potions]);

		if (styleName == "PotsHell")
			pCond0.OnSuccess(ItemDropRule.Common(ItemID.PotionOfReturn, 5));

		loot.Add(pCond0);
		loot.Add(ItemDropRule.ByCondition(new DropConditions.Standard(Condition.Multiplayer), ItemID.WormholePotion, 30));

		const int branchSize = 5;

		if (styleName == "PotsIce")
			loot.AddCommon(ItemID.IceTorch, branchSize, 3, 12);
		else
			loot.AddCommon(TorchType(), branchSize, 5, 18);

		if (styleName == "PotsHell")
			loot.AddCommon(ItemID.HellfireArrow, branchSize, 10, 20);
		else if (Main.hardMode)
			loot.AddOneFromOptions(1, ItemID.UnholyArrow, ItemID.Grenade, (WorldGen.SavedOreTiers.Silver == TileID.Silver) ? ItemID.SilverBullet : ItemID.TungstenBullet);
		else
		{
			loot.AddCommon(ItemID.WoodenArrow, branchSize * 2, 10, 20);
			loot.AddCommon(ItemID.Shuriken, branchSize * 2, 10, 20);
		}

		loot.AddCommon(Main.hardMode ? ItemID.HealingPotion : ItemID.LesserHealingPotion, branchSize);
		loot.AddCommon((styleName == "PotsDesert") ? ItemID.ScarabBomb : ItemID.Bomb, branchSize, 1, 4);

		if (!Main.hardMode)
			loot.AddCommon(ItemID.Rope, branchSize, 20, 40);

		return loot;

		int TorchType()
		{
			int result = styleName switch
			{
				"PotCorruption" => ItemID.CorruptTorch,
				"PotCrimson" => ItemID.CrimsonTorch,
				"PotJungle" => ItemID.JungleTorch,
				"PotDesert" => ItemID.DesertTorch,
				_ => ItemID.Torch
			};

			return result;
		}
	}
}