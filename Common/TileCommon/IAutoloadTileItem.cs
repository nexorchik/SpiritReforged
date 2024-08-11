using System.Linq;
using Terraria.ModLoader.Core;

namespace SpiritReforged.Common.TileCommon;

internal interface IAutoloadTileItem
{
	// These are already defined on ModTiles and shortens the autoloading code a bit.
	public string Name { get; }
	public string Texture { get; }
}

public class AutoloadTileItemSystem : ModSystem
{
	public override void Load()
	{
		var types = AssemblyManager.GetLoadableTypes(Mod.Code).Where(x => typeof(IAutoloadTileItem).IsAssignableFrom(x) && !x.IsAbstract);

		foreach (var item in types)
		{
			if (!typeof(ModTile).IsAssignableFrom(item))
				throw new InvalidCastException("IAutoloadTileItem should be placed on only ModTiles!");

			var instance = Activator.CreateInstance(item) as IAutoloadTileItem;
			Mod.AddContent(new AutoloadedTileItem(instance.Name + "Item", instance.Texture + "Item"));
		}
	}
}

public class AutoloadedTileItem(string name, string texture) : ModItem
{
	protected override bool CloneNewInstances => true;
	public override string Name => _internalName;
	public override string Texture => _texture;

	private string _internalName = name;
	private string _texture = texture;

	public override ModItem Clone(Item newEntity)
	{
		var item = base.Clone(newEntity) as AutoloadedTileItem;
		item._internalName = _internalName;
		item._texture = _texture;
		return item;
	}

	public override void SetDefaults() => Item.DefaultToPlaceableTile(Mod.Find<ModTile>(_internalName.Replace("Item", "")).Type);
}