using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;

namespace SpiritReforged.Content.Underground.Moss.Oganesson;

[AutoloadGlowmask("255,255,255")]
public class OganessonMoss : GrassTile
{
	protected override int DirtType => TileID.Stone;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		Main.tileLighted[Type] = true;
		Main.tileMoss[Type] = true;

		RegisterItemDrop(ModContent.ItemType<OganessonMossItem>());
		AddMapEntry(new Color(220, 220, 220));
		this.Merge(TileID.Stone, TileID.GrayBrick);

		DustType = ModContent.DustType<OganessonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<OganessonMossGrayBrick>(), 1, TileID.GrayBrick) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to gray bricks

		GrowTiles(i, j);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ItemID.StoneBlock); //Drop stone blocks in every normal circumstance despite having a different type registered
	}

	protected virtual void GrowTiles(int i, int j) => TileExtensions.PlacePlant<OganessonPlants>(i, j, Main.rand.Next(OganessonPlants.StyleRange));
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.15f, 0.15f, 0.15f);
}