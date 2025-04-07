using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Underground.Tiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Pottery;

public class BiomePotsEcho : BiomePots
{
	public override string Texture => (typeof(BiomePots).Namespace + "." + typeof(BiomePots).Name).Replace('.', '/');
	public override Dictionary<string, int[]> Styles => [];

	public override void Load() => StyleDatabase.OnPopulateStyleGroups += AutoloadFromGroup;
	private void AutoloadFromGroup()
	{
		foreach (var c in StyleDatabase.Groups[ModContent.TileType<BiomePots>()])
			Mod.AddContent(new AutoloadedPotItem(nameof(BiomePotsEcho), c.name, c.styles[0]));
	}

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.tileCut[Type] = false;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY) { }
	public override void NearbyEffects(int i, int j, bool closer) { }
	public override bool KillSound(int i, int j, bool fail) => true;
}

public class MushroomPotsEcho : MushroomPots
{
	public override string Texture => (typeof(MushroomPots).Namespace + "." + typeof(MushroomPots).Name).Replace('.', '/');
	public override Dictionary<string, int[]> Styles => [];

	public override void Load() => StyleDatabase.OnPopulateStyleGroups += AutoloadFromGroup;
	private void AutoloadFromGroup()
	{
		foreach (var c in StyleDatabase.Groups[ModContent.TileType<MushroomPots>()])
			Mod.AddContent(new AutoloadedPotItem(nameof(MushroomPotsEcho), c.name, c.styles[0]));
	}

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.tileCut[Type] = false;
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) { }
	public override bool KillSound(int i, int j, bool fail) => true;
}

public class CommonPotsEcho : ModTile
{
	public override string Texture => StackablePots.PotTexture;

	public override void Load() => StyleDatabase.OnPopulateStyleGroups += AutoloadFromGroup;
	private void AutoloadFromGroup()
	{
		foreach (var c in StyleDatabase.Groups[TileID.Pots])
			Mod.AddContent(new AutoloadedPotItem(nameof(CommonPotsEcho), c.name, c.styles[0]));
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