using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;

namespace SpiritReforged.Content.Underground.Moss.Radon;

[AutoloadGlowmask("224,232,70")]
public class RadonMoss : GrassTile
{
	protected override int DirtType => TileID.Stone;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		Main.tileLighted[Type] = true;

		RegisterItemDrop(ModContent.ItemType<RadonMossItem>());
		AddMapEntry(new Color(252, 248, 3));
		this.Merge(TileID.Stone, TileID.GrayBrick);

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

	protected virtual void GrowTiles(int i, int j) => TileExtensions.PlacePlant<RadonPlants>(i, j, Main.rand.Next(RadonPlants.StyleRange));
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.338f, 0.219f, 0.04f);
}