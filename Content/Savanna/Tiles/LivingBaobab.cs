using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Savanna.Tiles;

internal class LivingBaobab : ModTile
{
	public override void SetStaticDefaults()
	{
		TileID.Sets.IsSkippedForNPCSpawningGroundTypeCheck[Type] = true;

		Main.tileSolid[Type] = true;
		Main.tileBlendAll[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;

		RegisterItemDrop(ItemMethods.AutoItemType<Drywood>());
		AddMapEntry(new Color(142, 125, 106));
		HitSound = SoundID.Dig;
		DustType = DustID.t_PearlWood;
	}
}
