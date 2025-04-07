using RubbleAutoloader;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.PresetTiles;

/// <summary> Helper for building pot tiles automatically registered in the Potstiary. </summary>
public abstract class PotTile : ModTile, IRecordTile, IAutoloadRubble
{
	public IAutoloadRubble.RubbleData Data => default; //Effectively creates no connection with the Rubblemaker item
	public abstract Dictionary<string, int[]> TileStyles { get; }
	public Dictionary<string, int[]> Styles
	{
		get
		{
			if (Autoloader.IsRubble(Type))
				return [];
			else
				return TileStyles;
		}
	}

	/// <inheritdoc cref="ModType.Load"/>
	public virtual void Load(Mod mod) { }
	public sealed override void Load()
	{
		if (Name.Contains("Rubble")) //Autoloader.IsRubble is unusuable before before loading is complete
			return;

		StyleDatabase.OnPopulateStyleGroups += AutoloadFromGroup;
		Load(Mod);
	}

	public virtual void AddRecord(int type, StyleDatabase.StyleGroup group) => RecordHandler.Records.Add(new TileRecord(group.name, type, group.styles));
	public virtual void AutoloadFromGroup()
	{
		foreach (var c in StyleDatabase.Groups[Type])
			Mod.AddContent(new AutoloadedPotItem(Name + "Rubble", c.name, c.styles[0], c.styles.Length));
	}

	/// <summary> <inheritdoc cref="ModType.SetStaticDefaults"/><para/>
	/// Automatically sets common pot data by type. See <see cref="AddObjectData"/>.
	/// </summary>
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileCut[Type] = !Autoloader.IsRubble(Type);
		Main.tileFrameImportant[Type] = true;
		Main.tileSpelunker[Type] = true;

		AddObjectData();

		AddMapEntry(new Color(100, 90, 35), Language.GetText("MapObject.Pot"));
		DustType = -1;
	}

	/// <summary> Adds object data for this pot. By default, assumes <see cref="TileObjectData.Style2x2"/> with <see cref="TileObjectData.StyleWrapLimit"/> of 3. </summary>
	public virtual void AddObjectData()
	{
		const int row = 3;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new(0, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.StyleWrapLimit = row;
		TileObjectData.newTile.RandomStyleRange = row;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);
	}
}