using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Underground.Tiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Pottery;

/// <summary> Placeable variants of vanilla pot tiles. </summary>
public class CommonPotsEcho : ModTile
{
	public override string Texture => StackablePots.PotTexture;

	public override void Load() => StyleDatabase.OnPopulateStyleGroups += AutoloadFromGroup;
	private void AutoloadFromGroup()
	{
		foreach (var c in StyleDatabase.Groups[TileID.Pots])
			Mod.AddContent(new AutoloadedPotItem(Name, c.name, c.styles[0], c.styles.Length));
	}

	public override void SetStaticDefaults()
	{
		const int row = 3;

		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileSpelunker[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new(0, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.StyleWrapLimit = row;
		TileObjectData.newTile.RandomStyleRange = row * 3;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 90, 35), Language.GetText(StackablePots.NameKey));
		DustType = -1;
	}
}