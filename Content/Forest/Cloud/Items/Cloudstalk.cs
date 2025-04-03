using SpiritReforged.Common.ModCompat.Classic;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.TileCommon.TileTag;
using SpiritReforged.Content.Jungle.Bamboo.Tiles;

namespace SpiritReforged.Content.Forest.Cloud.Items;

[FromClassic("CloudstalkItem")]
public class Cloudstalk : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 25;

	public override void SetDefaults()
	{
		Item.autoReuse = false;
		Item.useTurn = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = Item.useTime = 15;
		Item.maxStack = Item.CommonMaxStack;
		Item.width = 22;
		Item.height = 18;
	}

	public override void AddRecipes()
	{
		Recipe.Create(ItemID.FeatherfallPotion, 1).AddIngredient(ItemID.Blinkroot).AddIngredient(ItemID.Daybloom).
			AddIngredient(ItemID.BottledWater).AddIngredient(Type).AddTile(TileID.Bottles).Register();

		Recipe.Create(ItemID.GravitationPotion).AddIngredient(ItemID.Blinkroot).AddIngredient(ItemID.Fireblossom).
			AddIngredient(ItemID.Deathweed).AddIngredient(ItemID.BottledWater).AddIngredient(Type).AddTile(TileID.Bottles).Register();
	}
}

[TileTag(TileTags.HarvestableHerb)]
public class CloudstalkTile : HerbTile
{
	private const float BloomWindSpeed = 20; //Constant for bloom wind speed, in mph

	public override void StaticDefaults()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Grass, TileID.HallowedGrass, TileID.JungleGrass, ModContent.TileType<Savanna.Tiles.SavannaGrass>(), 
			ModContent.TileType<Savanna.Tiles.SavannaGrassHallow>(), TileID.Cloud, TileID.RainCloud, TileID.SnowCloud];
		TileObjectData.newTile.AnchorAlternateTiles = [TileID.ClayPot, TileID.PlanterBox, ModContent.TileType<Tiles.CloudstalkBox>(), ModContent.TileType<BambooPot>()];
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(178, 234, 234), name);

		DustType = DustID.Cloud;
	}

	public override IEnumerable<Item> ItemDrops(int i, int j, int herbStack, int seedStack)
	{
		if (herbStack > 0)
			yield return new Item(ModContent.ItemType<Cloudstalk>()) { stack = herbStack };
		if (seedStack > 0)
			yield return new Item(ModContent.ItemType<CloudstalkSeed>()) { stack = seedStack };
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		float windSpeed = Math.Abs(Main.windSpeedCurrent) * 50;
		if (windSpeed > BloomWindSpeed && GetStage(i, j) == PlantStage.Growing)
			SetStage(i, j, PlantStage.Grown);
		else if (windSpeed <= BloomWindSpeed && GetStage(i, j) == PlantStage.Grown)
			SetStage(i, j, PlantStage.Growing);
	}
}