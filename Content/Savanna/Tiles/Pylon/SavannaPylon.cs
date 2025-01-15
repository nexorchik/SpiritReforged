using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Content.Savanna.Biome;

namespace SpiritReforged.Content.Savanna.Tiles.Pylon;

public class SavannaPylon : PylonTile
{
	public override void SetStaticDefaults(LocalizedText mapEntry) => AddMapEntry(Color.LightGoldenrodYellow, mapEntry);
	public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => SavannaTileCounts.InSavanna;
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (.8f, .72f, .4f);

	public override NPCShop.Entry GetNPCShopEntry()
	{
		var biomeCondition = new Condition("In Savanna", () => Main.LocalPlayer.InModBiome<SavannaBiome>());
		return new NPCShop.Entry(ModItem.Type, Condition.HappyEnoughToSellPylons, biomeCondition);
	}

	public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var color = Color.LightGoldenrodYellow;
		DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), color * .1f, color, 6, frameCount);
	}
}