using System.Linq;

namespace SpiritReforged.Common.TileCommon.PresetTiles;

public abstract class GrassTile : ModTile
{
	protected virtual int DirtType => TileID.Dirt;

	protected void AllowAnchor(params int[] types)
	{
		foreach (int type in types)
		{
			var data = TileObjectData.GetTileData(type, 0);
			if (data != null)
				data.AnchorValidTiles = data.AnchorValidTiles.Concat([Type]).ToArray();
		}
	}

	/// <summary>
	/// <inheritdoc/><para/>Also automatically controls common grass tile settings.
	/// </summary>
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileBlendAll[Type] = true;

		this.Merge(DirtType, TileID.Grass);
		AllowAnchor(TileID.Sunflower);
		TileID.Sets.Grass[Type] = true;
		TileID.Sets.CanBeDugByShovel[Type] = true;
		TileID.Sets.NeedsGrassFramingDirt[Type] = DirtType;
		TileID.Sets.NeedsGrassFraming[Type] = true;
	}

	public override bool CanExplode(int i, int j)
	{
		WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
		return true;
	}

	//Behavior for playing dust despite not killing the tile
	public virtual void DoDust(int i, int j, int amount = 5, int dustType = -1, float scale = 1f)
	{
		if (!Main.dedServ)
		{
			for(int d = 0; d < amount; d++)
				Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, dustType == -1 ? DustType : dustType, 0f, 0f, 0, default, scale);
		}
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!effectOnly) //Change self into dirt
		{
			fail = true;
			DoDust(i, j);
			Framing.GetTileSafely(i, j).TileType = (ushort)DirtType;
		}
	}
}
