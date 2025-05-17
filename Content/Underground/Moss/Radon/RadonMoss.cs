using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;
using SpiritReforged.Content.Underground.Moss.Oganesson;

namespace SpiritReforged.Content.Underground.Moss.Radon;

[AutoloadGlowmask("224,232,70")]
public class RadonMoss : OganessonMoss
{
	public override void SetEntry()
	{
		RegisterItemDrop(ModContent.ItemType<RadonMossItem>());
		AddMapEntry(new Color(252, 248, 3));

		DustType = ModContent.DustType<RadonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<RadonMossGrayBrick>(), 1, TileID.GrayBrick) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to gray bricks

		GrowTiles(i, j);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ItemID.StoneBlock);
	}

	protected override void GrowTiles(int i, int j) => Placer.PlacePlant<RadonPlants>(i, j, Main.rand.Next(RadonPlants.StyleRange));
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.234f, 0.153f, 0.03f);
}