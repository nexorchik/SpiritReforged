using System.Linq;
using Terraria.ModLoader.Core;

namespace SpiritReforged.Common.WallCommon;

/// <summary>
/// Automatically generates an item that places the given <see cref="ModWall"/> down.
/// </summary>
internal interface IAutoloadWallItem
{
	// These are already defined on ModTiles and shortens the autoloading code a bit.
	public string Name { get; }
	public string Texture { get; }
}

public class AutoloadWallItemSystem : ModSystem
{
	public override void Load()
	{
		var types = AssemblyManager.GetLoadableTypes(Mod.Code).Where(x => typeof(IAutoloadWallItem).IsAssignableFrom(x) && !x.IsAbstract);

		foreach (var item in types)
		{
			if (!typeof(ModWall).IsAssignableFrom(item))
				throw new InvalidCastException("IAutoloadTileItem should be placed on only ModTiles!");

			var instance = Activator.CreateInstance(item) as IAutoloadWallItem;
			Mod.AddContent(new AutoloadedWallItem(instance.Name + "Item", instance.Texture + "Item"));
		}
	}
}

public class AutoloadedWallItem(string name, string texture) : ModItem
{
	protected override bool CloneNewInstances => true;
	public override string Name => _internalName;
	public override string Texture => _texture;

	private string _internalName = name;
	private string _texture = texture;

	public override ModItem Clone(Item newEntity)
	{
		var item = base.Clone(newEntity) as AutoloadedWallItem;
		item._internalName = _internalName;
		item._texture = _texture;
		return item;
	}

	public override void SetDefaults() => Item.DefaultToPlaceableWall(Mod.Find<ModWall>(_internalName.Replace("Item", "")).Type);
}