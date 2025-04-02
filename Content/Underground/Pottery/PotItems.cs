using SpiritReforged.Content.Underground.Tiles;

namespace SpiritReforged.Content.Underground.Pottery;

/// <summary> Specialised pot item autoloader (<see cref="AutoloadedPotItem"/>) for <see cref="PotteryWheel"/>. </summary>
public class PotItems : ILoadable
{
	/// <summary> Sorts style by index. </summary>
	private static readonly string[] Names = ["Cavern", "Gold", "Ice", "Desert", "Jungle", "Dungeon", "Corrupt", "Crimson", "Marble", "Hell"]; 

	public void Load(Mod mod)
	{
		for (int i = 0; i < Names.Length; i++)
		{
			string name = Names[i];
			mod.AddContent(new AutoloadedPotItem(nameof(BiomePots), name, i * 3));
		}
	}

	public void Unload() { }
}

public sealed class AutoloadedPotItem(string baseName, string name, int style) : ModItem
{
	protected override bool CloneNewInstances => true;
	public override string Name => _name + "PotItem";
	public override string Texture => (GetType().Namespace + $".{_name}Pot").Replace('.', '/');

	private string _baseName = baseName;
	private string _name = name;
	private int _style = style;

	private ModTile Tile => Mod.Find<ModTile>(_baseName + "Echo");

	public override ModItem Clone(Item newEntity)
	{
		var item = base.Clone(newEntity) as AutoloadedPotItem;
		item._baseName = _baseName;
		item._name = _name;
		item._style = _style;
		return item;
	}

	public override void SetStaticDefaults() //Register echo tile drops for all styles
	{
		int wrap = TileObjectData.GetTileData(Tile.Type, 0)?.StyleWrapLimit ?? 0;
		List<int> styles = [];

		for (int i = 0; i < wrap; i++)
			styles.Add(_style + i);
		
		if (styles.Count != 0)
			Tile.RegisterItemDrop(Type, [.. styles]);
	}

	public override void SetDefaults() => Item.DefaultToPlaceableTile(Tile.Type, _style);
	public override void AddRecipes() => CreateRecipe().AddRecipeGroup("ClayAndMud", 5)
		.AddTile(ModContent.TileType<PotteryWheel>()).Register();
}