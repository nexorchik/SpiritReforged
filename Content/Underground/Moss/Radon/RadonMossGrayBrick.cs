using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;

namespace SpiritReforged.Content.Underground.Moss.Radon;

[AutoloadGlowmask("224,232,70")]
public class RadonMossGrayBrick : GrassTile
{
	protected override int DirtType => TileID.GrayBrick;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		Main.tileLighted[Type] = true;
		Main.tileMoss[Type] = true;

		RegisterItemDrop(ItemID.GrayBrick);
		AddMapEntry(new Color(252, 248, 3));
		this.Merge(TileID.Stone, TileID.GrayBrick);
		
		HitSound = SoundID.Grass;
		DustType = ModContent.DustType<RadonMossDust>();
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 1, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread moss

		if (SpreadHelper.Spread(i, j, ModContent.TileType<RadonMoss>(), 1, TileID.Stone) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Also spread to stone

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j) => Placer.PlacePlant<RadonPlants>(i, j, Main.rand.Next(RadonPlants.StyleRange));
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.234f, 0.153f, 0.03f);
}