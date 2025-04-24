using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration;

public class ClearReplaceable : GenAction
{
	public override bool Apply(Point origin, int x, int y, params object[] args) => WorldGen.TryKillingReplaceableTile(x, y, 0) ? UnitApply(origin, x, y, args) : Fail();
}

public class ReplaceType(ushort type) : GenAction
{
	private readonly ushort _type = type;

	public override bool Apply(Point origin, int x, int y, params object[] args)
	{
		Main.tile[x, y].TileType = _type;
		return UnitApply(origin, x, y, args);
	}
}

public class Send() : GenAction
{
	public override bool Apply(Point origin, int x, int y, params object[] args)
	{
		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, x, y);

		return UnitApply(origin, x, y, args);
	}
}