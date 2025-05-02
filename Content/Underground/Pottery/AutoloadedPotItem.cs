namespace SpiritReforged.Content.Underground.Pottery;

/// <summary> Specialised pot item template for <see cref="PotteryWheel"/>. </summary>
/// <param name="baseName"> The internal name of the tile this item places. </param>
/// <param name="name"></param>
/// <param name="style"> The base style this item places implicitly affected by RandomStyleRange. </param>
/// <param name="styleLimit"> The length of styles this item can place, used for calculating drops. </param>
public sealed class AutoloadedPotItem(string baseName, string name, int style, int styleLimit = 3) : ModItem
{
	protected override bool CloneNewInstances => true;
	public override string Name => _name + "Item";
	public override string Texture => (GetType().Namespace + $".{_name}").Replace('.', '/');

	private readonly int _styleLimit = styleLimit;

	private string _baseName = baseName;
	private string _name = name;
	private int _style = style;

	private ModTile Tile => Mod.Find<ModTile>(_baseName);

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
		List<int> styles = [];

		for (int i = 0; i < _styleLimit; i++)
			styles.Add(_style + i);
		
		if (styles.Count != 0)
			Tile.RegisterItemDrop(Type, [.. styles]);
	}

	public override void SetDefaults() => Item.DefaultToPlaceableTile(Tile.Type, _style);
	public override void AddRecipes()
	{
		CreateRecipe().AddRecipeGroup("ClayAndMud", 5).AddTile(ModContent.TileType<PotteryWheel>())
			.AddCondition(Language.GetText("Mods.SpiritReforged.Conditions.Discovered"), RecordedPot).Register();

		bool RecordedPot() => Main.LocalPlayer.GetModPlayer<RecordPlayer>().IsValidated(_name);
	}
}