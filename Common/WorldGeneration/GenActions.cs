using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration;

public class ClearReplaceable : GenAction
{
	public override bool Apply(Point origin, int x, int y, params object[] args) => WorldGen.TryKillingReplaceableTile(x, y, 0) ? UnitApply(origin, x, y, args) : Fail();
}