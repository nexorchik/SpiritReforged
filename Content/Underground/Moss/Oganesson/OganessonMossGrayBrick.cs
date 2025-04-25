using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;

namespace SpiritReforged.Content.Underground.Moss.Oganesson;

[AutoloadGlowmask("255,255,255")]
public class OganessonMossGrayBrick : GrassTile
{
	protected override int DirtType => TileID.GrayBrick;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		Main.tileLighted[Type] = true;

		RegisterItemDrop(ItemID.GrayBrick);
		AddMapEntry(new Color(220, 220, 220));
		this.Merge(TileID.Stone, TileID.GrayBrick);

		DustType = ModContent.DustType<OganessonMossDust>();
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<OganessonMoss>(), 1, TileID.Stone) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to stone

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j) => TileExtensions.PlacePlant<OganessonPlants>(i, j, Main.rand.Next(OganessonPlants.StyleRange));
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.3f, 0.3f, 0.3f);
}