namespace SpiritReforged.Common.TileCommon.FurnitureTiles;

public abstract class FurnitureTile : ModTile
{
	/// <summary> Defaults to <see cref="TileLoader.GetItemDropFromTypeAndStyle"/>. </summary>
	public virtual int MyItemDrop => TileLoader.GetItemDropFromTypeAndStyle(Type);

	public sealed override void SetStaticDefaults()
	{
		if (MyItemDrop > 0)
			RegisterItemDrop(MyItemDrop);

		StaticDefaults();
		PostStaticDefaults();
	}

	/// <summary> Functions like <see cref="SetStaticDefaults"/>. </summary>
	public virtual void StaticDefaults() { }

	/// <summary> Called directly after <see cref="StaticDefaults"/> to conveniently modify tile properties. </summary>
	public virtual void PostStaticDefaults() { }
}
