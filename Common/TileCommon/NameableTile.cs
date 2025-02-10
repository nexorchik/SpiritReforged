namespace SpiritReforged.Common.TileCommon;

/// <summary>
/// <inheritdoc cref="ModTile"/><para/>
/// Can be renamed externally by using <see cref="ChangeName"/>.
/// </summary>
public abstract class NameableTile : ModTile
{
	public string BaseName => GetType().Name;

	public void ChangeName(string name, string textureName = default)
	{
		_name = name;
		_textureName = textureName;
	}

	private string _name;
	private string _textureName;

	public override string Name => (_name == default) ? BaseName : _name;
	public override string Texture => (GetType().Namespace + "." + ((_textureName == default) ? BaseName : _textureName)).Replace('.', '/');
}
